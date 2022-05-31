using Microsoft.AspNetCore.Identity;

namespace API.Core.Model;

public class Request
{
    public DateTimeOffset Timestamp { get; set; }
    public string   Reason   { get; set; }
    public string   Answer   { get; set; }
    public bool     Approved { get; set; }
    public bool     Rejected { get; set; }

    public Request() { }

    public Request(string reason)
    {
        Reason = reason;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public void Approve()
    {
        Approved = true;
    }

    public void Deny(string answer)
    {
        Answer = answer;
        Rejected = true;
    }
}

public class Penalty
{
    public int            Points  { get; set; }
    public DateTimeOffset Expires { get; set; }

    private void Unexpire()
    {
        Expires = DateTimeOffset.Now.AddMonths(1).AddDays(-DateTimeOffset.Now.Day + 1);
    }

    public void Increment()
    {
        Points++;
        Unexpire();
    }

    public void Reset()
    {
        Points = 0;
        Unexpire();
    }
}

public class OneTimePassword
{
    public string         Value { get; set; }
    public DateTimeOffset Expires { get; set; }

    public bool Validate(string value) => Value == value && Expires >= DateTimeOffset.UtcNow;
}

public static class Role
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";
    public const string BoatOwner = "Boat Owner";
    public const string CabinOwner = "Cabin Owner";
    public const string Fisher = "Fisher";
    public const string BusinessOwner = $"{BoatOwner}, {CabinOwner}, {Fisher}";
    public const string BusinessOwnerOrAdmin = $"{BusinessOwner}, {Admin}";

    public static readonly List<string> Available = new()
    {
        Admin,
        Customer,
        BoatOwner,
        CabinOwner,
        Fisher
    };

    public static bool IsValid(string role) => Available.Contains(role);
}

public class User : IdentityUser<Guid>, IDeletable
{
    public string          FirstName       { get; set; }
    public string          LastName        { get; set; }
    public Address         Address         { get; set; }
    public Penalty         Penalty         { get; set; }
    public int             LoyaltyPoints   { get; set; }
    public Loyalty         Level           { get; set; }
    public Request         JoinRequest     { get; set; }
    public Request         DeletionRequest { get; set; }
    public List<Business>  Subscriptions   { get; set; } = new();
    public bool            IsDeleted       { get; set; }
    public OneTimePassword OneTimePassword { get; set; }
    public List<string>    Roles           { get; set; } = new();
    public bool            LockedOut       { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public bool IsAdmin => Roles.Contains(Role.Admin);
    public bool IsCustomer => Roles.Contains(Role.Customer);
    public bool IsBusinessOwner => Roles.Any(r => new string[] 
    { 
        Role.Fisher,
        Role.CabinOwner,
        Role.BoatOwner
    }
    .Contains(r));

    public void Delete() => IsDeleted = true;

    public void Subscribe(Business business) => Subscriptions.Add(business);
    public void Unsubscribe(Business business) => Subscriptions.Remove(business);


}