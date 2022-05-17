using System.Text.Json.Serialization;

using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Mapster;

using FastExpressionCompiler;

using API.Core.Model;
using API.Infrastructure.Data;
using API.Security;
using API.Infrastructure.Swagger;
using API.Services.Email;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o => 
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });

    options.OperationFilter<AuthFilter>();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(o =>
{
    string connection = 
    Environment.GetEnvironmentVariable("POSTGRES_CONNECTION") ??
    builder.Configuration.GetConnectionString("Postgres");

    o.UseNpgsql(connection);
});

builder.Services
.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    // Adjust for deployment
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Tokens.EmailConfirmationTokenProvider = nameof(TokenProvider);
    options.Tokens.PasswordResetTokenProvider = nameof(TokenProvider);
})
.AddTokenProvider<TokenProvider>(nameof(TokenProvider))
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services
.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o => 
{
    o.TokenValidationParameters = new()
    {
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromSeconds(0),
        ValidAudience = JWTProvider.Audience,
        ValidIssuer = JWTProvider.Issuer,
        IssuerSigningKey = JWTProvider.Key
    };
});

builder.Services.AddSingleton<Mailer>();

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();

var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

await context.Database.MigrateAsync();
await DbSeed.SeedUsers(userManager);
await DbSeed.SeedFinance(context);

await scope.DisposeAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(c => c.AllowAnyMethod()
                      .AllowAnyHeader()
                      .SetIsOriginAllowed(origin => true)
                      .AllowCredentials());
}

TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileFast();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();