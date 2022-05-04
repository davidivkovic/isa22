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
                PhoneNumber = "+1-202-555-0125",
                EmailConfirmed = true,
                Roles = new() { Role.Admin },
                FirstName = "Admin",
                LastName = "Admin",
                Address = new()
                {
                    City = "New York",
                    Apartment = "96A",
                    Country = "United States",
                    PostalCode = "54382",
                    Street = "Brooklyn Avenue"
                }
            }, "admin");
        }
    }
}