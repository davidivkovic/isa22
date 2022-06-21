namespace API.Infrastructure.Data;

using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using API.Core.Model;

public static class DbContextExtensions
{
    public static void EnableDeleteFilter(this ModelBuilder modelBuilder)
    {
        Expression<Func<IDeletable, bool>> filter = e => !e.IsDeleted;

        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            if (mutableEntityType.ClrType != null && 
                mutableEntityType.ClrType.IsAssignableTo(typeof(IDeletable)) &&
                mutableEntityType.BaseType == null
            )
            {
                var parameter = Expression.Parameter(mutableEntityType.ClrType);
                var body = ReplacingExpressionVisitor.Replace(
                    filter.Parameters.First(),
                    parameter, filter.Body
                );
                var lambdaExpression = Expression.Lambda(body, parameter);
                mutableEntityType.SetQueryFilter(lambdaExpression);
            }
        }
    }
}