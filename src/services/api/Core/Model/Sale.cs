namespace API.Core.Model;

public class Sale : Reservation
{
    public int People { get; set; }

    public Sale(
        Business business,
        DateTime start,
        DateTime end,
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

    public Money Price(User user)
    {
        double discount = user.Level.DiscountPercentage + DiscountPercentage;
        return Business.Price(Start, End, People, discount, Services);
    }

    public void Sell(User user, double taxPercentage)
    {
        Status = ReservationStatus.Created;
        Timestamp = DateTime.Now;
        DiscountPercentage = user.Level.DiscountPercentage;
        Payment = new (Price(user), taxPercentage);
    }
}