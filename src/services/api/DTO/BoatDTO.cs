namespace API.DTO;

using API.Core.Model;

public class CreateBoatDTO : CreateBusinessDTO
{
    public BoatCharacteristics Characteristics { get; set; }
    public BoatEquipment Equipment { get; set; }
}
public class UpdateBoatDTO : UpdateBusinessDTO
{
    public BoatCharacteristics Characteristics { get; set; }
    public BoatEquipment Equipment { get; set; }
}
public class BoatDTO : BusinessDTO
{
    public BoatCharacteristics Characteristics { get; set; }
    public BoatEquipment Equipment { get; set; }
}

