namespace API.DTO;

public class AdventureCreateDTO : BusinessCreateDTO
{
    public List<string> FishingEquipment { get; set; }
    public string Biography { get; set; }
}

public class AdventureDT0 : AdventureCreateDTO
{
    public Guid Id { get; set; }
}