namespace API.Security;

using Microsoft.AspNetCore.Identity;

using System.Globalization;
using System.Security.Cryptography;

using API.Core.Model;
using API.Infrastructure.Data;

public class TokenProvider : IUserTwoFactorTokenProvider<User>
{
    private readonly AppDbContext _context;

    public TokenProvider(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(
        UserManager<User> manager,
        User user
    )
    {
        return Task.FromResult(true);
    }

    public async Task<string> GenerateAsync(
        string purpose,
        UserManager<User> manager,
        User user
    )
    {
        await manager.UpdateSecurityStampAsync(user);
        string token = RandomNumberGenerator.GetInt32(1000000)
                                            .ToString("D6", CultureInfo.InvariantCulture);
        user.OneTimePassword = new()
        {
            Value = token,
            Expires = DateTimeOffset.UtcNow + TimeSpan.FromMinutes(30)
        };
        await _context.SaveChangesAsync();
        return token;
    }

    public Task<bool> ValidateAsync(
        string purpose,
        string token,
        UserManager<User> manager,
        User user
    )
    {
        return Task.FromResult(user.OneTimePassword.Validate(token));
    }
}