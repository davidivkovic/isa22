using API.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.Data.Queries;

public static class UserQueries
{
    public static async Task<decimal> GetDiscountMultiplier(this AppDbContext context, Guid userId)
    {
        if (userId != Guid.Empty)
        {
            var loyaltyLevel = await context.GetLoyaltyLevel(userId);
            return 1 - Convert.ToDecimal(loyaltyLevel?.DiscountPercentage ?? 0 / 100);
        }

        return 1;
    }

    public static async Task<Loyalty> GetLoyaltyLevel(this AppDbContext context, Guid userId)
    {
        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.Level)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is not null)
        {
            return await context.LoyaltyLevels
                .AsNoTracking()
                .OrderByDescending(l => l.Threshold)
                .FirstOrDefaultAsync(l => l.Threshold <= user.LoyaltyPoints);
        }

        return null;
    }
}