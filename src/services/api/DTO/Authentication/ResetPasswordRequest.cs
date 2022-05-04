namespace API.DTO.Authentication;

using System.ComponentModel.DataAnnotations;

public class ResetPasswordRequest
{
    [Required]
    public string Password { get; set; }

    [Required, DataType(DataType.Password)]
    public string NewPassword { get; set; }
}