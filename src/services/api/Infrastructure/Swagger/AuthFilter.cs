namespace API.Infrastructure.Swagger;

using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

public class AuthFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        IEnumerable<AuthorizeAttribute> authAttributes = context.MethodInfo
        .GetCustomAttributes(true)
        .OfType<AuthorizeAttribute>()
        .Distinct();

        if (authAttributes.Any())
        {
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            OpenApiSecurityScheme jwtBearerScheme = new()
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [ jwtBearerScheme ] = System.Array.Empty<string>()
                }
            };
        }
    }
}