namespace api.Core.Models;

public class Money
{
    public decimal Amount   { get; private set; }
    public string  Currency { get; private set; }
    public string  Symbol   { get; private set; }

    private readonly Dictionary<string, string> _symbols = new()
    {
        ["EUR"] = "€",
        ["USD"] = "$",
        ["RSD"] = "din"
    };

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
        Symbol = _symbols[currency];
    }
}