namespace API.Services.Email.Messages;

using API.Core.Model;
using Scriban;

public class NewSale : IMessage
{
    public string TemplateName => "new-sale.html";
    public string Subject => "New sale on adventure.com";

    private readonly Sale sale;
    private readonly string user;
    private readonly string contactUrl;
    private readonly string businessImageUrl;

    public NewSale(string user, Sale sale, string businessImageUrl, string contactUrl)
    {
        this.user = user;
        this.sale = sale;
        this.businessImageUrl = businessImageUrl;
        this.contactUrl = contactUrl;
    }

    public string Render(Template messageTemplate)
    {
        var newPrice = sale.Price(0);
        var oldPrice = newPrice.Amount * (decimal)(1 - sale.DiscountPercentage / 100);

        return messageTemplate.Render(new
        {
            NewPrice = newPrice.Symbol + newPrice.Amount,
            OldPrice = newPrice.Symbol + oldPrice,
            FullName = user,
            sale,
            businessImageUrl,
            contactUrl
        });
    }
}
