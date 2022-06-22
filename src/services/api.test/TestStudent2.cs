using API.Controllers;
using API.Core.Model;
using API.DTO;
using API.DTO.Authentication;
using API.DTO.Search;
using API.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web;

namespace api.test;

[TestClass]
public class TestStudent2
{
    private static HttpClient _httpClient;
    private static CustomWebApplicationFactory<Program> _app;

    public TestStudent2()
    {
        _httpClient = _app.CreateDefaultClient();
    }

    [ClassInitialize]
    public static void TestFixtureSetup(TestContext c)
    {
        _app = new CustomWebApplicationFactory<Program>();
    }

    private async Task<JsonElement> SignIn()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        using var userManager = scope.ServiceProvider.GetService<UserManager<User>>();

        var user = await userManager.FindByEmailAsync("boatowner@api.com");
        if(user is null)
        {
            var newUser = await userManager.CreateAsync(new User()
            {
                UserName = "boatowner@api.com",
                Email = "boatowner@api.com",
                EmailConfirmed = true,
                FirstName = "Miladin",
                LastName = "Momcilovic",
                Address = new()
                {
                    City = "New York",
                    Apartment = "96A",
                    Country = "United States",
                    PostalCode = "54382",
                    Street = "Brooklyn Avenue"
                },
                Roles = new() { Role.BoatOwner }
            }, "password");

            context.Add(new Boat()
            {
                Owner = await userManager.FindByEmailAsync("boatowner@api.com"),
                Name = "Brod na Dunavu",
                PricePerUnit = new()
                {
                    Amount = 15,
                    Currency = "USD"
                },
                Address = new()
                {
                    City = "Novi Sad",
                    Country = "Serbia"
                },
                People = 2
            });
            context.SaveChanges();
        }

        EmailSignInRequest credentials = new()
        {
            Email = "boatowner@api.com",
            Password = "password"
        };

        var content = JsonSerializer.Serialize(credentials);
        var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/auth/email/sign-in", stringContent);
        var responseBody = await response.Content.ReadAsStringAsync();

        var results = JsonSerializer.Deserialize<dynamic>(responseBody);
        var token = results.GetProperty("accessToken");
        return token;
    }

    [TestMethod]
    public async Task CheckCreateReport()
    {
        // Authorization
        var token = await SignIn();
        _httpClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", token.ToString());

        // Test
        var reservationId = new Guid();
        CreateReportDTO request = new()
        {
            Penalize = false,
            Reason = "Testing"
        };
        var content = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/reservations/{reservationId}/report", stringContent);
        Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NotFound);

    }

    [TestMethod]
    public async Task CheckCreateUnavailability()
    {
        // Authorization
        var token = await SignIn();
        _httpClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", token.ToString());

        // Testing
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();

        var id = context.Boats.FirstOrDefault().Id;

        var pars = new {
            id,
            start = DateTimeOffset.Now,
            end = DateTimeOffset.Now.AddDays(2)
        };

        var content = JsonSerializer.Serialize(pars);
        var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"/boats/{id}/calendar/create-unavailability", stringContent);
        Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task CheckGetReservations()
    {
        // Authorization
        var token = await SignIn();
        _httpClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", token.ToString());

        // Testing
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();

        var pars = new
        {
            page = 0,
            status = "Cancelled"
        };

        var content = JsonSerializer.Serialize(pars);
        var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");

        var response = await _httpClient.GetAsync("/boats/reservations");
        Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
    }
}

