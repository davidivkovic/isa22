namespace API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using API.DTO.Finance;
using API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Mapster;

[ApiController]
[Route("finances")]
public class FinanceController : ControllerBase
{
    private readonly AppDbContext _context;

    public FinanceController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("params")]
    public async Task<FinanceParamsDTO> GetFinanceParams()
    {
        var levels = await _context.LoyaltyLevels.ToListAsync();
        var finance = await _context.Finances.FirstOrDefaultAsync();

        if (finance is null)
        {
            finance = new()
            {
                TaxPercentage = 20,
                CustomerPoints = 5,
                BusinessOwnerPoints = 2
            };
            _context.Add(finance);
            await _context.SaveChangesAsync();
        }

        return new()
        {
            Finance = finance.Adapt<FinanceDTO>(),
            LoyaltyLevels = levels
        };
    }


    [Authorize(Roles = "Admin")]
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
}