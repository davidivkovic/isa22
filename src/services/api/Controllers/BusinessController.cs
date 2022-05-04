namespace API.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Mapster;

using API.Core.Model;
using API.DTO;
using API.Infrastructure.Data;
using API.Infrastructure.Extensions;
using API.Services;

[Route("api/[controller]")]
[ApiController]
public class BusinessController<
    TBusiness,
    TReadDTO,
    TCreateDTO,
    TUpdateDTO
> : ControllerBase 
    where TBusiness : Business
    where TReadDTO : BusinessDTO
    where TUpdateDTO : UpdateBusinessDTO
    where TCreateDTO : CreateBusinessDTO
{
    protected virtual string BusinessType { get; set; }

    protected AppDbContext Context { get; }

    public BusinessController(AppDbContext dbContext)
    {
        Context = dbContext;
    }

    protected string ImageUrl(Guid id, string image)
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
        var business = await Context.Set<TBusiness>()
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(a => a.Id == id);
        if (business is null)
        {
            return BadRequest($"The requested {BusinessType} does not exist.");
        }

        return Ok(business.Images.Select(image => ImageUrl(id, image)));
    }


    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var businessDTO = await Context.Set<TBusiness>()
            .AsNoTracking()
            .Include(a => a.Services)
            .Include(a => a.Rules)
            .Where(a => a.Id == id)
            .ProjectToType<TReadDTO>()
            .FirstOrDefaultAsync();

        if (businessDTO is null)
        {
            return NotFound();
        }

        businessDTO.WithImages(ImageUrl);

        return Ok(businessDTO);
    }

    [HttpPost("{id}/images/add")]
    public virtual async Task<ActionResult> AddImage(Guid id, [FromForm] List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            return BadRequest("No images to add!");
        }

        var business = await Context.Set<TBusiness>()
                            .Include(a => a.Owner)
                            .FirstOrDefaultAsync(a => a.Id == id);

        if (business.Owner.Id != User.Id())
        {
            return BadRequest($"Cannot update someone else's {BusinessType}.");
        }

        if (business is null)
        {
            return BadRequest($"The requested {BusinessType} does not exist.");
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
                ImageService.Persist(business.Id, image.FileName, fs => image.CopyToAsync(fs)
            )
        ));

        business.Images = new(images);
        await Context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("create")]
    public virtual async Task<IActionResult> Create(TCreateDTO dto)
    {
        User user = await Context.Users.FindAsync(User.Id());
        if (user is null)
        {
            return BadRequest($"The user with email {user.Email} does not exist.");
        }

        var business = dto.Adapt<TBusiness>();
        business.Id = Guid.NewGuid();
        business.Owner = user;

        Context.Set<TBusiness>().Add(business);
        await Context.SaveChangesAsync();

        return Ok(business.Id);
    }

    [HttpPost("update")]
    public virtual async Task<ActionResult> Update(TUpdateDTO dto)
    {
        var business = await Context.Set<TBusiness>()
                            .Include(a => a.Owner)
                            .FirstOrDefaultAsync(a => a.Id == dto.Id);

        if (business.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        dto.Adapt(business);
        bool success = await Context.SaveChangesAsync() > 0;

        if (!success)
        {
            BadRequest($"Could not update your {BusinessType} at this time. Please try again later.");
        }

        return Ok(business.Id);
    }

    [HttpPost("{businessId}/make-reservation")]
    public virtual async Task<ActionResult> MakeReservation([FromRoute] Guid businessId, [FromBody] MakeReservationDTO request)
    {
        return Ok("Hello");
        //var user = Context.Users.FindAsync();
    }

    [HttpGet("reservations")]
    public virtual async Task<ActionResult> GetReservations(string status)
    {
        var reservationsQuery = Context.Reservations
        .AsNoTrackingWithIdentityResolution()
        .Where(r => r.User.Id == User.Id());

        reservationsQuery = status switch
        {
            "pending" => reservationsQuery.Where(r => r.Start > DateTime.Now),
            "completed" => reservationsQuery.Where(r => r.End < DateTime.Now),
            "ongoing" => reservationsQuery.Where(r => r.Start <= DateTime.Now && r.End >= DateTime.Now),
            "all" or _ => reservationsQuery
        };

        var reservations = await reservationsQuery
            .OrderBy(r => r.Start)
            .Take(10)
            .ProjectToType<ReservationDTO>()
            .ToListAsync();

        reservations.ForEach(r => r.Business.WithImages(ImageUrl));

        return Ok(reservations);
    }
}