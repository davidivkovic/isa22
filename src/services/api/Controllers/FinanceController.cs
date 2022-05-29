namespace API.Controllers;

using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using Mapster;

using API.DTO.Finance;
using API.Infrastructure.Data;
using API.Core.Model;
using API.Infrastructure.Extensions;

[ApiController]
[Route("finances")]
public class FinanceController : ControllerBase
{
    private readonly AppDbContext _context;

    public FinanceController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = Role.Admin)]
    [HttpGet("params")]
    public async Task<FinanceParamsDTO> GetFinanceParams()
    {
        var levels = await _context.LoyaltyLevels.ToListAsync();
        var finance = await _context.Finances.FirstOrDefaultAsync();

        return new()
        {
            Finance = finance.Adapt<FinanceDTO>(),
            LoyaltyLevels = levels
        };
    }


    [Authorize(Roles = Role.Admin)]
    [HttpPost("params/set")]
    public async Task<ActionResult> SetFinanceParams(FinanceParamsDTO parameters)
    {
        parameters.Finance.TaxPercentage = Math.Clamp(parameters.Finance.TaxPercentage, 0, 100);
        parameters.LoyaltyLevels.ForEach(l => 
            l.DiscountPercentage = Math.Clamp(l.DiscountPercentage, 0, 100)
        );

        var finance = await _context.Finances.FirstOrDefaultAsync();

        if (finance is null)
        {
            finance = new();
        }

        parameters.Finance.Adapt(finance);

        _context.LoyaltyLevels.RemoveRange(_context.LoyaltyLevels);
        _context.AddRange(parameters.LoyaltyLevels);
        _context.Update(finance);

        bool success = await _context.SaveChangesAsync() > 0;

        if (success) return Ok();

        return BadRequest();
    }

    [Authorize(Roles = Role.BusinessOwnerOrAdmin)]
    [HttpGet("report")]
    public ActionResult GetReport([FromQuery] DateTimeOffset startDate, [FromQuery] DateTimeOffset endDate, [FromQuery] string type)
    {
        var isAttendance = type == "attendance";
        var role = User.Role();
        var isAdmin = role == Role.Admin;
        var delta = (endDate - startDate).TotalDays;

        var query = _context.Reservations
            .AsNoTracking()
            .Where(r => r.Status != Reservation.ReservationStatus.Cancelled)
            .Select(r => r.Payment)
            .Where(p => p.Timestamp >= startDate)
            .Where(p => p.Timestamp <= endDate);

        if (delta > 365) return Ok(query
            .GroupBy(p => p.Timestamp.Year)
            .Select(g => new
            {
                Year = g.Key,
                Total = isAttendance ? g.Count() : g.Sum(isAdmin ?
                    p => p.Price.Amount * (decimal)(p.TaxPercentage / 100) :
                    p => p.Price.Amount * (decimal)(1 - p.TaxPercentage / 100)
                )
            })
        );

        else if (delta > 30) return Ok(query
            .GroupBy(p => new
            {
                p.Timestamp.Year,
                p.Timestamp.Month
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Total = isAttendance ? g.Count() : g.Sum(isAdmin ?
                    p => p.Price.Amount * (decimal)(p.TaxPercentage / 100) :
                    p => p.Price.Amount * (decimal)(1 - p.TaxPercentage / 100)
                )
            })
        );

        return Ok(query
            .GroupBy(p => new
            {
                Year = p.Timestamp.Year,
                Month = p.Timestamp.Month,
                Week = 1 + (p.Timestamp.DayOfYear - 1) / 7,
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.Week,
                Total = isAttendance ? g.Count() : g.Sum(isAdmin ?
                    p => p.Price.Amount * (decimal)(p.TaxPercentage / 100) :
                    p => p.Price.Amount * (decimal)(1 - p.TaxPercentage / 100)
                )
            })
        );
    }
}