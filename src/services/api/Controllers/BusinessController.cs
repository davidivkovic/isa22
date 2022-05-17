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
    public async Task<ActionResult> Get([FromRoute]Guid id, [FromQuery] DateTimeOffset start, [FromQuery] DateTimeOffset end)
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
    
    [HttpPost("{id}/delete")]
    public virtual async Task<ActionResult> Delete(Guid id)
    {
        var business = await Context.Set<TBusiness>()
            .Include(b => b.Owner)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (business is null)
        {
            return BadRequest($"The specified {BusinessType} does not exist.");
        }

        if (business.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        business.Delete();
        await Context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/sales/preview-create")]
    public virtual async Task<ActionResult> PreviewCreateSale([FromRoute] Guid id, [FromBody] CreateSaleDTO request)
    {
        var business = await Context.Set<TBusiness>()
                    .Include(b => b.Owner)
                    .Include(b => b.Services)
                    .FirstOrDefaultAsync(b => b.Id == id);

        if (business is null)
        {
            return BadRequest($"The specified {BusinessType} does not exist.");
        }

        if (business.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        bool isAvailable = await Context.Businesses
            .Where(b => b.Id == id)
            .Available(request.Start, request.End)
            .AnyAsync();

        if (!isAvailable)
        {
            return BadRequest($"Your {BusinessType} is already occupied at that time.");
        }

        var loyaltyLevel = await Context.GetLoyaltyLevel(business.Owner.Id);
        var finance = await Context.Finances.FirstOrDefaultAsync();

        var price = business.Price(
            request.Start,
            request.End,
            request.People,
            request.DiscountPercentage,
            business.Services.Where(s => request.Services.Contains(s)).ToList()
        );

        return Ok(new Payment(price, finance.TaxPercentage * (100 / loyaltyLevel.DiscountPercentage)));
    }

    [HttpPost("{id}/sales/create")]
    public virtual async Task<ActionResult> CreateSale([FromRoute] Guid id, [FromBody] CreateSaleDTO request)
    {
        var business = await Context.Set<TBusiness>()
                    .Include(b => b.Owner)
                    .Include(b => b.Services)
                    .FirstOrDefaultAsync(b => b.Id == id);

        if (business is null)
        {
            return BadRequest($"The specified {BusinessType} does not exist.");
        }

        if (business.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        bool isAvailable = await Context.Set<TBusiness>()
            .Where(b => b.Id == id)
            .Available(request.Start, request.End)
            .AnyAsync();

        if (!isAvailable)
        {
            return BadRequest($"Your {BusinessType} is already occupied at that time.");
        }

        var loyaltyLevel = await Context.GetLoyaltyLevel(business.Owner.Id);

        Context.Add(new Sale(
            business,
            request.Start,
            request.End,
            request.DiscountPercentage,
            request.People,
            business.Services.Where(s => request.Services.Contains(s)).ToList()
        ));
        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize]
    [AllowAnonymous]
    [HttpGet("{id}/sales")]
    public async Task<ActionResult> GetSales(Guid id)
    {
        var sales = await Context.Sales
            .AsNoTrackingWithIdentityResolution()
            .Where(s => s.Business.Id == id)
            .Take(4)
            .ToListAsync();

        Loyalty loyalty = new();

        if (sales.Count > 0)
        {
            loyalty = await Context.GetLoyaltyLevel(User.Id());
        }

        return Ok(sales.Select(sale => new
        {
            sale,
            Price = sale.Price(loyalty.DiscountPercentage)
        }));
    }

    [Authorize(Roles = Role.Customer)]
    [HttpPost("{id}/make-reservation")]
    public async Task<ActionResult> MakeReservation([FromRoute] Guid id, [FromBody] MakeReservationDTO request)
    {
        var business = await Context.Set<TBusiness>()
                    .Include(b => b.Services)
                    .FirstOrDefaultAsync(b => b.Id == id);

        if (business is null)
        {
            return BadRequest($"The specified {BusinessType} does not exist.");
        }

        bool isAvailable = await Context.Set<TBusiness>()
            .Where(b => b.Id == id)
            .Available(request.Start, request.End)
            .AnyAsync();

        if (!isAvailable)
        {
            return BadRequest($"The {BusinessType} is already occupied at that time.");
        }

        var user = await Context.Users.FindAsync(User.Id());
        var finance = await Context.Finances.FirstOrDefaultAsync();

        Reservation reservation = new(
            user,
            business,
            request.Start,
            request.End,
            business.People,
            business.Services.Where(s => request.Services.Contains(s)).ToList(),
            finance.TaxPercentage
        );

        Context.Add(reservation);
        await Context.SaveChangesAsync();

        return Ok(reservation.Id);
    }

    [HttpGet("reservations")]
    protected async Task<ActionResult> GetReservations(string businessType, string status)
    {
        var currentTime = DateTimeOffset.Now;

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