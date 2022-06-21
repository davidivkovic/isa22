namespace API.DTO.Authentication;

public class AcceptRegistrationRequest
{
    public Guid UserId{ get; set; }
    public string DenialReason { get; set; }
}