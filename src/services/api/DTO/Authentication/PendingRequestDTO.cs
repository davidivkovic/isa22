using API.Core.Model;

namespace API.DTO.Authentication;

public class PendingRequestDTO
{
    public UserDTO User { get; set; }
    public Request Request { get; set; }
}