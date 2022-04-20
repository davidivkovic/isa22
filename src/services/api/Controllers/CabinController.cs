namespace API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Mapster;

using API.DTO;
using API.Services;
using API.Core.Model;
using API.Infrastructure.Data;
using API.Infrastructure.Data.Queries;
using API.Infrastructure.Extensions;
using API.DTO.Search;

[ApiController]
[Route("cabins")]
public class CabinController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public CabinController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private string ImageUrl(Guid id, string image)
    {
        return Url.Action(
            nameof(GetImage),
            ControllerContext.ActionDescriptor.ControllerName,
            new { id, image },
            Request.Scheme
        );
    }

    [HttpGet("{id}/images/{image}")]
    public ActionResult GetImage([FromRoute] Guid id, [FromRoute] string image)
    {
        string imagePath = ImageService.GetPath(id, image);
        if (imagePath is null)
        {
            return BadRequest();
        }

        return PhysicalFile(imagePath, "image/*");
    }

    [HttpGet("{id}/images")]
    public async Task<ActionResult> GetImages(Guid id)
    {
        var cabin = await _dbContext.Cabins
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(a => a.Id == id);
        if (cabin is null)
        {
            return BadRequest("The requested adventure does not exist.");
        }

        return Ok(cabin.Images.Select(image => ImageUrl(id, image)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var cabinDTO = await _dbContext.Cabins
            .AsNoTracking()
            .Include(a => a.Services)
            .Include(a => a.Rules)
            .Where(a => a.Id == id)
            .ProjectToType<CabinDTO>()
            .FirstOrDefaultAsync();

        if (cabinDTO is null)
        {
            return NotFound();
        }

        cabinDTO.WithImages(ImageUrl);

        return Ok(cabinDTO);
    }

    [HttpPost("{id}/images/add")]
    [Authorize(Roles = Role.CabinOwner)]
    public async Task<ActionResult> AddImage(Guid id, [FromForm] List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            return BadRequest("No images to add!");
        }

        var cabin = await _dbContext.Cabins
                            .Include(a => a.Owner)
                            .FirstOrDefaultAsync(a => a.Id == id);

        if (cabin.Owner.Id != User.Id())
        {
            return BadRequest("Cannot update someone else's cabin.");
        }

        if (cabin is null)
        {
            return BadRequest("The requested cabin does not exist.");
        }

        if (files.Count > 10)
        {
            return BadRequest("A maximum of 10 images can be uploaded.");
        }

        foreach (var file in files)
        {
            if (!ImageService.IsValid(file.FileName, file.OpenReadStream()))
            {
                return BadRequest($"The file {file.FileName} is not a valid image.");
            }
        }

        var images = await Task.WhenAll(
            files.Select(image =>
            ImageService.Persist(cabin.Id, image.FileName, fs => image.CopyToAsync(fs))
            )
        );

        cabin.Images = new(images);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("create")]
    [Authorize(Roles = Role.CabinOwner)]
    public async Task<IActionResult> Create(CreateCabinDTO dto)
    {
        User user = await _dbContext.Users.FindAsync(User.Id());
        if (user is null)
        {
            return BadRequest($"The user with email {user.Email} does not exist.");
        }

        var cabin = dto.Adapt<Cabin>();
        cabin.Id = Guid.NewGuid();
        cabin.Owner = user;

        _dbContext.Cabins.Add(cabin);
        await _dbContext.SaveChangesAsync();

        return Ok(cabin.Id);
    }

    [HttpPost("update")]
    [Authorize(Roles = Role.CabinOwner)]
    public async Task<ActionResult> Update(UpdateCabinDTO dto)
    {
        var cabin = await _dbContext.Cabins
                            .Include(a => a.Owner)
                            .FirstOrDefaultAsync(a => a.Id == dto.Id);

        if (cabin.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        dto.Adapt(cabin);
        bool success = await _dbContext.SaveChangesAsync() > 0;

        if (!success)
        {
            BadRequest("Could not update your cabin at this time. Please try again later.");
        }

        return Ok(cabin.Id);
    }

    [Authorize]
    [AllowAnonymous]
    [HttpGet("/search")]
    public async Task<List<CabinSearchResponse>> Search([FromQuery] CabinSearchRequest request)
    {
        decimal multiplier = 1;
        if (User.Identity.IsAuthenticated)
        {
            var user = await _dbContext.Users
                .Include(u => u.Level)
                .FirstOrDefaultAsync(u => u.Id == User.Id());

            if (user is not null) 
            {
                multiplier = 1 - Convert.ToDecimal(user.Level?.DiscountPercentage ?? 0 / 100);
            }
        }

        int totalUnits = new Cabin().GetTotalUnits(request.Start, request.End);

        var query = _dbContext.Cabins
            .AsNoTrackingWithIdentityResolution()
            //.Available(request.Start, request.End)
            .Where(c => c.Address.City == request.City)
            .Where(c => c.Address.Country == request.Country)
            .Where(c => c.People == request.People);
            
        if (request.RatingHigher != default)
        {
            query = query.Where(c => c.Rating >= request.RatingHigher);
        }
        if (request.PriceLow != default)
        {
            query = query.Where(c => c.PricePerUnit.Amount >= request.PriceLow);
        }
        if (request.PriceHigh != default)
        {
            query = query.Where(c => c.PricePerUnit.Amount <= request.PriceHigh);
        }
        if (request.Rooms != default)
        {
            query = query.Where(c => c.Rooms.Count == request.Rooms);
        }

        var results = await query
            .OrderBy(request.Direction)
            .Take(9)
            .Select(c => new CabinSearchResponse
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                Beds = c.Rooms.Sum(r => r.Beds),
                Image = c.Images.FirstOrDefault(),
                People = c.People,
                Rooms = c.Rooms.Count,
                Rating = c.Rating,
                Price = new Money
                {
                    Amount = c.PricePerUnit.Amount * request.People * totalUnits * multiplier,
                    Currency = c.PricePerUnit.Currency
                }
            }) 
            .ToListAsync();

        results.ForEach(r => r.Image = ImageUrl(r.Id, r.Image));

        return results;
            
    }
}