using Microsoft.AspNetCore.Identity;

namespace API.Core.Model;

public class Request
{
    public DateTime Timestamp { get; set; }
    public string   Reason   { get; set; }
    public string   Answer   { get; set; }
    public bool     Approved { get; set; }
    public bool     Rejected { get; set; }

    public Request() { }

    public Request(string reason)
    {
        Reason = reason;
        Timestamp = DateTime.UtcNow;
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
    public int      Points  { get; set; }
    public DateTime Expires { get; set; }

    private void Unexpire()
    {
        Expires = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.Day + 1);
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

public enum Role
{
    Admin,
    Customer,
    BoatOwner,
    CabinOwner,
    Fisher
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
    public List<Role>      Roles           { get; set; } = new();
    public bool            LockedOut       { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public bool IsAdmin => Roles.Contains(Role.Admin);
    public bool IsCustomer => Roles.Contains(Role.Customer);
    public bool IsBusinessOwner => Roles.Any(r => new Role[] 
    { 
        Role.Fisher,
        Role.CabinOwner,
        Role.BoatOwner
    }
    .Contains(r));

    public void Delete() => IsDeleted = true;
}