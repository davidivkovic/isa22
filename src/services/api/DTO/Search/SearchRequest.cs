using Microsoft.AspNetCore.Mvc;

namespace API.DTO.Search;

public class SearchRequest
{
    [FromQuery]
    public DateTime Start { get; set; }
    [FromQuery]
    public DateTime End { get; set; }
    [FromQuery]
    public string City { get; set; }
    [FromQuery]
    public string Country { get; set; }
    [FromQuery]
    public int People { get; set; }
    [FromQuery]
    public decimal PriceLow { get; set; }
    [FromQuery] 
    public decimal PriceHigh { get; set; }
    [FromQuery] 
    public string Currency { get; set; }
    [FromQuery] 
    public double RatingHigher { get; set; }
    [FromQuery]
    public string Direction { get; set; }
    [FromQuery]
    public int Page { get; set; }
}

public class CabinSearchRequest : SearchRequest
{
    [FromQuery]
    public double Rooms { get; set; }
}