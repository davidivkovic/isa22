namespace API.Core.Model;

public class Complaint
{
    public string         Reason    { get; set; }
    public string         Answer    { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public bool           Answered  { get; set; }

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

    public void Approve() => IsApproved = true;
}

public class Reservation : Entity
{
    public static class ReservationStatus
    {
        public const string None = "None";
        public const string Created = "Created";
        public const string Fulfilled = "Fulfilled";
        public const string Cancelled = "Cancelled";
    }

    public string         Status             { get; set; }
    public User           User               { get; set; }
    public Business       Business           { get; set; }
    public DateTimeOffset Start              { get; set; }
    public DateTimeOffset End                { get; set; }
    public DateTimeOffset Timestamp          { get; set; }
    public Payment        Payment            { get; set; }
    public double         DiscountPercentage { get; set; }
    public List<Service>  Services           { get; set; }
    public Report         Report             { get; set; }
    public Complaint      Complaint          { get; set; }

    public int Units => Business.GetTotalUnits(Start, End);

    public Reservation() {}

    public Reservation(
        User user,
        Business business,
        DateTimeOffset start,
        DateTimeOffset end,
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
        Timestamp = DateTimeOffset.Now;
        //DiscountPercentage = User.Level.DiscountPercentage;
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
        if (Start - TimeSpan.FromDays(3) > DateTimeOffset.Now)
        {
            Status = ReservationStatus.Cancelled;
            Delete();
            return true;
        }
        return false;
    }

    public void Complain(string content)
    {
        Complaint = new()
        {
            Reason = content,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    public void ReportUser(string reason, bool penalize)
    {
        Report = new()
        {
            Reason = reason,
            Penalize = penalize
        };
    }

}