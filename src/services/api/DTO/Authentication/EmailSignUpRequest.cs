namespace API.DTO.Authentication;

using API.Core.Model;
using System.ComponentModel.DataAnnotations;

public class EmailSignUpRequest
{
    [Required] 
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public string[] Roles { get; set; }

    [Required]
    public Address Address { get; set; }

    [Required]
    public string Phone { get; set; }

    public string JoinReason { get; set; }
}