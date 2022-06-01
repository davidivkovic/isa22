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
[Route("boats")]
public class BoatController : BusinessController<Boat, BoatDTO, CreateBoatDTO, UpdateBoatDTO, BoatSearchResponse>
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

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<ActionResult> DeleteSale([FromRoute] Guid id, [FromRoute] Guid saleId)
    {
        return base.DeleteSale(id, saleId);
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<ActionResult> CreateUnavailability(Guid id, DateTimeOffset start, DateTimeOffset end)
    {
        return base.CreateUnavailability(id, start, end);
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<ActionResult> DeleteUnavailability(Guid id, Guid eventId)
    {
        return base.DeleteUnavailability(id, eventId);
    }

    [HttpGet]
    [Authorize(Roles = Role.BoatOwner)]
    public Task<ActionResult> GetBusinesses([FromQuery] BoatSearchRequest request)
    {
        return base.GetBusinesses(request);
    }

    [Authorize(Roles = Role.BoatOwner)]
    public override Task<ActionResult> GetCalendar(Guid id, DateTimeOffset start, DateTimeOffset end)
    {
        return base.GetCalendar(id, start, end);
    }

    [Authorize]
    [AllowAnonymous]
    [HttpGet("search")]
    public Task<ActionResult> Search([FromQuery] BoatSearchRequest request)
    {
        return Search(request, query =>
        {
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
            return query;
        });
    }
}