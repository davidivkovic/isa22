namespace API.Infrastructure.Data;

using Microsoft.AspNetCore.Identity;

using API.Core.Model;

public class DbSeed
{
    public static async Task SeedUsers(UserManager<User> userManager)
    {
        var user = await userManager.FindByEmailAsync("admin@api.com");
        if (user is null)
        {
            await userManager.CreateAsync(new()
            {
                Email = "admin@api.com",
                UserName = "admin@api.com",
                EmailConfirmed = true,
                Roles = new() { Role.Admin },
                FirstName = "Admin",
                LastName = "Admin",
                Address = new()
            }, "admin");
        }
    }
}