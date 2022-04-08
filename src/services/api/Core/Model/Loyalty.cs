namespace API.Core.Model;

public class Loyalty
{
    public string Name               { get; set; }
    public int    Threshold          { get; set; }
    public double DiscountPercentage { get; set; }
}
