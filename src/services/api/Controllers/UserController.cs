namespace API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using Mapster;

using API.DTO;
using API.Services.Email;
using API.DTO.Authentication;
using API.Infrastructure.Data;
using API.Services.Email.Messages;
using API.Infrastructure.Extensions;
using API.Core.Model;

[Route("users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly Mailer _mailer;

    public UserController(AppDbContext context, Mailer mailer)
    {
        _context = context;
        _mailer = mailer;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (User.Id() != id && user.IsCustomer)
        {
            return StatusCode(403);
        }

        return Ok(user.Adapt<UserDTO>());
    }

    [Authorize]
    [HttpPost("update")]
    public async Task<ActionResult> Update(UpdateUserDTO request)
    {
        var user = await _context.Users.FindAsync(User.Id());

        if (!user.Roles.Contains(Role.Fisher))
        {
            request.Biography = string.Empty;
        }

        request.Adapt(user);

        _context.Update(user);
        bool success = await _context.SaveChangesAsync() > 0;

        if (!success)
        {
            return BadRequest("Could not update your information at this time. Please try again later");
        }

        return Ok();
    }

    [Authorize]
    [HttpPost("delete")]
    public async Task<ActionResult> Delete([FromBody] string reason)
    {
        var user = await _context.Users.FindAsync(User.Id());

        if (user is null || user.IsDeleted || (user.DeletionRequest?.Approved ?? false))
        {
            return BadRequest("Your account was already deleted.");
        }

        if (!user.DeletionRequest?.Rejected ?? false)
        {
            return BadRequest("You have already requested to delete your account. Please wait for our adiminstrators to reply via email.");
        }

        user.DeletionRequest = new(reason);

        bool success = await _context.SaveChangesAsync() > 0;

        if (!success)
        {
            return BadRequest("Could not delete your account at this time, please try again later.");
        }

        return Ok();
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult> GetProfile()
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == User.Id())
            .FirstOrDefaultAsync();
        
        if (user is null)
        {
            return BadRequest("User does not exist.");
        }

        var loyaltyQuery = _context.LoyaltyLevels
            .AsNoTracking();
        
        var loyaltyLevel = await loyaltyQuery
            .OrderByDescending(l => l.Threshold)
            .FirstOrDefaultAsync(l => l.Threshold <= user.LoyaltyPoints);
        var nextLoyaltyLevel = await loyaltyQuery
            .OrderBy(l => l.Threshold)
            .FirstOrDefaultAsync(l => l.Threshold > user.LoyaltyPoints);

        return Ok(new UserProfileDTO
        {
            User = user.Adapt<UserDTO>(),
            DeletionRequest = user.DeletionRequest,
            LoyaltyLevel = new()
            {
                Points = user.LoyaltyPoints,
                Current = loyaltyLevel,
                Next = nextLoyaltyLevel
            }
        });
    }

    [Authorize(Roles = Role.Admin)]
    [HttpGet("registrations/pending")]
    public async Task<List<PendingRequestDTO>> GetPendingRegistrations([FromQuery] DateTimeOffset before)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => !u.JoinRequest.Approved)
            .Where(u => !u.JoinRequest.Rejected)
            .Where(u => u.JoinRequest.Timestamp <= before)
            .OrderByDescending(u => u.JoinRequest.Timestamp)
            .Take(10)
            .Select(u => new PendingRequestDTO
            {
                User = u.Adapt<UserDTO>(),
                Request = u.JoinRequest
            })
            .ToListAsync();
    }

    [Authorize(Roles = Role.Admin)]
    [HttpPost("registrations/{email}/update")]
    public async Task<ActionResult> AcceptRegistration([FromRoute] string email, [FromBody] string reason)
    {
        var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

        if (user is null)
        {
            return BadRequest("The email you entered doesn't belong to an account. Please check the email and try again.");
        }

        if (user.EmailConfirmed)
        {
            return BadRequest("The email you entered is already confirmed.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            user.JoinRequest.Approve();
            user.EmailConfirmed = true;
        }
        else
        {
            user.JoinRequest.Deny(reason);
        }

        bool success = await _context.SaveChangesAsync() > 0;

        if (!success)
        {
            BadRequest("Could not accept the registration at this time. Please try again later.");
        }

        if (user.JoinRequest.Approved)
        {
            _mailer.Send(user, new RegistrationApproved(user.FirstName, "ctaUrl"));
        }
        else if (user.JoinRequest.Rejected)
        {
            _mailer.Send(user, new RegistrationDeclined(
                user.FirstName,
                reason,
                "#contactUrl"
            ));
        }

        return Ok();
    }


    [Authorize(Roles = Role.Admin)]
    [HttpGet("delete-requests/pending")]
    public async Task<List<PendingRequestDTO>> GetDeleteRequests([FromQuery] DateTimeOffset before)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => !u.DeletionRequest.Approved)
            .Where(u => !u.DeletionRequest.Rejected)
            .Where(u => u.DeletionRequest.Timestamp <= before)
            .OrderByDescending(u => u.DeletionRequest.Timestamp)
            .Take(10)
            .Select(u => new PendingRequestDTO
            {
                User = u.Adapt<UserDTO>(),
                Request = u.DeletionRequest
            })
            .ToListAsync();
    }

    [Authorize(Roles = Role.Admin)]
    [HttpPost("delete-requests/{email}/update")]
    public async Task<ActionResult> AcceptDeleteRequest([FromRoute] string email, [FromBody] string reason)
    {
        var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

        if (user is null)
        {
            return BadRequest("The email you entered doesn't belong to an account. Please check the email and try again.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            user.DeletionRequest.Approve();
            user.Delete();
        }
        else
        {
            user.DeletionRequest.Deny(reason);
        }

        bool success = await _context.SaveChangesAsync() > 0;

        if (!success)
        {
            BadRequest("Could not delete your account at this time. Please try again later.");
        }

        if (user.DeletionRequest.Approved)
        {
            _mailer.Send(user, new AccountDeletionApproved(user.FirstName, "ctaUrl"));
        }
        else if (user.DeletionRequest.Rejected)
        {
            _mailer.Send(user, new AccountDeletionDeclined(
                user.FirstName,
                reason,
                "#contactUrl"
            ));
        }

        return Ok();
    }

    [Authorize(Roles = Role.Admin)]
    [HttpPost("reviews/{id}/update")]
    public async Task<ActionResult> ApproveReview([FromRoute] Guid id, [FromRoute] bool approve)
    {
        var review = await _context.Reviews
            .Include(r => r.Business)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review is null)
        {
            return BadRequest();
        }

        if (approve)
        {
            review.Approve();
        }
        else
        {
            review.Deny();
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Roles = Role.Admin)]
    [HttpGet("reviews/pending")]
    public async Task<List<PendingReviewDTO>> GetPendingReviews([FromQuery] DateTimeOffset before)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => !r.Approved)
            .Where(r => !r.Rejected)
            .Where(r => r.Timestamp <= before)
            .OrderByDescending(r => r.Timestamp)
            .Take(10)
            .ProjectToType<PendingReviewDTO>()
            .ToListAsync();
    }
}