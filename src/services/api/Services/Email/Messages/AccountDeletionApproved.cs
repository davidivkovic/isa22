using Scriban;

namespace API.Services.Email.Messages;

public class AccountDeletionApproved : IMessage
{
    public string TemplateName => "delete-request-approved.html";
    public string Subject => "Goodbye. adventure.com";

    private readonly string name;
    private readonly string ctaUrl;

    public AccountDeletionApproved(string name, string ctaUrl)
    {
        this.name = name;
        this.ctaUrl = ctaUrl;
    }

    public string Render(Template messageTemplate)
    {
        return messageTemplate.Render(new
        {
            name,
            ctaUrl
        });
    }
}