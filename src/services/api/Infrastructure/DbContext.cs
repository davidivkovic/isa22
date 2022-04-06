using Microsoft.EntityFrameworkCore;

using api.Core.Models;

namespace api.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Adventure>    Adventure     { get; set; }
    public DbSet<Boat>         Boats         { get; set; }
    public DbSet<Business>     Businesses    { get; set; }
    public DbSet<Cabin>        Cabins        { get; set; }
    public DbSet<Complaint>    Complaints    { get; set; }
    public DbSet<Loyalty>      LoyaltyLevels { get; set; }
    public DbSet<Reservation>  Reservations  { get; set; }
    public DbSet<Review>       Reviews       { get; set; }
    public DbSet<Service>      Services      { get; set; }
    public DbSet<Tax>          Tax           { get; set; }
    public DbSet<User>         Users         { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Owned<Address>();
        modelBuilder.Owned<BoatCharacteristics>();
        modelBuilder.Owned<BoatEquipment>();
        modelBuilder.Owned<Complaint>();
        modelBuilder.Owned<DeletionRequest>();
        modelBuilder.Owned<Money>();
        modelBuilder.Owned<Payment>();
        modelBuilder.Owned<Penalty>();
        modelBuilder.Owned<Report>();
        modelBuilder.Owned<Room>();
        modelBuilder.Owned<Rule>();
        modelBuilder.Owned<Service>();
        modelBuilder.Owned<Slot>();

        modelBuilder.Entity<User>().HasMany(b => b.Subscriptions);
        modelBuilder.Entity<User>().Navigation(c => c.Address).IsRequired();
        modelBuilder.Entity<Boat>().Navigation(c => c.Equipment).IsRequired();
        modelBuilder.Entity<Business>().HasOne(b => b.Owner);
        modelBuilder.Entity<Business>().HasMany(b => b.Subscribers);
        modelBuilder.Entity<Business>().Navigation(c => c.Address).IsRequired();
        modelBuilder.Entity<Loyalty>().HasKey(l => l.Name);
        modelBuilder.Entity<Reservation>().HasIndex(r => r.Start);
        modelBuilder.Entity<Reservation>().HasIndex(r => r.End);
        modelBuilder.Entity<Review>().HasOne(r => r.User);
        modelBuilder.Entity<Review>().HasOne(r => r.Business);

        modelBuilder.EnableDeleteFilter();
    }
}