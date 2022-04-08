namespace API.DTO.Authentication;

public class SignInResponse
{
    public string AccessToken { get; set; }
    public UserDTO User { get; set; }
}