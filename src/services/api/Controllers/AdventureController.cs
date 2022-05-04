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