namespace API.DTO.Authentication;

using System.ComponentModel.DataAnnotations;

public class EmailSignInRequest
{
    [Required]
    [EmailAddress] 
    public string Email { get; set; }
        
    [Required] 
    public string Password { get; set; }
}