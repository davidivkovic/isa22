namespace API.Core.Model;

public class Finance : Entity
{
    public double TaxPercentage { get; set; }
    public int CustomerPoints { get; set; }
    public int BusinessOwnerPoints { get; set; }
}