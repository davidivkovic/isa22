namespace API.DTO;

using API.Core.Model;

public abstract class BaseBusinessDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Address Address { get; set; }
    public int People { get; set; }
    public List<Rule> Rules { get; set; }
    public List<Service> Services { get; set; }
    public double CancellationFee { get; set; }
    public MoneyDTO PricePerUnit { get; set; }
    public string UnitName { get; set; }
}

public class CreateBusinessDTO : BaseBusinessDTO
{
    //public List<IFormFile> ImageData { get; set; }
}

public class UpdateBusinessDTO : BaseBusinessDTO
{
    public Guid Id { get; set; }
}

public class BusinessDTO : BaseBusinessDTO
{
    public Guid Id { get; set; }
    public UserDTO Owner { get; set; }
    public List<string> Images { get; set; }
    public int NumberOfReviews { get; set; }
    public double Rating { get; set; }
    public new Money PricePerUnit { get; set; }
    public Money TotalPrice { get; set; }
    public bool IsSubscribed { get; set; }
    public bool IsDeletable { get; set; } = true;
    public bool IsPenalized { get; set; }
    public Loyalty LoyaltyLevel { get; set; }
    public List<SaleDTO> Sales { get; set; }
    public List<ReviewDTO> Reviews { get; set; }

    public void WithImages(Func<Guid, string, string> imageUrl)
    {
        Images = Images.Select(image => imageUrl(Id, image)).ToList();
    }
}

public class SubscriptionDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public double Rating { get; set; }
    public List<string> Images { get; set; }
    public void WithImages(Func<Guid, string, string> imageUrl)
    {
        Images = Images.Select(image => imageUrl(Id, image)).ToList();
    }
}
