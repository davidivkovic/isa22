using API.Core.Model;
using Scriban;

namespace API.Services.Email.Messages
{
    public class ComplaintApproved : IMessage
    {
        public string TemplateName => "complaint-approved.html";
        public string Subject => "Approved report on adventure.com";

        private readonly Reservation reservation;
        private readonly string contactUrl;
        private readonly string businessImageUrl;

        public ComplaintApproved(Reservation reservation, string businessImageUrl, string contactUrl)
        {
            this.reservation = reservation;
            this.businessImageUrl = businessImageUrl;
            this.contactUrl = contactUrl;
        }

        public string Render(Template messageTemplate)
        {
            return messageTemplate.Render(new
            {
                reservation,
                businessImageUrl,
                contactUrl
            });
        }
    }

    
}
