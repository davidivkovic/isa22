namespace api.Core.Models;

public class Cabin : Business
{
    public List<Room>        Rooms { get; private set; } = new();
    public int               NumberOfRooms => Rooms.Count;
    public override TimeSpan Unit => TimeSpan.FromDays(1);

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
        List<Room> rooms
    ) : base(name, description, address, pricePerNight, owner, images, rules, services)
    {
        Rooms = rooms;
    }
}
