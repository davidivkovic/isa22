namespace API.Core.Model;

public class Adventure : Business
{
    public string            Biography        { get; set; }
    public List<string>      FishingEquipment { get; set; } = new();
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
        string biography,
        double cancellationFee
    ) : base(name, description, address, pricePerNight, owner, images, rules, services, cancellationFee)
    {
        Biography = biography;
    }
}