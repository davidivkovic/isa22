namespace API.Security;

using Microsoft.AspNetCore.Identity;

using API.Core.Model;

public class RefreshToken : IdentityUserToken<Guid>, IDeletable
{
    public Guid Id { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get ; set; }

    public bool HasExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsDeleted && !HasExpired;

    public RefreshToken() { }

    public RefreshToken(Guid userId)
    {
        Guid token = Guid.NewGuid();

        Id = token;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddMonths(3);
        Value = token.ToString("N");
    }

    public void Delete() => IsDeleted = true;

    public override string ToString()
    {
        return Value;
    }
}