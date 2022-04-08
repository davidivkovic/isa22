namespace API.DTO.Authentication;

public class AcceptRegistrationRequest
{
    public string Email { get; set; }
    public string DenialReason { get; set; }
}