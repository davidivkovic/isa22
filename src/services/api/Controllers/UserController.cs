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

    [Authorize(Roles = "Admin")]
    [HttpGet("registrations/pending")]
    public async Task<List<PendingRegistrationDTO>> GetPendingRegistrations()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => !u.JoinRequest.Approved)
            .Where(u => !u.JoinRequest.Rejected)
            .Select(u => new PendingRegistrationDTO
            {
                User = u.Adapt<UserDTO>(),
                JoinRequest = u.JoinRequest
            })
            .ToListAsync();
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("registrations/{email}/accept")]
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
}