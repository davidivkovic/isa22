namespace API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using Mapster;

using API.DTO;
using API.Services;
using API.Core.Model;
using API.Infrastructure.Data;
using API.Infrastructure.Extensions;

[ApiController]
[Route("boats")]
public class BoatController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public BoatController(AppDbContext dbContext)
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

    [HttpGet("{id}/images")]
    public async Task<ActionResult> GetImages(Guid id)
    {
        var boat = await _dbContext.Boats
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(a => a.Id == id);
        if (boat is null)
        {
            return BadRequest("The requested adventure does not exist.");
        }

        return Ok(boat.Images.Select(image => ImageUrl(id, image)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var boatDTO = await _dbContext.Boats
            .AsNoTracking()
            .Include(a => a.Services)
            .Include(a => a.Rules)
            .Where(a => a.Id == id)
            .ProjectToType<BoatDTO>()
            .FirstOrDefaultAsync();

        if (boatDTO is null)
        {
            return NotFound();
        }

        boatDTO.WithImages(ImageUrl);

        return Ok(boatDTO);
    }

    [HttpPost("{id}/images/add")]
    [Authorize(Roles = Role.BoatOwner)]
    public async Task<ActionResult> AddImage(Guid id, [FromForm] List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            return BadRequest("No images to add!");
        }

        var boat = await _dbContext.Boats
                            .Include(a => a.Owner)
                            .FirstOrDefaultAsync(a => a.Id == id);

        if (boat.Owner.Id != User.Id())
        {
            return BadRequest("Cannot update someone else's boat.");
        }

        if (boat is null)
        {
            return BadRequest("The requested boat does not exist.");
        }

        if (files.Count + boat.Images.Count > 10)
        {
            return BadRequest("A maximum of 10 images can be uploaded.");
        }

        foreach (var file in files)
        {
            if (!ImageService.IsValid(file.FileName, file.OpenReadStream()))
            {
                return BadRequest($"The file {file.FileName} is not a valid image.");
            }
        }

        var images = await Task.WhenAll(
            files.Select(image =>
            ImageService.Persist(boat.Id, image.FileName, fs => image.CopyToAsync(fs))
            )
        );

        List<string> imgs = new(images);

        boat.Images.AddRange(imgs);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("create")]
    [Authorize(Roles = Role.BoatOwner)]
    public async Task<IActionResult> Create(CreateBoatDTO dto)
    {
        User user = await _dbContext.Users.FindAsync(User.Id());
        if(user is null)
        {
            return BadRequest($"The user with email {user.Email} does not exist.");
        }

        var boat = dto.Adapt<Boat>();
        boat.Id = Guid.NewGuid();
        boat.Owner = user;

        _dbContext.Boats.Add(boat);
        await _dbContext.SaveChangesAsync();

        return Ok(boat.Id);
    }

    [HttpGet("update")]
    [Authorize(Roles = Role.BoatOwner)]
    public async Task<ActionResult> Update(UpdateBoatDTO dto)
    {
        var boat = await _dbContext.Boats
                            .Include(a => a.Owner)
                            .FirstOrDefaultAsync(a => a.Id == dto.Id);

        if (boat.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        dto.Adapt(boat);
        bool success = await _dbContext.SaveChangesAsync() > 0;

        if (!success)
        {
            BadRequest("Could not update your boat at this time. Please try again later.");
        }

        return Ok(boat.Adapt<BoatDTO>());
    }
}