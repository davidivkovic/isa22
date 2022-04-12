namespace API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Mapster;

using API.DTO;
using API.Services;
using API.Core.Model;
using API.Infrastructure.Data;
using API.Infrastructure.Extensions;
using API.Infrastructure.Data.Queries;

[ApiController]
[Route("adventures")]
public class AdventureController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public AdventureController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private string ImageUrl(Guid id, string image)
    {
        return Url.Action(
            nameof(GetImage),
            ControllerContext.ActionDescriptor.ControllerName,
            new { id, image },
            Request.Scheme
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var adventureDTO = await _dbContext.Adventures
            .AsNoTracking()
            .Include(a => a.Services)
            .Include(a => a.Rules)
            .Where(a => a.Id == id)
            .ProjectToType<AdventureDT0>()
            .FirstOrDefaultAsync();

        if (adventureDTO is null)
        {
            return NotFound();
        }

        adventureDTO.WithImages(ImageUrl);

        return Ok(adventureDTO);
    }

    [HttpGet("{id}/images")]
    public async Task<ActionResult> GetImages(Guid id)
    {
        var adventure = await _dbContext.Adventures
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(a => a.Id == id);
        if(adventure is null)
        {
            return BadRequest("The requested adventure does not exist.");
        }

        return Ok(adventure.Images.Select(image => ImageUrl(id, image)));
    }

    [HttpGet("{id}/images/{image}")]
    public ActionResult GetImage([FromRoute] Guid id, [FromRoute] string image)
    {
        string imagePath = ImageService.GetPath(id, image);
        if (imagePath is null)
        {
            return BadRequest();
        }

        return PhysicalFile(imagePath, "image/*");
    }

    [HttpPost("{id}/images/add")]
    [Authorize(Roles = Role.Fisher)]
    public async Task<ActionResult> AddImage(Guid id, [FromForm] List<IFormFile> files)
    {
        var adventure = await _dbContext.Adventures
                                        .Include(a => a.Owner)
                                        .FirstOrDefaultAsync(a => a.Id == id);

        if (adventure.Owner.Id != User.Id())
        {
            return BadRequest("Cannot update someone else's adventure");
        }

        if (adventure is null)
        {
            return BadRequest("The requested adventure does not exist.");
        }

        if (files.Count > 10)
        {
            return BadRequest("A maximum of 10 images can be uploaded.");
        }

        foreach (var file in files)
        {
            if (!ImageService.IsValid(file.FileName, file.OpenReadStream()))
            {
                return BadRequest($"The file { file.FileName } is not a valid image.");
            }
        }

        var images = await Task.WhenAll(
            files.Select(image =>
                ImageService.Persist(adventure.Id, image.FileName, fs => image.CopyToAsync(fs))
            )
        );

        adventure.Images = new(images);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("create")]
    [Authorize(Roles = Role.Fisher)]
    public async Task<ActionResult> Create(CreateAdventureDTO dto)
    {
        User user = await _dbContext.Users.FindAsync(User.Id());
        if (user is null)
        {
            return BadRequest($"The user with email {user.Email} does not exist.");
        }

        var adventure = dto.Adapt<Adventure>();
        adventure.Id = Guid.NewGuid();
        adventure.Owner = user;

        _dbContext.Adventures.Add(adventure);
        await _dbContext.SaveChangesAsync();

        return Ok(adventure.Id);
    }

    [HttpPut("update")]
    [Authorize(Roles = Role.Fisher)]
    public async Task<ActionResult> Update(UpdateAdventureDTO dto)
    {
        var adventure = await _dbContext.Adventures
                                        .Include(a => a.Owner)
                                        .FirstOrDefaultAsync(a => a.Id == dto.Id);

        if (adventure.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        dto.Adapt(adventure);
        bool success = await _dbContext.SaveChangesAsync() > 0;

        if (!success)
        {
            BadRequest("Could not update your adventure at this time. Please try later again.");
        }

        return Ok(adventure.Adapt<AdventureDT0>());
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