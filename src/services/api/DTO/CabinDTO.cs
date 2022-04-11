using API.Core.Model;

namespace API.DTO;

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

