namespace API.Core.Model;

public class Sale : Reservation
{
    public int People { get; set; }

    public Sale() {}

    public Sale(
        Business business,
        DateTimeOffset start,
        DateTimeOffset end,
        double discountPercentage,
        int people,
        List<Service> chosenServices
    )
    {
        Business = business;
        Start = start;
        End = end;
        DiscountPercentage = discountPercentage;
        People = people;
        Services = chosenServices;
    }

    public Money Price(double userDiscountPercentage)
    {
        double discount = userDiscountPercentage + DiscountPercentage;
        return Business.Price(Start, End, People, discount, Services);
    }

    public void Sell(User user, double userDiscountPercentage, double taxPercentage)
    {
        Status = ReservationStatus.Created;
        User = user;
        Timestamp = DateTimeOffset.Now;
        DiscountPercentage += userDiscountPercentage;
        Payment = new (Price(DiscountPercentage), taxPercentage);
    }
}