namespace API.Core.Model;

public class Complaint
{
    public string   Reason    { get; set; }
    public string   Answer    { get; set; }
    public DateTime Timestamp { get; set; }
    public bool     Answered  { get; set; }

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

    public ReservationStatus Status             { get; set; }
    public User              User               { get; set; }
    public Business          Business           { get; set; }
    public DateTime          Start              { get; set; }
    public DateTime          End                { get; set; }
    public DateTime          Timestamp          { get; set; }
    public Payment           Payment            { get; set; }
    public double            DiscountPercentage { get; set; }
    public List<Service>     Services           { get; set; }
    public Report            Report             { get; set; }
    public Complaint         Complaint          { get; set; }

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