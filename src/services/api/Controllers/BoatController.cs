﻿namespace API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using Mapster;

using API.DTO;
using API.Services;
using API.Core.Model;
using API.Infrastructure.Data;
using API.Infrastructure.Data.Queries;
using API.Infrastructure.Extensions;
using API.DTO.Search;

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

        if (files.Count > 10)
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
                ImageService.Persist(boat.Id, image.FileName, fs => image.CopyToAsync(fs)
            )
        ));

        boat.Images = new(images);
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

    [HttpPost("update")]
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

        return Ok(boat.Id);
    }

    [Authorize]
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<List<BoatSearchResponse>> Search([FromQuery] BoatSearchRequest request)
    {
        decimal multiplier = 1;
        if (User.Identity.IsAuthenticated)
        {
            var user = await _dbContext.Users
                .Include(u => u.Level)
                .FirstOrDefaultAsync(u => u.Id == User.Id());

            if (user is not null)
            {
                multiplier = 1 - Convert.ToDecimal(user.Level?.DiscountPercentage ?? 0 / 100);
            }
        }

        int totalUnits = new Boat().GetTotalUnits(request.Start, request.End);

        var query = _dbContext.Boats
            .AsNoTrackingWithIdentityResolution()
            //.Available(request.Start, request.End)
            .Where(c => c.Address.City == request.City)
            .Where(c => c.Address.Country == request.Country)
            .Where(c => c.People == request.People);

        if (request.RatingHigher != default)
        {
            query = query.Where(b => b.Rating >= request.RatingHigher);
        }
        if (request.PriceLow != default)
        {
            query = query.Where(b => b.PricePerUnit.Amount >= request.PriceLow);
        }
        if (request.PriceHigh != default)
        {
            query = query.Where(b => b.PricePerUnit.Amount <= request.PriceHigh);
        }
        if (request.Seats != default)
        {
            if (request.Seats >= 5)
                query = query.Where(b => b.Characteristics.Seats >= request.Seats);
            else
                query = query.Where(b => b.Characteristics.Seats == request.Seats);
        }

        var results = await query
            .OrderBy(request.Direction)
            .Take(9)
            .Select(b => new BoatSearchResponse
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Image = b.Images.FirstOrDefault(),
                People = b.People,
                Rating = b.Rating,
                BoatCharacteristics = b.Characteristics,
                Price = new Money
                {
                    Amount = b.PricePerUnit.Amount * request.People * multiplier,
                    Currency = b.PricePerUnit.Currency
                }
            })
            .ToListAsync();

        results.ForEach(r => r.Image = ImageUrl(r.Id, r.Image));

        return results;

    }
        

    [HttpGet("/boat-owner/{id}/boats")]
    //  [Authorize(Roles = Role.BoatOwner)]
    [Authorize]
    [AllowAnonymous]
    public async Task<List<SearchResponse>> SearchOwnersCabins([FromQuery] SearchRequest request)
    {
        var query = _dbContext.Boats
            .AsNoTrackingWithIdentityResolution();

        if (!String.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(a => a.Name.ToLower().Contains(request.Name.ToLower()));
        };
        if (!String.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(a => a.Address.City.ToLower().Contains(request.City.ToLower()));
        };
        if (!String.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(c => c.Address.Country.ToLower().Contains(request.Country.ToLower()));
        }

        var results = await query
            .OrderBy(request.Direction)
            .Take(9)
            .Select(c => new SearchResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Address = c.Address,
                Image = c.Images.FirstOrDefault(),
                People = c.People,
                Rating = c.Rating,
                CancellationFee = c.CancellationFee,
                Price = new Money
                {
                    Amount = c.PricePerUnit.Amount,
                    Currency = c.PricePerUnit.Currency
                }
            })
            .ToListAsync();

        results.ForEach(r => r.Image = ImageUrl(r.Id, r.Image));

        return results;
    }
}