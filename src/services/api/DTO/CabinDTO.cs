namespace API.DTO;

using API.Core.Model;

public class CreateCabinDTO : CreateBusinessDTO
{
    public List<Room> Rooms { get; set; }
}
public class UpdateCabinDTO : UpdateBusinessDTO
{
    public List<Room> Rooms { get; set; }
}
public class CabinDTO : BusinessDTO
{
    public List<Room> Rooms { get; set; }
}

