using API.Core.Model;

namespace API.Infrastructure.Data.Queries;

public static class BusinessQueries
{
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string criteria) where T : Business
    {
        return criteria switch
        {
            "price_asc" => query.OrderBy(b => b.PricePerUnit.Amount),
            "price_desc" => query.OrderByDescending(b => b.PricePerUnit.Amount),
            "rating_desc" => query.OrderByDescending(b => b.Rating),
            _ => query
        };
    }

    public static IQueryable<T> Available<T>(this IQueryable<T> query, DateTimeOffset start, DateTimeOffset end) where T : Business
    {
        return query
        .Where(b =>
            //b.Availability.Any(s => s.Start <= start && s.End >= end && s.Available) &&
            !b.Availability.Any(s =>
                (s.Start <= start && s.End >= end ||
                s.Start >= start && s.Start < end ||
                s.End > start && s.End <= end) &&
               !s.Available && !s.IsDeleted
            )
        )
        .Where(b =>
            !b.Reservations.Any(r =>
                (r.Start <= start && r.End >= end ||
                r.Start >= start && r.Start < end ||
                r.End > start && r.End <= end) &&
                !r.IsDeleted
            )
        );
    }
}