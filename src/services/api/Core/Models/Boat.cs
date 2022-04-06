namespace api.Core.Models;

public class BoatCharacteristics
{
    public int Seats    { get; set; }
    public int Length   { get; set; }
    public int Engines  { get; set; }
    public int BHP      { get; set; }
    public int TopSpeed { get; set; }
}

public class BoatEquipment
{
    public List<string> Navigational { get; set; } = new();
    public List<string> Additional   { get; set; } = new();
}

public class Boat : Business
{
    public BoatCharacteristics Characteristics { get; set; }
    public BoatEquipment       Equipment { get; set; }
    public double              CancellationFeePercentage { get; set; }
    public override TimeSpan   Unit => TimeSpan.FromHours(1);

    public Boat() {}

    public Boat(
        string name,
        string description,
        Address address,
        Money pricePerNight,
        User owner,
        List<string> images,
        List<Rule> rules,
        List<Service> services,
        BoatCharacteristics characteristics,
        BoatEquipment equipment,
        double cancellationFeePercentage
    ) : base(name, description, address, pricePerNight, owner, images, rules, services)
    {
        Characteristics = characteristics;
        Equipment = equipment;
        CancellationFeePercentage = cancellationFeePercentage;
    }
}