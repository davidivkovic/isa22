namespace api.Core.Models;

public class Adventure : Business
{
    public string   Biography                 { get; set; }
    public double   CancellationFeePercentage { get; set; }
    public override TimeSpan Unit => TimeSpan.FromHours(1);

    public Adventure() {}

    public Adventure(
        string name,
        string description,
        Address address,
        Money pricePerNight,
        User owner,
        List<string> images,
        List<Rule> rules,
        List<Service> services,
        double cancellationFeePercentage,
        string biography
    ) : base(name, description, address, pricePerNight, owner, images, rules, services)
    {
        CancellationFeePercentage = cancellationFeePercentage;
        Biography = biography;
    }
}