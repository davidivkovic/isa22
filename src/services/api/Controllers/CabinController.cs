﻿namespace API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Mapster;

using API.DTO;
using API.Services;
using API.Core.Model;
using API.Infrastructure.Data;
using API.Infrastructure.Extensions;

[ApiController]
[Route("cabins")]
public class CabinController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public CabinController(AppDbContext dbContext)
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
        var cabin = await _dbContext.Cabins
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(a => a.Id == id);
        if (cabin is null)
        {
            return BadRequest("The requested adventure does not exist.");
        }

        return Ok(cabin.Images.Select(image => ImageUrl(id, image)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var cabinDTO = await _dbContext.Cabins
            .AsNoTracking()
            .Include(a => a.Services)
            .Include(a => a.Rules)
            .Where(a => a.Id == id)
            .ProjectToType<CabinDTO>()
            .FirstOrDefaultAsync();

        if (cabinDTO is null)
        {
            return NotFound();
        }

        cabinDTO.WithImages(ImageUrl);

        return Ok(cabinDTO);
    }

    [HttpPost("{id}/images/add")]
    [Authorize(Roles = Role.CabinOwner)]
    public async Task<ActionResult> AddImage(Guid id, [FromForm] List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            return BadRequest("No images to add!");
        }

        var cabin = await _dbContext.Cabins
                            .Include(a => a.Owner)
                            .FirstOrDefaultAsync(a => a.Id == id);

        if (cabin.Owner.Id != User.Id())
        {
            return BadRequest("Cannot update someone else's cabin.");
        }

        if (cabin is null)
        {
            return BadRequest("The requested cabin does not exist.");
        }

        if (files.Count + cabin.Images.Count > 10)
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
            ImageService.Persist(cabin.Id, image.FileName, fs => image.CopyToAsync(fs))
            )
        );

        List<string> imgs = new(images);

        cabin.Images.AddRange(imgs);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("create")]
    [Authorize(Roles = Role.CabinOwner)]
    public async Task<IActionResult> Create(CreateCabinDTO dto)
    {
        User user = await _dbContext.Users.FindAsync(User.Id());
        if (user is null)
        {
            return BadRequest($"The user with email {user.Email} does not exist.");
        }

        var cabin = dto.Adapt<Cabin>();
        cabin.Id = Guid.NewGuid();
        cabin.Owner = user;

        _dbContext.Cabins.Add(cabin);
        await _dbContext.SaveChangesAsync();

        return Ok(cabin.Id);
    }

    [HttpGet("update")]
    [Authorize(Roles = Role.CabinOwner)]
    public async Task<ActionResult> Update(UpdateCabinDTO dto)
    {
        var cabin = await _dbContext.Cabins
                            .Include(a => a.Owner)
                            .FirstOrDefaultAsync(a => a.Id == dto.Id);

        if (cabin.Owner.Id != User.Id())
        {
            return StatusCode(403);
        }

        dto.Adapt(cabin);
        bool success = await _dbContext.SaveChangesAsync() > 0;

        if (!success)
        {
            BadRequest("Could not update your cabin at this time. Please try again later.");
        }

        return Ok(cabin.Adapt<CabinDTO>());
    }
}