namespace API.Security;

using Microsoft.AspNetCore.Identity;

using API.Core.Model;

public class RefreshToken : IdentityUserToken<Guid>, IDeletable
{
    public Guid Id { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsDeleted { get ; set; }

    public bool HasExpired => DateTimeOffset.Now >= ExpiresAt;
    public bool IsActive => !IsDeleted && !HasExpired;

    public RefreshToken() { }

    public RefreshToken(Guid userId)
    {
        Guid token = Guid.NewGuid();

        Id = token;
        UserId = userId;
        CreatedAt = DateTimeOffset.Now;
        ExpiresAt = DateTimeOffset.Now.AddMonths(3);
        Value = token.ToString("N");
    }

    public void Delete() => IsDeleted = true;

    public override string ToString()
    {
        return Value;
    }
}