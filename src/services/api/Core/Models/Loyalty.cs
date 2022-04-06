namespace api.Core.Models;

public class Loyalty
{
    public string Name               { get; set; }
    public int    Threshold          { get; set; }
    public double DiscountPercentage { get; set; }
}
