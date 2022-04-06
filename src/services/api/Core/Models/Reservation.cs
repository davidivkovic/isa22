namespace api.Core.Models;

public class Complaint
{
    public string   Reason    { get; init; }
    public string   Answer    { get; private set; }
    public DateTime Timestamp { get; init; }
    public bool     Answered  { get; private set; }

    public void Reply(string answer)
    {
        Answer = answer;
        Answered = true;
    }
}

public class Report
{
    public string Reason     { get; set; }
    public bool   IsApproved { get; set; }
    public bool   Penalize   { get; set; }
}

public class Reservation : Entity
{
    public enum ReservationStatus
    {
        None,
        Created,
        Fulfilled,
        Cancelled
    }

    public ReservationStatus Status             { get; protected set; }
    public User              User               { get; protected set; }
    public Business          Business           { get; protected set; }
    public DateTime          Start              { get; protected set; }
    public DateTime          End                { get; protected set; }
    public DateTime          Timestamp          { get; protected set; }
    public Payment           Payment            { get; protected set; }
    public double            DiscountPercentage { get; protected set; }
    public List<Service>     Services           { get; protected set; }
    public Report            Report             { get; private set; }
    public Complaint         Complaint          { get; private set; }

    public Reservation() {}

    public Reservation(
        User user,
        Business business,
        DateTime start,
        DateTime end,
        int people,
        List<Service> chosenServices,
        double taxPercentage
    )
    {
        User = user;
        Business = business;
        Start = start;
        End = end;
        Services = chosenServices;
        Status = ReservationStatus.Created;
        Timestamp = DateTime.Now;
        DiscountPercentage = User.Level.DiscountPercentage;
        Money price = Business.Price(
            Start,
            End,
            people,
            DiscountPercentage,
            Services
        );
        Payment = new(price, taxPercentage);
    }

    public void Fulfill()
    {
        Status = ReservationStatus.Fulfilled;
    }

    public bool Cancel()
    {
        if (Start - TimeSpan.FromDays(3) > DateTime.Now)
        {
            Status = ReservationStatus.Cancelled;
            return true;
        }
        return false;
    }
}