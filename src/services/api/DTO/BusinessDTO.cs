namespace API.DTO;

using API.Core.Model;

public class BusinessCreateDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Address Address { get; set; }
    //public List<IFormFile> Images { get; set; }
    public int People { get; set; }
    public List<Rule> Rules { get; set; }
    public List<Service> Services { get; set; }
    public double CancellationFee { get; set; }
}

public class BusinessDTO : BusinessCreateDTO
{
    public Guid Id { get; set; }
}