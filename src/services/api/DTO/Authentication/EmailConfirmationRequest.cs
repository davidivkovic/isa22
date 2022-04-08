namespace API.DTO.Authentication;

using System.ComponentModel.DataAnnotations;

public class EmailConfirmationRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }
}