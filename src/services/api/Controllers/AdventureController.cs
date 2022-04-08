namespace API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Mapster;

using API.Core.Model;
using API.DTO;
using API.Infrastructure.Data;
using API.Infrastructure.Extensions;

[ApiController]
[Route("adventures")]
public class AdventureController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public AdventureController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{id}")]
    public async Task<AdventureDT0> Get(Guid id)
    {
        return await _dbContext.Adventures
            .Where(a => a.Id == id)
            .ProjectToType<AdventureDT0>()
            .FirstOrDefaultAsync();
    }


    [HttpPost("create")]
    [Authorize(Roles = "Fisher")]
    public async Task<IActionResult> Create(AdventureCreateDTO dto)
    {
        var user = await _dbContext.Users.FindAsync(User.Id());
        if (user == null)
        {
            BadRequest("User does not exist");
        }

        var adventure = dto.Adapt<Adventure>();
        adventure.Owner = user;

        _dbContext.Adventures.Add(adventure);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public List<Adventure> GetCabins()
    {

        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow + TimeSpan.FromDays(2);

        return _dbContext.Reservations
            .Include(r => r.Business)
            .Where(r => r.Business.Address.City == "Belgrade")
            .Where(r => !(start >= r.Start && end < r.End))
            .Select(r => r.Business)
            .Cast<Adventure>()
            .ToList();
    }
}