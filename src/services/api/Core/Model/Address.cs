namespace API.Core.Model;

public class Address
{
    public string Country    { get; set; }
    public string PostalCode { get; set; }
    public string City       { get; set; }
    public string Street     { get; set; }
    public string Apartment  { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
}