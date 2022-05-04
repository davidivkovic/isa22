﻿namespace API.Controllers;

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

[ApiController]
[Route("boats")]
public class BoatController : BusinessController<Boat, BoatDTO, CreateBoatDTO, UpdateBoatDTO>
{
    protected override string BusinessType => "boat";

    public BoatController(AppDbContext dbContext) : base(dbContext) { }

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
    public async Task<List<BoatSearchResponse>> Search([FromQuery] BoatSearchRequest request)
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

        int totalUnits = new Boat().GetTotalUnits(request.Start, request.End);

        var query = Context.Boats
            .AsNoTrackingWithIdentityResolution()
            //.Available(request.Start, request.End)
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
                query = query.Where(b => b.Characteristics.Seats >= request.Seats);
            else
                query = query.Where(b => b.Characteristics.Seats == request.Seats);
        }

        var results = await query
            .OrderBy(request.Direction)
            .Take(9)
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
                    Amount = b.PricePerUnit.Amount * request.People * multiplier,
                    Currency = b.PricePerUnit.Currency
                }
            })
            .ToListAsync();

        results.ForEach(r => r.Image = ImageUrl(r.Id, r.Image));

        return results;

    }
        

    [HttpGet("/boat-owner/{id}/boats")]
    [Authorize(Roles = Role.BoatOwner)]
    [AllowAnonymous]
    public async Task<List<SearchResponse>> SearchOwnersCabins([FromQuery] SearchRequest request)
    {
        var query = Context.Boats
            .Where(b => b.Owner.Id == User.Id())
            .AsNoTrackingWithIdentityResolution();

        if (!String.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(b => EF.Functions.ILike(b.Name, $"%{request.Name}%"));
        };
        if (!String.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(b => EF.Functions.ILike(b.Address.City, $"%{request.City}%"));
        };
        if (!String.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(b => EF.Functions.ILike(b.Address.Country, $"%{request.Country}%"));
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

        return results;
    }
}