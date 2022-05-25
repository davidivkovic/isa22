﻿namespace API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

using API.DTO;
using API.Infrastructure.Data;
using API.Infrastructure.Extensions;
using API.Infrastructure.Data.Queries;
using API.Core.Model;
using API.DTO.Search;
using API.Services.Email;

using Mapster;

[ApiController]
[Route("adventures")]
public class AdventureController : BusinessController<Adventure, AdventureDT0, CreateAdventureDTO, UpdateAdventureDTO>
{
    protected override string BusinessType => "adventure";

    public AdventureController(AppDbContext dbContext, Mailer mailer) : base(dbContext, mailer) { }

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

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> PreviewCreateSale([FromRoute] Guid id, [FromBody] CreateSaleDTO request)
    {
        return base.PreviewCreateSale(id, request);
    }

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> CreateSale([FromRoute] Guid id, [FromBody] CreateSaleDTO request)
    {
        return base.CreateSale(id, request);
    }
        
    [HttpGet]
    [Authorize(Roles = Role.Fisher)]
    public async Task<ActionResult> Get()
    {
        var query = Context.Adventures
            .Where(a => a.Owner.Id == User.Id())
            .AsNoTrackingWithIdentityResolution();
        var results = await query
            .Take(3)
            .Select(a =>  a.Adapt<AdventureDT0>())
            .ToListAsync();

        results.ForEach(a => a.WithImages(ImageUrl));
        return Ok(results);
    }

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> DeleteSale([FromRoute] Guid id, [FromRoute] Guid saleId)
    {
        return base.DeleteSale(id, saleId);
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
            .Available(request.Start.UtcDateTime, request.End.UtcDateTime)
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
            .Skip(request.Page * 6)
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

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> GetCalendar(Guid id, DateTimeOffset start, DateTimeOffset end)
    {
        return base.GetCalendar(id, start, end);
    }

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> CreateUnavailability(Guid id, DateTimeOffset start, DateTimeOffset end)
    {
        return base.CreateUnavailability(id, start, end);
    }

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> DeleteUnavailability(Guid id, Guid eventId)
    {
        return base.DeleteUnavailability(id, eventId);
    }
}