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
}

public class CreateSaleDTO
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public int People { get; set; }
    public List<Service> Services { get; set; }
    public double DiscountPercentage { get; set; }
}

public class SaleDTO : CreateSaleDTO
{
    public Money Price { get; set; }
}