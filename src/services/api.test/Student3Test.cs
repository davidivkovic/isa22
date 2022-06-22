using API.Core.Model;
using API.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using static API.Core.Model.Reservation;

namespace api.test;

[TestClass]
public class Student3Tests
{
    private readonly HttpClient _httpClient;
    private static CustomWebApplicationFactory<Program> _app;

    public Student3Tests()
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
        Reservation r = new Reservation()
        {
            Business = ca,
            User = new User(),
            Start = new DateTime(2022, 6, 23),
            End = new DateTime(2022, 6, 24),
            Services = new List<Service>(),
            Payment = new Payment(new Money() { Amount = 10, Currency = "EUR" }, 10),
            DiscountPercentage = 5,
            Status = ReservationStatus.Created
        };
        context.Add(r);
        Review re = new Review()
        {
            User = new User(),
            Rating = 5,
            Content = "Fina vikendica",
            Timestamp = DateTime.Now,
            Approved = false,
            Rejected = false
        };
        context.Add(re);
        context.SaveChanges();
    }

    [TestMethod]
    public void CheckReservationCancel()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        var reservation = context.Reservations.FirstOrDefault();

        Assert.AreEqual("Created", reservation.Status);

        Assert.AreEqual(false, reservation.Cancel());
    }

    [TestMethod]
    public void CheckReservationStatus()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        var reservation = context.Reservations.FirstOrDefault();

        reservation.Start = new DateTime(2022, 7, 20);
        reservation.End = new DateTime(2022, 7, 21);

        Assert.AreEqual("Created", reservation.Status);

        reservation.Cancel();

        Assert.AreEqual("Cancelled", reservation.Status);

        reservation.Fulfill();

        Assert.AreEqual("Fulfilled", reservation.Status);
    }

    [TestMethod]
    public void ReviewActions()
    {
        using var scope = _app.Services.GetService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetService<AppDbContext>();
        var review = context.Reviews.FirstOrDefault();
        var cabin = context.Cabins.FirstOrDefault(c => c.Name == "Kabina");

        review.Business = cabin;

        Assert.AreEqual(3.5, cabin.Rating);
        Assert.AreEqual(false, review.Approved);

        review.Approve();

        double d = cabin.Rating;
        Assert.AreEqual(3.7, Math.Round(d, 1));
        Assert.AreEqual(true, review.Approved);
    }
}