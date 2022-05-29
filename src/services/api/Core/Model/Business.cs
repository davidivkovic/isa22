namespace API.Core.Model;

public class Rule
{
    public string Name    { get; set; }
    public bool   Allowed { get; set; }
}

public class Service
{
    public string Name  { get; set; }
    public Money  Price { get; set; }
    public static bool operator == (Service lhs, Service rhs) => lhs?.Name == rhs?.Name;
    public static bool operator != (Service lhs, Service rhs) => lhs?.Name != rhs?.Name;

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        return ((Service)obj).Name == Name;
    }

    public override int GetHashCode() => Name.GetHashCode();
}

public abstract class Business : Entity
{
    public string            Name            { get; set; }
    public string            Description     { get; set; }
    public Address           Address         { get; set; }
    public Money             PricePerUnit    { get; set; }
    public User              Owner           { get; set; }
    public List<string>      Images          { get; set; } = new();
    public List<Rule>        Rules           { get; set; } = new();
    public List<Service>     Services        { get; set; } = new();
    public List<Slot>        Availability    { get; set; } = new();
    public List<Reservation> Reservations    { get; set; } = new();
    public List<Review>      Reviews         { get; set; } = new();
    public List<User>        Subscribers     { get; set; } = new();
    public int               People          { get; set; }
    public int               NumberOfReviews { get; set; }
    public double            Rating          { get; set; }
    public double            CancellationFee { get; set; }
    public virtual           TimeSpan Unit   { get; }

    public string UnitName => Unit.Hours switch
    {
        <= 1 => "Hours",
        <= 24 or _ => "Days"
    };

    public Business() {}

    public Business(
        string name,
        string description,
        Address address,
        Money pricePerUnit,
        User owner,
        List<string> images,
        List<Rule> rules,
        List<Service> services,
        double cancellationFee
    )
    {
        Name = name;
        Description = description;
        Address = address;
        PricePerUnit = pricePerUnit;
        Owner = owner;
        Images = images;
        Rules = rules;
        Services = services;
        CancellationFee = cancellationFee;
    }

    public bool IsAvailable(DateTimeOffset start, DateTimeOffset end)
    {
        return Availability.Exists(s => s.Contains(start, end) && s.Available) &&
              !Availability.Exists(s => s.Intersects(start, end) && !s.Available);
    }

    public Review Review(User user, double rating, string content)
    {
        Review r = new()
        {
            User = user,
            Business = this,
            Rating = rating,
            Content = content,
        };

        Reviews.Add(r);
        return r;
    }

    public void Rate(double rating)
    {
        Rating = (NumberOfReviews * Rating + rating) / ++NumberOfReviews;
    }

    public int GetTotalUnits(DateTimeOffset start, DateTimeOffset end)
    {
        double totalUnits = 1;
        if (Unit.Hours == 1)
        {
            totalUnits = (end - start).TotalHours;
        }
        else if (Unit.Days == 1)
        {
            totalUnits = (end - start).TotalDays;
        }
        return (int)Math.Round(totalUnits);
    }

    public Money Price
    (
        DateTimeOffset start,
        DateTimeOffset end,
        int people,
        double discountPercentage,
        List<Service> chosenServices
    )
    {
        decimal discount = 1 - Convert.ToDecimal(discountPercentage / 100);
        decimal amount = GetTotalUnits(start, end) *
                         people                    *
                         PricePerUnit.Amount       +
                         chosenServices.Sum(s => s.Price.Amount);

        return new(amount * discount, PricePerUnit.Currency);
    }
}