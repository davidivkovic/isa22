using api.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace api.Infrastructure;

public static class DbContextExtensions
{
    public static void EnableDeleteFilter(this ModelBuilder modelBuilder)
    {
        Expression<Func<Entity, bool>> filter = e => !e.IsDeleted;

        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            if (mutableEntityType.ClrType != null && 
                mutableEntityType.ClrType.IsAssignableTo(typeof(Entity)) &&
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