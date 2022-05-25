namespace API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using API.DTO;
using API.Infrastructure.Data;
using API.Infrastructure.Data.Queries;
using API.Infrastructure.Extensions;
using API.DTO.Search;
using API.Core.Model;
using Microsoft.EntityFrameworkCore;
using API.DTO.Report;
using API.Services.Email;

[ApiController]
[Route("boats")]
public class BoatController : BusinessController<Boat, BoatDTO, CreateBoatDTO, UpdateBoatDTO>
{
    protected override string BusinessType => "boat";

    public BoatController(AppDbContext dbContext, Mailer mailer) : base(dbContext, mailer) { }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<IActionResult> Create(CreateBoatDTO dto)
    {
        return base.Create(dto);
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<ActionResult> Update(UpdateBoatDTO dto)
    {
        return base.Update(dto);
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<ActionResult> Delete(Guid id)
    {
        return base.Delete(id);
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<ActionResult> AddImage(Guid id, [FromForm] List<IFormFile> files)
    {
        return base.AddImage(id, files);
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<ActionResult> PreviewCreateSale([FromRoute] Guid id, [FromBody] CreateSaleDTO request)
    {
        return base.PreviewCreateSale(id, request);
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<ActionResult> CreateSale([FromRoute] Guid id, [FromBody] CreateSaleDTO request)
    {
        return base.CreateSale(id, request);
    }

    [Authorize]
    [HttpGet("reservations")]
    public Task<ActionResult> GetReservations(string status)
    {
        return GetReservations("boats", status);
    }

    [Authorize]
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ActionResult> Search([FromQuery] BoatSearchRequest request)
    {
        int totalUnits = new Boat().GetTotalUnits(request.Start, request.End);
        decimal discountMultiplier = await Context.GetDiscountMultiplier(User.Id());

        var query = Context.Boats
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
        if (request.Seats != default)
        {
            if (request.Seats >= 5)
            {
                query = query.Where(b => b.Characteristics.Seats >= 5);
            }
            else
            { 
                query = query.Where(b => b.Characteristics.Seats == request.Seats);
            }
        }

        int totalResults = await query.CountAsync();
        var results = await query
            .OrderBy(request.Direction)
            .Skip(request.Page * 6)
            .Take(6)
            .Select(b => new BoatSearchResponse
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Image = b.Images.FirstOrDefault(),
                People = b.People,
                Rating = b.Rating,
                BoatCharacteristics = b.Characteristics,
                Price = new Money
                {
                    Amount = b.PricePerUnit.Amount,
                    Currency = b.PricePerUnit.Currency
                }
            })
            .ToListAsync();

        results.ForEach(r => r.Image = ImageUrl(r.Id, r.Image));

        return Ok(new
        {
            results,
            totalResults,
        });
    }

    [HttpGet("/boat-owner/{id}/boats")]
    [Authorize(Roles = Role.BoatOwner)]
    [AllowAnonymous]
    public async Task<ActionResult> SearchOwnersCabins([FromQuery] SearchRequest request)
    {
        var query = Context.Boats
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

        var results = await query
            .OrderBy(request.Direction)
            .Take(9)
            .Select(c => new SearchResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Address = c.Address,
                Image = c.Images.FirstOrDefault(),
                People = c.People,
                Rating = c.Rating,
                CancellationFee = c.CancellationFee,
                Price = new Money
                {
                    Amount = c.PricePerUnit.Amount,
                    Currency = c.PricePerUnit.Currency
                }
            })
            .ToListAsync();

        results.ForEach(r => r.Image = ImageUrl(r.Id, r.Image));

        return Ok(new { results });
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<List<ReportReservationResponse>> GetReservationsInPeriod(DateTime startDate, DateTime endDate)
    {
        return base.GetReservationsInPeriod(startDate, endDate);
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<List<ReportPaymentResponse>> GetPaymentReport(DateTime startDate, DateTime endDate)
    {
        return base.GetPaymentReport(startDate, endDate);
    }
}