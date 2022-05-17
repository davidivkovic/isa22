namespace API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using API.DTO;
using API.Infrastructure.Data;
using API.Infrastructure.Extensions;
using API.Infrastructure.Data.Queries;
using API.Core.Model;
using Microsoft.EntityFrameworkCore;
using API.DTO.Search;

[ApiController]
[Route("adventures")]
public class AdventureController : BusinessController<Adventure, AdventureDT0, CreateAdventureDTO, UpdateAdventureDTO>
{
    protected override string BusinessType => "adventure";

    public AdventureController(AppDbContext dbContext) : base(dbContext) { }

    [Authorize(Roles = Role.Fisher)]
    public override Task<IActionResult> Create(CreateAdventureDTO dto)
    {
        return base.Create(dto);
    }

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> Update(UpdateAdventureDTO dto)
    {
        return base.Update(dto);
    }

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> AddImage(Guid id, [FromForm] List<IFormFile> files)
    {
        return base.AddImage(id, files);
    }

    [Authorize]
    [HttpGet("reservations")]
    public Task<ActionResult> GetReservations(string status)
    {
        return GetReservations("adventures", status);
    }


    [Authorize]
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ActionResult> Search([FromQuery] AdventureSearchRequest request)
    {
        int totalUnits = new Adventure().GetTotalUnits(request.Start, request.End);
        decimal discountMultiplier = await Context.GetDiscountMultiplier(User.Id());

        var query = Context.Adventures
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

        int totalResults = await query.CountAsync();
        var results = await query
            .OrderBy(request.Direction)
            .Take(6)
            .Select(a => new AdventureSearchResponse
            {
                Id = a.Id,
                Name = a.Name,
                Address = a.Address,
                Image = a.Images.FirstOrDefault(),
                People = a.People,
                Rating = a.Rating,
                Price = new Money
                {
                    Amount = a.PricePerUnit.Amount,
                    Currency = a.PricePerUnit.Currency
                },
                FishingEquipment = a.FishingEquipment
            })
            .ToListAsync();

        results.ForEach(r => r.Image = ImageUrl(r.Id, r.Image));

        return Ok(new
        {
            results,
            totalResults,
        });
    }

    //[HttpGet]
    //public async Task<ActionResult> GetAdventures()
    //{

    //    var start = DateTime.UtcNow;
    //    var end = DateTime.UtcNow + TimeSpan.FromDays(2);

    //    var adventures = await _dbContext.Adventures
    //        .Include(c => c.Availability)
    //        .Include(c => c.Reservations)
    //        .Where(c => c.Address.City == "Belgrade")
    //        .Available(start, end)
    //        .ToListAsync();

    //    return Ok(adventures);
    //}
}