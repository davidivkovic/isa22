namespace API.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using API.Core.Model;
using API.Security;

public class AppDbContext : IdentityDbContext<
    User,
    IdentityRole<Guid>,
    Guid,
    IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    RefreshToken
>
{
    public DbSet<Adventure>   Adventures    { get; set; }
    public DbSet<Boat>        Boats         { get; set; }
    public DbSet<Business>    Businesses    { get; set; }
    public DbSet<Cabin>       Cabins        { get; set; }
    public DbSet<Complaint>   Complaints    { get; set; }
    public DbSet<Loyalty>     LoyaltyLevels { get; set; }
    public DbSet<Reservation> Reservations  { get; set; }
    public DbSet<Sale>        Sales         { get; set; }
    public DbSet<Review>      Reviews       { get; set; }
    public DbSet<Service>     Services      { get; set; }
    public DbSet<Finance>     Finances      { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        base.OnModelCreating(builder);

        builder.Owned<Address>();
        builder.Owned<BoatCharacteristics>();
        builder.Owned<BoatEquipment>();
        builder.Owned<Complaint>();
        builder.Owned<Request>();
        builder.Owned<Money>();
        builder.Owned<Payment>();
        builder.Owned<Penalty>();
        builder.Owned<Report>();
        builder.Owned<Room>();
        builder.Owned<Rule>();
        builder.Owned<Service>();
        builder.Owned<OneTimePassword>();

        //builder.Entity<User>().HasMany(b => b.Subscriptions);
        builder.Entity<User>().Navigation(c => c.Address).IsRequired();
        builder.Entity<Boat>().Navigation(c => c.Equipment).IsRequired();
        builder.Entity<Business>().HasOne(b => b.Owner);
        //builder.Entity<Business>().HasMany(b => b.Subscribers);
        builder.Entity<Business>().Navigation(c => c.Address).IsRequired();
        builder.Entity<Loyalty>().HasKey(l => l.Name);
        builder.Entity<Reservation>().HasIndex(r => r.Start);
        builder.Entity<Reservation>().HasIndex(r => r.End);
        builder.Entity<Review>().HasOne(r => r.User);
        builder.Entity<Review>().HasOne(r => r.Business);
        builder.Entity<Slot>().HasIndex(s => s.Start);
        builder.Entity<Slot>().HasIndex(s => s.End);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<User>()
               .HasMany(u => u.Subscriptions)
               .WithMany(b => b.Subscribers)
               .UsingEntity(j => j.ToTable("Subscriptions"));
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<RefreshToken>().ToTable("RefreshTokens").HasKey(k => k.Id);

        builder.EnableDeleteFilter();
    }
}