using API.Core.Model;
using API.DTO.Authentication;
using API.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace api.test;

[TestClass]
public class TestStudent3
{
    private static HttpClient _httpClient;
    private static CustomWebApplicationFactory<Program> _app;

    public TestStudent3()
    {
        _httpClient = _app.CreateDefaultClient();
    }

    [ClassInitialize]
    public static void TestFixtureSetup(TestContext c)
    {
        _app = new CustomWebApplicationFactory<Program>();
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();

        context.Add(new Reservation()
        {
            Business = new Adventure(),
            User = new User(),
        });
        context.SaveChanges();
    }

    private async Task<JsonElement> SignIn()
    {
        EmailSignInRequest credentials = new()
        {
            Email = "admin@api.com",
            Password = "admin"
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
    public async Task CheckSetPassword()
    {
        var token = await SignIn();
        _httpClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", token.ToString());

        SetPasswordRequest request = new()
        {
            Email = "admin@api.com",
            Password = "admin",
            NewPassword = "admin2"
        };
        var content = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/auth/password/set", stringContent);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(responseBody, "You have already set your password.");
    }

    [TestMethod]
    public async Task CheckPendingRegistrationRequests()
    {
        var token = await SignIn();
        _httpClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", token.ToString());

        var response = await _httpClient.GetAsync("/users/registrations/pending");
        var responseBody = await response.Content.ReadAsStringAsync();
        var requests = JsonSerializer.Deserialize<List<PendingRequestDTO>>(responseBody);

        Assert.AreEqual(requests.Count, 0);

    }

    [TestMethod]
    public async Task CheckApproveReport()
    {
        var token = await SignIn();
        _httpClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", token.ToString());

        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();

        var reservation = context.Reservations.FirstOrDefault();

        var response = await _httpClient.PostAsync($"/users/reservations/{reservation.Id}/report/update?penalize=false", null);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(responseBody, "There are no reports for this reservation.");
    }

}


