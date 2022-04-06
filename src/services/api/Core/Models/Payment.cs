namespace api.Core.Models;

public class Payment
{
    public Money    Price              { get; private set; }
    public double   DiscountPercentage { get; set; }
    public double   TaxPercentage      { get; private set; }
    public DateTime Timestamp          { get; private set; }
    
    private Payment() {}

    public Payment(Money price, double taxPercentage)
    {
        Price = price;
        TaxPercentage = taxPercentage;
        Timestamp = DateTime.Now;
    }
}