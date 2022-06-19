namespace API.DTO;

using API.Core.Model;

public class MakeReservationDTO
{
    public int People { get; set; } 
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public List<Service> Services { get; set; }
}

public class ReservationDTO
{
    public Guid Id { get; set; }
    public UserDTO User { get; set; }
    public List<Service> Services { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public BusinessDTO Business { get; set; }
    public Payment Payment { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public int Units { get; set; }
    public bool IsCancellable { get; set; }
}

public class DashboardReservationDTO
{
    public class DashboardUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class DashboardBusinessDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public List<string> Images { get; set; }

        public void WithImages(Func<Guid, string, string> imageUrl)
        {
            Images = Images.Select(image => imageUrl(Id, image)).ToList();
        }
    }

    public Guid Id { get; set; }
    public DashboardUserDTO User { get; set; }
    public DashboardBusinessDTO Business { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public Payment Payment { get; set; }
}

public class CreateSaleDTO
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public int People { get; set; }
    public List<Service> Services { get; set; }
    public double DiscountPercentage { get; set; }
    public Guid CustomerId { get; set; }
}

public class SaleDTO : CreateSaleDTO
{
    public Money Price { get; set; }
    public Guid Id { get; set; }
}