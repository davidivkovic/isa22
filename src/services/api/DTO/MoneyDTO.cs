namespace API.DTO;

using System.ComponentModel.DataAnnotations;

public class MoneyDTO
{
    [Required]
    public decimal Amount { get; set; }

    [Required]
    [RegularExpression("EUR|USD|RSD", ErrorMessage = "Please enter a valid currency.")]
    public string Currency { get; set; }
}