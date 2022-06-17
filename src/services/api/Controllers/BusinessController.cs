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
using API.Services.Email;
using API.Services.Email.Messages;
using API.DTO.Search;

[Route("api/[controller]")]
[ApiController]
public class BusinessController<
    TBusiness,
    TReadDTO,
    TCreateDTO,
    TUpdateDTO,
    TSearchDTO
> : ControllerBase 
    where TBusiness : Business
    where TReadDTO : BusinessDTO
    where TUpdateDTO : UpdateBusinessDTO
    where TCreateDTO : CreateBusinessDTO
    where TSearchDTO : SearchResponse
{
    private readonly Mailer _mailer;

    protected virtual string BusinessType { get; set; }
    protected AppDbContext Context { get; }

    public BusinessController(AppDbContext dbContext, Mailer mailer)
    {
        Context = dbContext;
        _mailer = mailer;
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

    protected async Task<ActionResult> GetBusinesses([FromQuery] SearchRequest request)
    {
        var query = Context.Set<TBusiness>()
            .Where(b => b.Owner.Id == User.Id())
            .AsNoTrackingWithIdentityResolution();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(b => EF.Functions.ILike(b.Name, $"%{request.Name}%"));
        };
        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(b => EF.Functions.ILike(b.Address.City, $"%{request.City}%"));
        };
        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(b => EF.Functions.ILike(b.Address.Country, $"%{request.Country}%"));
        }
        if (request.People != 0)
        {
            query = query.Where(c => c.People == request.People);
        }

        int totalResults = await query.CountAsync();

        var results = await query
            .OrderBy(request.Direction)
            .Skip(request.Page * 6)
            .Take(6)
            .ProjectToType<TSearchDTO>()
            .ToListAsync();

            results.ForEach(r =>
            {
                r.Image = ImageUrl(r.Id, r.Images.FirstOrDefault());
                r.Price = new Money
                {
                    Amount = r.PricePerUnit.Amount,
                    Currency = r.PricePerUnit.Currency
                };
            });
        return Ok(new { results, totalResults, averageRating = results.Average(c => c.Rating) });
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

    [HttpGet("{id}/name")]
    public async Task<ActionResult> GetName(Guid id)
    {
        var businessName = await Context.Set<TBusiness>()
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => b.Name)
            .FirstOrDefaultAsync();

        if (businessName is null)
        {
            return NotFound();
        }

        return Ok(businessName);
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


        businessDTO.IsSubscribed = user?.IsSubscribed ?? false;

        var sales = await Context.Sales
            .Include(s => s.Business)
            .Where(s => s.Payment == null)
            .Where(s => s.Business.Id == business.Id)
            .OrderBy(s => s.Start)
            .Take(3)
            .ToListAsync();

        var reviews = await Context.Reviews
            .Include(r => r.User)
            .Where(r => r.Business.Id == business.Id)
            .Where(r => r.Approved)
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
                    .Include(b => b.Subscribers)
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

        sale.Business.Subscribers.ForEach(u => _mailer.Send(u, new NewSale(
            u.FullName,
            sale,
            ImageUrl(sale.Business.Id, sale.Business.Images.FirstOrDefault()),
            "#contactUrl"
        )));

        return Ok(sale.Adapt<SaleDTO>());
    }

    [Authorize(Roles = Role.Customer)]
    [HttpPost("{id}/subscribe")]
    public async Task<ActionResult> Subscribe([FromRoute] Guid id)
    {
        var business = await Context.Set<TBusiness>()
                    .FirstOrDefaultAsync(b => b.Id == id);

        var user = await Context.Users
                   .Include(u => u.Subscriptions)
                   .FirstOrDefaultAsync(u => u.Id == User.Id());

        bool isSubscribed = user.Subscriptions.Contains(business);
        if (isSubscribed)
        {
            return BadRequest($"You are already subscribed to this {BusinessType}.");
        }

        user.Subscribe(business);
        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Roles = Role.Customer)]
    [HttpPost("{id}/unsubscribe")]
    public async Task<ActionResult> Unubscribe([FromRoute] Guid id)
    {
        var user = await Context.Users
                   .Include(u => u.Subscriptions)
                   .FirstOrDefaultAsync(u => u.Id == User.Id());

        user.Subscriptions.RemoveAll(b => b.Id == id);
        await Context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/sales/delete")]
    public virtual async Task<ActionResult> DeleteSale([FromRoute] Guid id, [FromRoute] Guid saleId)
    {
        var business = await Context.Set<TBusiness>()
            .Include(b => b.Owner)
            .Include(b => b.Reservations.Where(r => r.Id == saleId))
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (business is null)
        {
            return BadRequest($"The specified {BusinessType} does not exist.");
        }

        if (business.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        if (business.Reservations.Exists(r => r.User is not null))
        {
            return BadRequest("Cannot delete a sale after reservation.");
        }

        business.Reservations.ForEach(r => r.Delete());
        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize]
    [AllowAnonymous]
    [HttpGet("{id}/sales")]
    // TODO: Get only future sales
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
    public async Task<ActionResult> MakeQuickReservation([FromRoute] Guid id, [FromRoute] Guid saleId)
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
        user.LoyaltyPoints += finance.CustomerPoints;
        await Context.SaveChangesAsync();

        _mailer.Send(user, new ReservationCreated(
            sale,
            ImageUrl(sale.Business.Id, sale.Business.Images.FirstOrDefault()),
            "#contactUrl"
         ));

        return Ok();
    }


    [Authorize(Roles = Role.Customer)]
    [HttpPost("{id}/review")]
    public async Task<ActionResult> Review([FromRoute] Guid id, [FromBody] CreateReviewDTO request)
    {
        var business = await Context.Set<TBusiness>()
                    .Include(b => b.Owner)
                    .FirstOrDefaultAsync(b => b.Id == id);

        if (business is null)
        {
            return BadRequest($"The specified {BusinessType} does not exist.");
        }

        var user = await Context.Users.FindAsync(User.Id());

        var reservations = await Context.Reservations
            .Where(r => r.User.Id == user.Id)
            .Where(s => s.Business.Id == id)
            .ToListAsync();

        var reviews = await Context.Reviews
            .Include(r => r.User)
            .Include(r => r.Business)
            .Where(r => r.User.Id == user.Id)
            .Where(r => r.Approved || r.Rejected)
            .Where(r => r.Business.Id == id)
            .ToListAsync();

        if (reservations.Count == 0)
        {
            return BadRequest($"You must have a completed reservation in order to leave a review.");
        }

        if (reviews.Count > 0)
        {
            return BadRequest($"You have already reviewed this business.");
        }

        Review review = business.Review(user, request.Rating, request.Content);
        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Roles = Role.Customer)]
    [HttpPost("reservations/{reservationId}/complain")]
    public async Task<ActionResult> Complain([FromRoute] Guid reservationId, [FromBody] CreateComplaintDTO request)
    {
        var user = await Context.Users.FindAsync(User.Id());

        var reservation = await Context.Reservations
            .Where(r => r.User.Id == user.Id)
            .Where(r => r.Id == reservationId)
            .FirstOrDefaultAsync();

        var isPast = reservation.End < DateTimeOffset.UtcNow;

        if (reservation is null)
        {
            return BadRequest("You must have a completed reservation in order to leave a complaint.");
        }

        if(reservation.Complaint is not null)
        {
            return BadRequest("You have already complained about this reservation.");
        }

        if (!isPast)
        {
            return BadRequest("The reservation is still not over.");
        }

        reservation.Complain(request.Content);

        await Context.SaveChangesAsync();
        return Ok(reservation.Complaint);

    }

    [HttpPost("reservations/{reservationId}/report")]
    public virtual async Task<ActionResult> Report([FromRoute] Guid reservationId, CreateReportDTO request)
    {
        var user = await Context.Users.FindAsync(User.Id());

        var reservation = await Context.Reservations
            .Where(r => r.Business.Owner.Id == user.Id)
            .Where(r => r.Id == reservationId)
            .FirstOrDefaultAsync();

        var isPast = reservation.End < DateTimeOffset.UtcNow;

        if (reservation is null)
        {
            return BadRequest("There must be a completed reservation in order to leave a report.");
        }

        if (reservation.Report is not null)
        {
            return BadRequest("You have already submitted a report.");
        }

        reservation.ReportUser(request.Reason, request.Penalize);
        await Context.SaveChangesAsync();

        return Ok(reservation.Report);
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
        user.LoyaltyPoints += finance.CustomerPoints;
        await Context.SaveChangesAsync();

        _mailer.Send(user, new ReservationCreated(
            reservation,
            ImageUrl(reservation.Business.Id, reservation.Business.Images.FirstOrDefault()),
            "#contactUrl"
         ));

        return Ok(reservation.Id);
    }

    [Authorize]
    [HttpGet("reservations")]
    public async Task<ActionResult> GetReservations(string status, int page, int size = 10, bool isDashboard = false)
    {
        size = Math.Clamp(size, 0, 20);
        var currentTime = DateTimeOffset.Now;
        var isCustomer = User.IsInRole(Role.Customer);

        var reservationsQuery = Context.Reservations
            .Include(r => r.Payment)
            .AsNoTrackingWithIdentityResolution()
            .Where(r => r.User != null)
            .Where(r => r.Business is TBusiness)
            .Where(r => r.Status != Reservation.ReservationStatus.Cancelled);

        if (isCustomer)
        {
            reservationsQuery = reservationsQuery.Where(r => r.User.Id == User.Id());
        }
        else
        {
            reservationsQuery = reservationsQuery.Where(r => r.Business.Owner.Id == User.Id());
        }

        reservationsQuery = status switch
        {
            "pending" => reservationsQuery.Where(r => r.Start > currentTime),
            "completed" => reservationsQuery.Where(r => r.End < currentTime),
            "ongoing" => reservationsQuery.Where(r => r.Start <= currentTime && r.End >= currentTime),
            "all" or _ => reservationsQuery
        };

        int totalResults = await reservationsQuery.CountAsync();
        reservationsQuery = reservationsQuery
            .OrderBy(r => r.Start)
            .Skip(page * size)
            .Take(size);

        if (isDashboard)
        {
            var reservations = await reservationsQuery
                .ProjectToType<DashboardReservationDTO>()
                .ToListAsync();
            reservations.ForEach(r => r.Business.WithImages(ImageUrl));

            return Ok(new { Results = reservations, totalResults });
        }
        else
        {
            var reservations = await reservationsQuery
                .ProjectToType<ReservationDTO>()
                .ToListAsync();
            reservations.ForEach(r => r.Business.WithImages(ImageUrl));
            reservations.ForEach(r => r.IsCancellable = r.Start - currentTime >= TimeSpan.FromDays(3));

            return Ok(new { Results = reservations, totalResults });
        }
    }

    // TODO: Ultra slow, probably a bug
    [HttpGet("subscriptions")]
    public async Task<ActionResult> GetSubscriptions()
    {
        var subscriptions = await Context.Users
                    .AsNoTrackingWithIdentityResolution()
                    .Where(u => u.Id == User.Id())
                    .SelectMany(u => u.Subscriptions)
                    .Where(b => b is TBusiness)
                    .ProjectToType<SubscriptionDTO>()
                    .ToListAsync();
        subscriptions.ForEach(s => s.WithImages(ImageUrl));

        return Ok(subscriptions);
    }

    [Authorize(Roles = Role.Customer)]
    [HttpPost("reservations/{reservationId}/cancel")]
    public async Task<ActionResult> CancelReservation([FromRoute] Guid reservationId)
    {
        var reservation = await Context.Set<Reservation>()
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
        {
            return BadRequest($"The specified reservation does not exist.");
        }

        bool cancelled = reservation.Cancel();
        if(!cancelled)
        {
            return BadRequest($"The specified reservation cannot be cancelled.");
        }

        var finance = await Context.Finances.FirstOrDefaultAsync();
        reservation.User.LoyaltyPoints -= finance.CustomerPoints;

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
            .Where(s =>
                s.Start >= start && s.Start <= end || // start this week
                s.End >= start && s.End <= end ||     // end this week
                s.Start <= start && s.End >= end      // pass through this week
            )
            .Select(r => new
            {
                r.Id,
                r.Start,
                r.End,
                Type = r.GetType().Name.ToLowerInvariant(),
                Name = r.User == null ? "" : r.User.FirstName + " " + r.User.LastName,
                Reported = r.Report != null
            })
            .ToListAsync();

        var slots = await Context.Businesses
                .AsNoTracking()
                .Where(b => b.Id == id)
                .SelectMany(b => b.Availability)
                .Where(s =>
                    s.Start >= start && s.Start <= end || // start this week
                    s.End >= start && s.End <= end ||     // end this week
                    s.Start <= start && s.End >= end      // pass through this week
                )
                .Select(r => new
                {
                    r.Id,
                    r.Start,
                    r.End,
                    Type = "unavailable",
                    Name = "Unavailable",
                    Reported = false
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

    protected async Task<ActionResult> Search(SearchRequest request, Func<IQueryable<TBusiness>, IQueryable<TBusiness>> filter = null)
    {
        decimal discountMultiplier = await Context.GetDiscountMultiplier(User.Id());

        var query = Context.Set<TBusiness>()
            .AsNoTrackingWithIdentityResolution()
            .Available(request.Start.UtcDateTime, request.End.UtcDateTime)
            .Where(c => c.Address.City == request.City)
            .Where(c => c.Address.Country == request.Country)
            .Where(c => c.People == request.People);

        if (request.RatingHigher != default)
        {
            query = query.Where(b => b.Rating >= request.RatingHigher);
        }
        if (request.PriceLow != default)
        {
            query = query.Where(b => b.PricePerUnit.Amount >= request.PriceLow);
        }
        if (request.PriceHigh != default)
        {
            query = query.Where(b => b.PricePerUnit.Amount <= request.PriceHigh);
        }

        if (filter is not null) query = filter.Invoke(query);

        int totalResults = await query.CountAsync();
        var results = await query
            .OrderBy(request.Direction)
            .Skip(request.Page * 6)
            .Take(6)
            .ProjectToType<TSearchDTO>()
            .ToListAsync();

        results.ForEach(r =>
        {
            r.Image = ImageUrl(r.Id, r.Images.FirstOrDefault());
            r.Price = new Money
            {
                Amount = r.PricePerUnit.Amount * request.People,
                Currency = r.PricePerUnit.Currency
            };
        });

        return Ok(new
        {
            results,
            totalResults,
        });
    }

}