namespace API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using API.DTO;
using API.Core.Model;
using API.Infrastructure.Data;
using API.Infrastructure.Data.Queries;
using API.Infrastructure.Extensions;
using API.DTO.Search;

[ApiController]
[Route("cabins")]
public class CabinController : BusinessController<Cabin, CabinDTO, CreateCabinDTO, UpdateCabinDTO>
{
    protected override string BusinessType => "cabin";

    public CabinController(AppDbContext dbContext) : base(dbContext) { }

    [Authorize(Roles = Role.CabinOwner)]
    public override Task<ActionResult> Update(UpdateCabinDTO dto)
    {
        return base.Update(dto);
    }

    [Authorize(Roles = Role.CabinOwner)]
    public override Task<IActionResult> Create(CreateCabinDTO dto)
    {
        return base.Create(dto);
    }

    [Authorize(Roles = Role.CabinOwner)]
    public override Task<ActionResult> AddImage(Guid id, [FromForm] List<IFormFile> files)
    {
        return base.AddImage(id, files);
    }

    [Authorize(Roles = Role.Customer)]
    public override Task<ActionResult> MakeReservation([FromRoute] Guid businessId, [FromBody] MakeReservationDTO request)
    {
        return base.MakeReservation(businessId, request);
    }

    [Authorize]
    public override Task<ActionResult> GetReservations(string status)
    {
        return base.GetReservations(status);
    }

    [Authorize]
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<List<CabinSearchResponse>> Search([FromQuery] CabinSearchRequest request)
    {
        decimal multiplier = 1;
        if (User.Identity.IsAuthenticated)
        {
            var user = await Context.Users
                .Include(u => u.Level)
                .FirstOrDefaultAsync(u => u.Id == User.Id());

            if (user is not null) 
            {
                multiplier = 1 - Convert.ToDecimal(user.Level?.DiscountPercentage ?? 0 / 100);
            }
        }

        int totalUnits = new Cabin().GetTotalUnits(request.Start, request.End);

        var query = Context.Cabins
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
            if (request.Rooms >= 4)
            {
                query = query.Where(c => c.Rooms.Count >= 4);
            }
            else
            {
                query = query.Where(c => c.Rooms.Count == request.Rooms);
            }
        }

        var results = await query
            .OrderBy(request.Direction)
            .Take(9)
            .Select(c => new CabinSearchResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Address = c.Address,
                Beds = c.Rooms.Sum(r => r.Beds),
                Image = c.Images.FirstOrDefault(),
                People = c.People,
                Rooms = c.Rooms.Count,
                Rating = c.Rating,
                Price = new Money
                {
                    Amount = c.PricePerUnit.Amount * request.People * multiplier,
                    Currency = c.PricePerUnit.Currency
                }
            }) 
            .ToListAsync();

        results.ForEach(r => r.Image = ImageUrl(r.Id, r.Image));

        return results;
    }

    [HttpGet("/cabin-owner/{id}/cabins")]
    [Authorize(Roles = Role.CabinOwner)]
    public async Task<List<CabinSearchResponse>> SearchOwnersCabins([FromQuery] CabinSearchRequest request)
    {
        var query = Context.Cabins
            .Where(c => c.Owner.Id == User.Id())
            .AsNoTrackingWithIdentityResolution();

        if (!String.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(a => EF.Functions.ILike(a.Name, $"%{request.Name}%"));
        }
        if (!String.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(a => EF.Functions.ILike(a.Address.City, $"%{request.City}%"));
        }
        if (!String.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(c => EF.Functions.ILike(c.Address.Country, $"%{request.Country}%"));
        }

        var results = await query
            .OrderBy(request.Direction)
            .Take(9)
            .Select(c => new CabinSearchResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Address = c.Address,
                Beds = c.Rooms.Sum(r => r.Beds),
                Image = c.Images.FirstOrDefault(),
                People = c.People,
                Rooms = c.Rooms.Count,
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

        return results;
    }
}