using API.Core.Model;

namespace API.DTO.Authentication;

public class PendingRegistrationDTO
{
    public UserDTO User { get; set; }
    public Request JoinRequest { get; set; }
}