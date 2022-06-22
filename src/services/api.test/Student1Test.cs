using API.Core.Model;
using API.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace api.test;

[TestClass]
public class Student1Tests
{
    private readonly HttpClient _httpClient;
    private static CustomWebApplicationFactory<Program> _app;

    public Student1Tests()
    {
        _httpClient = _app.CreateDefaultClient();
    }

    [ClassInitialize]
    public static void TestFixtureSetup(TestContext c)
    {
        _app = new CustomWebApplicationFactory<Program>();
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
    }
    [TestMethod]
    public void CheckUserSubscription()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        var user = context.Users.FirstOrDefault();
        var cabin = context.Cabins.FirstOrDefault();

        Assert.AreEqual(0, user.Subscriptions.Count);

        user.Subscribe(cabin);

        Assert.AreEqual(1, user.Subscriptions.Count);

        user.Unsubscribe(cabin);

        Assert.AreEqual(0, user.Subscriptions.Count);

    }

    [TestMethod]
    public void CheckSlotAvailability()
    {
        Slot s = new Slot() { Start = new DateTime(2022, 7, 20), End = new DateTime(2022, 7, 22), Available = false };

        DateTime dt1 = DateTime.Now;
        DateTime dt2 = new DateTime(2022, 7, 21);

        Assert.AreEqual(false, s.Contains(dt1, dt1 + TimeSpan.FromDays(1)));
        Assert.AreEqual(true, s.Intersects(dt2, dt2 + TimeSpan.FromHours(1)));
    }


    [TestMethod]
    public void CheckUserDeletion()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        var cabin = context.Cabins.FirstOrDefault();
        User u = new User() { FirstName = "Korisnik" };
        context.Add(u);
        context.SaveChanges();

        var user = context.Users.FirstOrDefault(u => u.FirstName == "Korisnik");

        Assert.AreEqual("Korisnik", user.FirstName);

        user.Delete();

        context.SaveChanges();

        Assert.AreEqual(null, context.Users.FirstOrDefault(u => u.FirstName == "Korisnik"));
    }
}