using API.Core.Model;
using API.DTO.Authentication;
using API.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
using System.Transactions;
using System.Web;

namespace api.test;

[TestClass]
public class ApiTests
{
    private readonly HttpClient _httpClient;
    private static CustomWebApplicationFactory<Program> _app;


    public ApiTests()
    {
        _httpClient = _app.CreateDefaultClient();
    }

    [ClassInitialize]
    public static void TestFixtureSetup(TestContext c)
    {
        _app = new CustomWebApplicationFactory<Program>();
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        context.Add(new User()
        {
            Id = new Guid(),
            FirstName = "Milica",
            LastName = "Draca",
            Email = "user@example.com",
            Roles = new() { "Customer" }
        });

        context.Cabins.Add(new Cabin()
        {
            Name = "Sweet home",
            Address = new Address()
            {
                Country = "Serbia",
                City = "Novi Sad"
            },
            People = 2,
            PricePerUnit = new()
            {
                Amount = 10,
                Currency = "USD"
            },
        });
        context.SaveChanges();
    }



    [TestMethod]
    public void CheckGetUser()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();

        var user = context.Users.Where(u => u.Roles.Contains("Customer")).FirstOrDefault();
        Assert.AreEqual(user.FirstName, "Milica");
    }


    [TestMethod]
    public async Task CheckSignUpEmailUnavailable()
    {
        EmailSignUpRequest request = new()
        {
            Email = "admin@api.com",
            FirstName = "Pera",
            LastName = "Peric",
            Address = new Address(),
            Password = "password123",
            PhoneNumber = "0545333",
            Roles = new[] { "Customer" }
        };

        var content = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/auth/email/sign-up", stringContent);
        Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CheckSearch()
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["city"] = "Novi Sad";
        query["country"] = "Serbia";
        query["start"] = "2022-07-10";
        query["end"] = "2022-07-12";
        query["people"] = "2";

        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();

        string queryString = query.ToString();

        var response = await _httpClient.GetAsync("/cabins/search?" + queryString);
        var responseBody = await response.Content.ReadAsStringAsync();

        var results = JsonSerializer.Deserialize<dynamic>(responseBody);

        Assert.AreEqual(results.GetProperty("totalResults").GetInt32(), 1);
    }

}
