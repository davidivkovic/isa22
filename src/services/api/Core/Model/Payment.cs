namespace API.Core.Model;

public class Payment
{
    public Money    Price              { get; set; }
    public double   DiscountPercentage { get; set; }
    public double   TaxPercentage      { get; set; }
    public DateTime Timestamp          { get; set; }
    
    private Payment() {}

    public Payment(Money price, double taxPercentage)
    {
        Price = price;
        TaxPercentage = taxPercentage;
        Timestamp = DateTime.Now;
    }
}