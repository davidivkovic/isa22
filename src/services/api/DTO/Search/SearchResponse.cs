namespace API.DTO.Search;

using API.Core.Model;

public class SearchResponse
{
    public string Name { get; set; }
    public string Image { get; set; }
    public Address Address { get; set; }
    public MoneyDTO Price { get; set; }
    public double Discount { get; set; }
    public int People { get; set; }
}

public class CabinSearchResponse : SearchResponse
{
    public int Rooms { get; set; }
    public int Beds { get; set; }
}

public class AdventureSearchResponse : SearchResponse {}
public class BoatSearchResponse : SearchResponse {}