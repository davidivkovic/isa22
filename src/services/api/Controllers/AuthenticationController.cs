namespace API.Controllers;

using System.Linq;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using Mapster;

using API.DTO;
using API.Security;
using API.Core.Model;
using API.Services.Email;
using API.DTO.Authentication;
using API.Infrastructure.Data;
using API.Services.Email.Messages;
using API.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

[Route("/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly Mailer _mailer;

    public AuthenticationController(
        AppDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        Mailer mailer
    )
    {
        _context = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _mailer = mailer;
    }

    //[Authorize]
    [HttpGet]
    public async Task<ActionResult> Test()
    {
        //var user = _context.Users.Find(User.Id());
        //_mailer.Send(user, new ConfirmEmail(user.FirstName, "784320"));

        var reservation = await _context.Reservations
            .Include(r => r.Business)
            .Include(r => r.User)
            .Where(r => r.Id == Guid.Parse("e8dc6d2a-60fb-4f05-bc20-76379a41c145"))
            .FirstOrDefaultAsync();

        var loyaltyLevel = await _context.LoyaltyLevels
            .AsNoTracking()
            .OrderBy(l => l.Threshold)
            .FirstOrDefaultAsync(l => l.Threshold <= reservation.User.LoyaltyPoints);

        reservation.Start = new DateTime(2022, 7, 7, 21, 0, 0, DateTimeKind.Utc);
        reservation.End = new DateTime(2022, 7, 8, 5, 0, 0, DateTimeKind.Utc);

        reservation.Services = new()
        {
            new()
            {
                Name = "Sauna",
                Price = new(12, "USD")
            },
            new()
            {
                Name = "Dinner",
                Price = new(6, "USD")
            },
        };

        reservation.Payment.Price = reservation.Business.Price(
            reservation.Start,
            reservation.End,
            reservation.Business.People,
            loyaltyLevel.DiscountPercentage,
            reservation.Services
        );

        await _context.SaveChangesAsync();

        return Ok();
    }

    [Authorize]
    [HttpPost("password/reset")]
    public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
    {
        User user = await _context.Users.FindAsync(User.Id());

        if (user is null)
        {
            return BadRequest("The email you entered doesn't belong to an account. Please check your email and try again.");
        }

        if (user.IsAdmin && user.LockedOut)
        {
            return BadRequest("Please first set your password as an administrator.");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: false
        );

        if (!signInResult.Succeeded)
        {
            return BadRequest("Sorry, your password was incorrect. Please double-check your password.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var refreshTokens = await _context.UserTokens.Where(t => t.UserId == user.Id).ToListAsync();
        refreshTokens.ForEach(t => t.Delete());

        IdentityResult result = await _userManager.ResetPasswordAsync(user, token, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest("Could not reset your password in at this time. Please try again later.");
        }

        Response.Cookies.DeleteRefreshToken();

        return Ok();
    }

    [HttpPost("password/set")]
    public async Task<ActionResult> SetPassword(SetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest("Your account has been deleted.");
        }

        if (!user.IsAdmin)
        {
            return StatusCode(403);
        }

        if (!user.LockedOut)
        {
            return BadRequest("You have already set your password.");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: false
        );

        if (!signInResult.Succeeded)
        {
            return BadRequest("Sorry, your password was incorrect. Please double-check your password.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
        {
            BadRequest("Could not set password in at this time. Please try again later.");
        }

        user.LockedOut = false;
        string accessToken = JWTProvider.GetAccessToken(user);
        RefreshToken refreshToken = new(user.Id);

        _context.UserTokens.Add(refreshToken);
        bool success = await _context.SaveChangesAsync() != 0;

        if (!success)
        {
            BadRequest("Could not sign in at this time. Please try again later.");
        }

        Response.Cookies.AppendRefreshToken(refreshToken.Value);

        return Ok(new SignInResponse 
        { 
            AccessToken = accessToken,
            User = user.Adapt<UserDTO>()
        });
    }

    [HttpPost("sign-out")]
    public new async Task<ActionResult> SignOut()
    {
        string token = Request.Cookies.RefreshToken();

        if (token is null)
        {
            return BadRequest($"No refresh-token cookie found.");
        }

        RefreshToken refreshToken = await _context.UserTokens.FindAsync(token.ToGuid());

        if (refreshToken is null)
        {
            return BadRequest("Invalid refresh token.");
        }

        if(User.Id() != refreshToken.UserId)
        {
            StatusCode(403, "Cannot revoke someone else's token.");
        }

        refreshToken.Delete();
        bool success = await _context.SaveChangesAsync() != 0;

        if(!success)
        {
            return BadRequest();
        }

        Response.Cookies.DeleteRefreshToken();
        return Ok();
    }

    [HttpGet("token-refresh")]
    public async Task<ActionResult<string>> TokenRefresh()
    {
        string token = Request.Cookies.RefreshToken();

        if (token is null)
        {
            return BadRequest($"No refresh-token cookie found.");
        }

        RefreshToken refreshToken = await _context.UserTokens.FindAsync(token.ToGuid());

        if (refreshToken is null)
        {
            return BadRequest("Refresh token invalid.");
        }

        User user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());

        if (user is null)
        {
            return BadRequest("Refresh token invalid");
        }

        string accessToken = JWTProvider.GetAccessToken(user);
        return Ok(new { accessToken });
    }
    
    [HttpPost("email/sign-in")]
    public async Task<ActionResult<SignInResponse>> EmailSignIn(EmailSignInRequest request)
    {
        User user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest("The email you entered doesn't belong to an account. Please check your email and try again.");
        }

        if (!user.EmailConfirmed)
        {
            if (user.JoinRequest.Rejected)
            {
                return BadRequest("Sorry, but your registration request was rejected. Please contact us for further information.");
            }
            else if (user.IsBusinessOwner)
            {
                return BadRequest("Your account was not yet approved by our administrators. Please try again later.");
            }
            else 
            {
                return BadRequest("The email you entered isn't confirmed. Please confirm your email and try again.");
            }
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: false
        );

        if (!signInResult.Succeeded)
        {
            return BadRequest("Sorry, your password was incorrect. Please double-check your password.");
        }

        if (user.LockedOut)
        {
            return Unauthorized("Please set your password before proceeding.");
        }

        string accessToken = JWTProvider.GetAccessToken(user);

        RefreshToken refreshToken = new(user.Id);

        _context.UserTokens.Add(refreshToken);
        bool success = await _context.SaveChangesAsync() != 0;

        if (!success)
        {
            BadRequest("Could not sign in at this time. Please try again later.");
        }

        Response.Cookies.AppendRefreshToken(refreshToken.Value);

        return Ok(new SignInResponse 
        { 
            AccessToken = accessToken,
            User = user.Adapt<UserDTO>()
        });
    }

    [AllowAnonymous]
    [Authorize(Roles = "Admin")]
    [HttpPost("email/sign-up")]
    public async Task<ActionResult<string>> EmailSignUp(EmailSignUpRequest request)
    {
        User user = await _userManager.FindByEmailAsync(request.Email);

        if (user is not null)
        {
            return BadRequest($"Another account is using {request.Email}.");
        }

        bool roleValid = request.Roles.Any() &&
                         request.Roles.All(r => Role.IsValid(r));

        if(!roleValid)
        {
            return BadRequest("The role you have chosen does not exist. Please change the role and try again.");
        }

        user = request.Adapt<User>();
        user.UserName = user.Email;
        user.Address = new();

        if(request.Roles.Contains(Role.Admin.ToString()))
        {
            if (User.Identity.IsAuthenticated && User.IsInRole(Role.Admin.ToString()))
            {
                user.EmailConfirmed = true;
                user.LockedOut = true;
            }
            else
            {
                return BadRequest("Only other administrators can sign up new adminstrators.");
            }
        }

        if (user.IsBusinessOwner)
        {
            user.JoinRequest = new(request.JoinReason);
        }

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest("We couldn't create your account at this time. Please try again later.");
        }

        user.Address = request.Address;
        await _context.SaveChangesAsync();

        if (user.Roles.Count == 1 && user.IsCustomer)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _mailer.Send(user, new ConfirmEmail(user.FirstName, token));
            return Ok();
        }

        return Ok();
    }

    [HttpPost("email/send-confirmation")]
    public async Task<ActionResult> SendConfirmationEmail([Required][EmailAddress] string email)
    {
        User user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return BadRequest("The email you entered doesn't belong to an account. Please check your email and try again.");
        }

        if (user.EmailConfirmed)
        {
            return BadRequest("The email you entered is already confirmed.");
        }

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        _mailer.Send(user, new ConfirmEmail(user.FirstName, token));

        return Ok();
    }

    [HttpPost("email/confirm")]
    public async Task<ActionResult> ConfirmEmail(EmailConfirmationRequest request)
    {
        User user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest("The email you entered doesn't belong to an account. Please check your email and try again.");
        }

        if (user.EmailConfirmed)
        {
            return BadRequest("The email you entered is already confirmed.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Code);

        if (result.Succeeded)
        {
            return Ok();
        }

        const int maximumNumberOfAttempts = 3;
        await _userManager.AccessFailedAsync(user);

        if (await _userManager.GetAccessFailedCountAsync(user) == maximumNumberOfAttempts)
        {
            await _userManager.DeleteAsync(user);
            return BadRequest("The maximum number of attempts was exceeded, please sign up again.");
        }

        return BadRequest("The code you entered is invalid or expired, please try again.");
    }
}