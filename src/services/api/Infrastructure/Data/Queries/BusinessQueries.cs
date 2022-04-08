using API.Core.Model;

namespace API.Infrastructure.Data.Queries;

public static class BusinessQueries
{
    public static IQueryable<T> Available<T>(this IQueryable<T> query, DateTime start, DateTime end) where T : Business
    {
        return query.Where(b =>
            b.Availability.Exists(s => s.Start <= start && s.End >= end && s.Available) &&
            !b.Availability.Exists(s => 
                s.Start <= start && s.End   >= end || 
                s.Start >= start && s.Start <= end ||
                s.End   >= start && s.End   <= end &&
               !s.Available
            )
        );
    }
}