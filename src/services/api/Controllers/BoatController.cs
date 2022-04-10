namespace API.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.DTO;
using API.Infrastructure.Data;
using API.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using API.Core.Model;
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
            Request.Scheme);
    }

    [HttpGet("{id}/images/{image}")]
    private ActionResult GetImage([FromRoute] Guid id, [FromRoute] string image)
    {
        string imagePath = ImageService.GetPath(id, image);
        if (image is null)
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

    [HttpPost("create")]
    [Authorize(Roles = "BoatOwner")]
    public async Task<ActionResult> Create([FromForm] CreateBoatDTO dto)
    {
        User user = await _dbContext.Users.FindAsync(User.Id());
        if (user is null)
        {
            return BadRequest($"The user with email {user.Email} does not exist.");
        }

        dto.ImageData ??= new();

        if (dto.ImageData.Count > 10)
        {
            return BadRequest("A maximum of 10 iamges can be uploaded.");
        }

        foreach (var image in dto.ImageData)
        {
            if (!ImageService.IsValid(image.FileName, image.OpenReadStream()))
            {
                return BadRequest($"The file {image.FileName} is not a valid image file.");
            }
        }

        var boat = dto.Adapt<Boat>();
        boat.Id = Guid.NewGuid();
        boat.Owner = user;

        var images = await Task.WhenAll(
            dto.ImageData.Select(image =>
                ImageService.Persist(boat.Id, image.FileName, fs => image.CopyToAsync(fs))
            )
        );

        boat.Images = new(images);
        _dbContext.Boats.Add(boat);
        await _dbContext.SaveChangesAsync();

        return Ok(boat.Id);
    }

    [HttpGet("update")]
    [Authorize(Roles = "BoatOwner")]
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
            BadRequest("Could not update your adventure at this time. Please try again later.");
        }

        return Ok(boat.Adapt<BoatDTO>());
    }
}
