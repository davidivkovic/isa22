using API.Core.Model;

namespace API.Infrastructure.Data.Queries;

public static class BusinessQueries
{
    public static IQueryable<T> Available<T>(this IQueryable<T> query, DateTime start, DateTime end) where T : Business
    {
        return query
        .Where(b =>
            b.Availability.Any(s => s.Start <= start && s.End >= end && s.Available) &&
            !b.Availability.Any(s => 
                s.Start <= start && s.End   >= end || 
                s.Start >= start && s.Start <  end ||
                s.End   >  start && s.End   <= end &&
               !s.Available
            )
        ).Where(b => 
            !b.Reservations.Any(r =>
                r.Start <= start && r.End   >= end ||
                r.Start >= start && r.Start <  end ||
                r.End   >  start && r.End   <= end
            )
        );
    }
}