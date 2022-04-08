namespace API.Core.Model;

public class Room
{
    public int Beds { get; set; }
}

public class Cabin : Business
{
    public override TimeSpan Unit => TimeSpan.FromDays(1);
    public List<Room>        Rooms { get; set; } = new();
    public int               NumberOfRooms => Rooms.Count;

    public Cabin() {}

    public Cabin(
        string name,
        string description,
        Address address,
        Money pricePerNight,
        User owner,
        List<string> images,
        List<Rule> rules,
        List<Service> services,
        List<Room> rooms,
        double cancellationFee
    ) : base(name, description, address, pricePerNight, owner, images, rules, services, cancellationFee)
    {
        Rooms = rooms;
    }
}
