namespace API.Services.Email.Messages;

using API.Core.Model;
using Scriban;

public class NewReview : IMessage
{
    public string TemplateName => "new-review.html";
    public string Subject => "New review on adventure.com";

    private readonly Review review;
    private readonly Reservation reservation;
    private readonly string contactUrl;
    private readonly string businessImageUrl;

    public NewReview(Review review, string businessImageUrl, string contactUrl)
    {
        this.review = review;
        this.businessImageUrl = businessImageUrl;
        this.contactUrl = contactUrl;
    }

    public string Render(Template messageTemplate)
    {
        string ratingDescription = review.Rating switch
        {
            5 => "Excellent",
            4 => "Very Good",
            3 => "Good",
            2 => "Okay",
            1 or _ => "Bad"
        };

        return messageTemplate.Render(new
        {
            review,
            reservation,
            ratingDescription,
            businessImageUrl,
            contactUrl
        });
    }
}
