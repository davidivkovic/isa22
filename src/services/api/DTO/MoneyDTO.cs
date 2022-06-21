namespace API.DTO;

using System.ComponentModel.DataAnnotations;

public class MoneyDTO
{
    [Required]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; }
}