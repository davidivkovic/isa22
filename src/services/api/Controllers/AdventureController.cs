namespace API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using API.DTO;
using API.Core.Model;
using API.DTO.Search;
using API.Services.Email;
using API.Infrastructure.Data;

[ApiController]
[Route("adventures")]
public class AdventureController : BusinessController<Adventure, AdventureDT0, CreateAdventureDTO, UpdateAdventureDTO, AdventureSearchResponse>
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

    [Authorize(Roles = Role.Fisher + "," + Role.Admin)]
    public override Task<ActionResult> Delete(Guid id)
    {
        return base.Delete(id);
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

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> DeleteSale([FromRoute] Guid id, [FromRoute] Guid saleId)
    {
        return base.DeleteSale(id, saleId);
    }

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> Report([FromRoute] Guid reservationId, CreateReportDTO request)
    {
        return base.Report(reservationId, request);
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

    [HttpGet]
    [Authorize(Roles = Role.Fisher + "," + Role.Admin)]
    public Task<ActionResult> GetBusinesses([FromQuery] AdventureSearchRequest request)
    {
        return base.GetBusinesses(request);
    }

    [Authorize(Roles = Role.Fisher)]
    public override Task<ActionResult> GetCalendar(Guid id, DateTimeOffset start, DateTimeOffset end)
    {
        return base.GetCalendar(id, start, end);
    }

    [Authorize]
    [AllowAnonymous]
    [HttpGet("search")]
    public Task<ActionResult> Search([FromQuery] AdventureSearchRequest request)
    {
        return Search(request);
    }
}