using API.Core.Model;

namespace API.DTO.Finance;

public class FinanceDTO
{
    public double TaxPercentage { get; set; }
    public int CustomerPoints { get; set; }
    public int BusinessOwnerPoints { get; set; }
}

public class FinanceParamsDTO
{
    public List<Loyalty> LoyaltyLevels { get; set; }
    public FinanceDTO Finance { get; set; }
}