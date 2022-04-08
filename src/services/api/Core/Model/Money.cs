namespace API.Core.Model;

public class Money
{
    public decimal Amount   { get; set; }
    public string  Currency { get; set; }
    public string  Symbol   => _symbols[Currency];

    private readonly Dictionary<string, string> _symbols = new()
    {
        ["EUR"] = "€",
        ["USD"] = "$",
        ["RSD"] = "din"
    };

    public Money() {}

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
}