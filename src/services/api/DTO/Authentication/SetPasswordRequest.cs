namespace API.DTO.Authentication;

using System.ComponentModel.DataAnnotations;

public class SetPasswordRequest
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required, DataType(DataType.Password)]
    public string NewPassword { get; set; }
}