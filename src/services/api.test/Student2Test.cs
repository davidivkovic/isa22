using API.Core.Model;
using API.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace api.test;

[TestClass]
public class Student2Tests
{
    private readonly HttpClient _httpClient;
    private static CustomWebApplicationFactory<Program> _app;

    public Student2Tests()
    {
        _httpClient = _app.CreateDefaultClient();
    }

    [ClassInitialize]
    public static void TestFixtureSetup(TestContext c)
    {
        _app = new CustomWebApplicationFactory<Program>();
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();

        Cabin ca = new Cabin()
        {
            Name = "Kabina",
            PricePerUnit = new Money() { Currency = "EUR", Amount = 10 },
            People = 2,
            Services = new List<Service>(),
            Rating = 3.5,
            NumberOfReviews = 6,
            Availability = new List<Slot>() { new Slot() { Available = false, Start = new DateTime(2022, 7, 28), End = new DateTime(2022, 7, 29) } },
            Subscribers = new List<User>()
        };
        context.Add(ca);
        context.SaveChanges();
    }
    [TestMethod]
    public void CheckPriceCaluclation()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        var cabin = context.Cabins.FirstOrDefault();
        DateTimeOffset dt1 = new DateTimeOffset(DateTime.Now);
        DateTimeOffset dt2 = new DateTimeOffset(DateTime.Now + TimeSpan.FromDays(2));
        decimal turePrice = 36;
        decimal price = cabin.Price(dt1, dt2, cabin.People, 10, new List<Service>()).Amount;

        Assert.AreEqual(turePrice, price);
        
    }

    [TestMethod]
    public void CheckRatingCalculation()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        var cabin = context.Cabins.FirstOrDefault();

        Assert.AreEqual(3.5, cabin.Rating);
        cabin.Rate(5);
        double d = cabin.Rating;
        Assert.AreEqual(3.7, Math.Round(d, 1));
    }


    [TestMethod]
    public void CheckAvailibilityCalculation()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        var cabin = context.Cabins.FirstOrDefault();
        User u = new User();

        Assert.AreEqual(0, cabin.Subscribers.Count);

        cabin.Subscribe(u);

        Assert.AreEqual(1, cabin.Subscribers.Count);

        cabin.Unsubscribe(u);

        Assert.AreEqual(0, cabin.Subscribers.Count);
    }
}