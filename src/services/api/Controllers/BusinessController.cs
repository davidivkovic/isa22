namespace API.Controllers;

using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using Mapster;

using API.Core.Model;
using API.DTO;
using API.Infrastructure.Data;
using API.Infrastructure.Extensions;
using API.Services;
using API.Infrastructure.Data.Queries;

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
    public async Task<ActionResult> Get([FromRoute] Guid id, [FromQuery] DateTimeOffset start, [FromQuery] DateTimeOffset end)
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
            .Include(s => s.Business)
            .Where(s => s.Business.Id == business.Id)
            .Take(3)
            .ToListAsync();

        var reviews = await Context.Reviews
            .Include(s => s.User)
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

        businessDTO.Reviews = reviews.Select(r => r.Adapt<ReviewDTO>()).ToList();

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
                    .AsNoTracking()
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
            .AsNoTracking()
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

        return Ok(new Payment(price, finance.TaxPercentage * (1 - (loyaltyLevel?.DiscountPercentage ?? 0) / 100)));
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

        var sale = new Sale(
            business,
            request.Start,
            request.End,
            request.DiscountPercentage,
            request.People,
            business.Services.Where(s => request.Services.Contains(s)).ToList()
        );
        Context.Add(sale);
        await Context.SaveChangesAsync();

        return Ok(sale.Adapt<SaleDTO>());
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
    [HttpPost("{id}/sales/{saleId}/book")]
    public async Task<ActionResult> MakeQuickReservation([FromQuery] Guid id, [FromQuery] Guid saleId)
    {
        var sale = await Context.Set<Sale>()
                    .Include(s => s.Business)
                    .Where(s => s.Business.Id == id)
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == saleId);

        if (sale is null)
        {
            return BadRequest($"The specified sale does not exist.");
        }

        if (sale.User is not null)
        {
            return BadRequest($"Sorry, the specified sale is already booked.");

        }

        var user = await Context.Users.FindAsync(User.Id());
        var finance = await Context.Finances.FirstOrDefaultAsync();
        var loyalty = await Context.GetLoyaltyLevel(User.Id());

        sale.Sell(user, loyalty?.DiscountPercentage ?? 0, finance.TaxPercentage);
        await Context.SaveChangesAsync();

        return Ok();
    }


    [Authorize(Roles = Role.Customer)]
    [HttpPost("{id}/review")]
    public async Task<ActionResult> Review([FromRoute] Guid id, [FromBody] CreateReviewDTO request)
    {
        var business = await Context.Set<TBusiness>()
                    .FirstOrDefaultAsync(b => b.Id == id);
        var user = await Context.Users.FindAsync(User.Id());

        // Check if user had previous reservation

        if (business is null)
        {
            return BadRequest($"The specified {BusinessType} does not exist.");
        }

        business.Review(user, request.Rating, request.Content);
        await Context.SaveChangesAsync();

        return Ok();
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
            .Where(r => r.User.Id == User.Id())
            .Where(r => r.Status != "Cancelled");

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
        reservations.ForEach(r => r.IsCancellable = r.Start - currentTime >= TimeSpan.FromDays(3));

        return Ok(reservations);
    }

    [Authorize(Roles = Role.Customer)]
    [HttpPost("reservations/{reservationId}/cancel")]
    public async Task<ActionResult> CancelReservation([FromRoute] Guid reservationId)
    {
        var reservation = await Context.Set<Reservation>().FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
        {
            return BadRequest($"The specified reservation does not exist.");
        }

        bool canceled = reservation.Cancel();
        if(!canceled)
        {
            return BadRequest($"The specified reservation cannot be canceled.");
        }

        await Context.SaveChangesAsync();
        return Ok();

    }

    

    [HttpGet("{id}/calendar")]
    public virtual async Task<ActionResult> GetCalendar(Guid id, DateTimeOffset start, DateTimeOffset end)
    {
        var business = await Context.Set<TBusiness>()
            .AsNoTracking()
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

        var reservations = await Context.Reservations
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.Business.Id == id)
            .Where(r => r.Start >= start)
            .Where(r => r.End <= end)
            .Select(r => new
            {
                r.Id,
                r.Start,
                r.End,
                Type = "reservation",
                Name = r.User.FirstName + " " + r.User.LastName
            })
            .ToListAsync();

        var slots = await Context.Businesses
                .AsNoTracking()
                .Where(b => b.Id == id)
                .SelectMany(b => b.Availability)
                .Where(s => s.Start >= start)
                .Where(s => s.End <= end)
                .Select(r => new
                {
                    r.Id,
                    r.Start,
                    r.End,
                    Type = "unavailable",
                    Name = "Unavailable"
                })
                .ToListAsync();

        return Ok(reservations.Concat(slots));
    }

    [HttpPost("{id}/calendar/create-unavailability")]
    public virtual async Task<ActionResult> CreateUnavailability(Guid id, DateTimeOffset start, DateTimeOffset end)
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

        Slot slot = new()
        {
            Start = start,
            End = end,
            Available = false
        };

        business.Availability.Add(slot);
        Context.SaveChanges();

        return Ok(new
        {
            slot.Id,
            slot.Start,
            slot.End,
            Type = "unavailable",
            Name = "Unavailable"
        });
    }

    [HttpPost("{id}/calendar/delete-unavailability")]
    public virtual async Task<ActionResult> DeleteUnavailability(Guid id, Guid eventId)
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

        var slot = await Context.Set<TBusiness>()
            .SelectMany(b => b.Availability)
            .Where(s => s.Id == eventId)
            .FirstOrDefaultAsync();

        if (slot is null)
        {
            return BadRequest("The specified slot does not exist.");
        }

        slot.Delete();
        await Context.SaveChangesAsync();

        return Ok();
    }

}