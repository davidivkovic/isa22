using API.Core.Model;
using System.ComponentModel.DataAnnotations;

namespace API.DTO;

public class UserDTO
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public List<string> Roles { get; set; }
    public Address Address { get; set; }
}

public class UpdateUserDTO
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public Address Address { get; set; }
     
    public string Biography { get; set; }
}

public class LoyaltyDTO
{
    public int Points { get; set; }
    public Loyalty Current { get; set; }
    public Loyalty Next { get; set; }
}

public class UserProfileDTO
{
    public UserDTO User { get; set; }
    public LoyaltyDTO LoyaltyLevel { get; set; }
    public Request DeletionRequest { get; set; }
}