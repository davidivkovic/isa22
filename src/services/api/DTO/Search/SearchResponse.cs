namespace API.DTO.Search;

using API.Core.Model;

public class SearchResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public List<string> Images { get; set; }
    public Address Address { get; set; }
    public Money Price { get; set; }
    public Money PricePerUnit { get; set; }
    public double Discount { get; set; }
    public int People { get; set; }
    public double Rating { get; set; }
    public double CancellationFee { get; set; }
}

public class CabinSearchResponse : SearchResponse
{
    public List<Room> Rooms { get; set; }
}

public class BoatSearchResponse : SearchResponse
{
    public BoatCharacteristics BoatCharacteristics { get; set; }
}

public class AdventureSearchResponse : SearchResponse 
{
    public List<string> FishingEquipment { get; set; }
}
