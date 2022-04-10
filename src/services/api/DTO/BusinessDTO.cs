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
}

public class CreateBusinessDTO : BaseBusinessDTO
{
    public List<IFormFile> ImageData { get; set; }
}

public class UpdateBusinessDTO : BaseBusinessDTO
{
    public Guid Id { get; set; }
}

public class BusinessDTO : BaseBusinessDTO
{
    public Guid Id { get; set; }
    public User Owner { get; set; }
    public List<string> Images { get; set; }
    public int NumberOfReviews { get; set; }
    public int Rating { get; set; }

    public void WithImages(Func<Guid, string, string> imageUrl)
    {
        Images = Images.Select(image => imageUrl(Id, image)).ToList();
    }
}