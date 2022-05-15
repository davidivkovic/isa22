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
using API.Infrastructure.Data.Queries;
using Microsoft.AspNetCore.Authorization;

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

    [Authorize]
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult> Get([FromRoute]Guid id, [FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var business = await Context.Set<TBusiness>()
            .AsNoTracking()
            .Include(b => b.Owner)
            .Include(b => b.Services)
            .Include(b => b.Rules)
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();

        if (business is null)
        {
            return NotFound();
        }

        var businessDTO = business.Adapt<TReadDTO>();

        var user = await Context.Users
            .Include(u => u.Subscriptions)
            .Where(u => u.Id == User.Id())
            .Select(u => new
            {
                IsSubscribed = u.Subscriptions.Contains(business),
                u.LoyaltyPoints
            })
            .FirstOrDefaultAsync();

        var sales = await Context.Sales
            .Where(s => s.Business.Id == business.Id)
            .Take(3)
            .ToListAsync();

        Loyalty loyaltyLevel = null;

        if (user is not null && start != default && end != default)
        {
            loyaltyLevel = await Context.LoyaltyLevels
                .AsNoTracking()
                .OrderByDescending(l => l.Threshold)
                .FirstOrDefaultAsync(l => l.Threshold <= user.LoyaltyPoints);

            businessDTO.LoyaltyLevel = loyaltyLevel;
            businessDTO.TotalPrice = business.Price(
                start,
                end,
                business.People,
                loyaltyLevel?.DiscountPercentage ?? 0,
                new()
            );
        }

        businessDTO.Sales = sales.Select(s =>
        {
            var sale = s.Adapt<SaleDTO>();
            sale.Price = s.Price(loyaltyLevel?.DiscountPercentage ?? 0);
            return sale;
        })
        .ToList();

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
                            .Include(b => b.Owner)
                            .FirstOrDefaultAsync(b => b.Id == dto.Id);

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
    
    [HttpGet("{id}/sale/preview-create")]
    public virtual async Task<ActionResult> PreviewCreateSale([FromRoute] Guid id, [FromBody] CreateSaleDTO request)
    {
        var business = await Context.Set<TBusiness>()
                    .Include(b => b.Owner)
                    .Include(b => b.Services)
                    .FirstOrDefaultAsync(b => b.Id == id);

        if (business.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        //TODO: Check if business is available at the given dates
        bool isAvailable = await Context.Businesses
            .Where(b => b.Id == id)
            .Available(request.Start, request.End)
            .AnyAsync();

        if (!isAvailable)
        {
            return BadRequest($"Your {BusinessType} is already occupied at that time.");
        }

        var loyaltyLevel = await Context.GetLoyaltyLevel(business.Owner.Id);

        var sale = business.Price(
            request.Start,
            request.End,
            request.People,
            loyaltyLevel.DiscountPercentage,
            request.Services
            //business.Services.Where(s => request.Services.Contains(s)).ToList()
        );

        return Ok(sale);
    }

    [Authorize(Roles = Role.Customer)]
    [HttpPost("{id}/make-reservation")]
    public async Task<ActionResult> MakeReservation([FromRoute] Guid id, [FromBody] MakeReservationDTO request)
    {
        return Ok("Hello");
        //var user = Context.Users.FindAsync();
    }

    [HttpGet("reservations")]
    protected async Task<ActionResult> GetReservations(string businessType, string status)
    {
        var currentTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        var reservationsQuery = Context.Reservations
            .AsNoTrackingWithIdentityResolution()
            .Where(r => r.User.Id == User.Id());

        reservationsQuery = businessType switch
        {
            "boats" => reservationsQuery.Where(b => b.Business is Boat),
            "adventures" => reservationsQuery.Where(b => b.Business is Adventure),
            "cabins" => reservationsQuery.Where(b => b.Business is Cabin),
            "all" or _ => reservationsQuery
        };

        reservationsQuery = status switch
        {
            "pending" => reservationsQuery.Where(r => r.Start > currentTime),
            "completed" => reservationsQuery.Where(r => r.End < currentTime),
            "ongoing" => reservationsQuery.Where(r => r.Start <= currentTime && r.End >= currentTime),
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