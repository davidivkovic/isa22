namespace API.DTO;

public class CreateAdventureDTO : CreateBusinessDTO
{
    public List<string> FishingEquipment { get; set; }
    public string Biography { get; set; }
}

public class UpdateAdventureDTO: UpdateBusinessDTO 
{
    public List<string> FishingEquipment { get; set; }
    public string Biography { get; set; }
}

public class AdventureDT0 : BusinessDTO
{
    public List<string> FishingEquipment { get; set; }
    public string Biography { get; set; }
}