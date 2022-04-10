namespace API.DTO;

public class CreateBoatDTO : CreateBusinessDTO
{
    public string BoatType { get; set; }
    public string BoatLength { get; set; }
    public string MotorNumber { get; set; }
    public double MotorPower { get; set; }
    public double MaxSpeed { get; set; }
    public List<string> NavigationEquipment { get; set; }
    public List<string> FishingEquipment { get; set; }
}
public class UpdateBoatDTO : UpdateBusinessDTO
{
    public string BoatType { get; set; }
    public string BoatLength { get; set; }
    public string MotorNumber { get; set; }
    public double MotorPower { get; set; }
    public double MaxSpeed { get; set; }
    public List<string> NavigationEquipment { get; set; }
    public List<string> FishingEquipment { get; set; }
}
public class BoatDTO : BusinessDTO
{
    public string BoatType { get; set; }
    public string BoatLength { get; set; }
    public string MotorNumber { get; set; }
    public double MotorPower { get; set; }
    public double MaxSpeed { get; set; }
    public List<string> NavigationEquipment { get; set; }
    public List<string> FishingEquipment { get; set; }
}

