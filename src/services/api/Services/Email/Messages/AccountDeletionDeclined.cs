using Scriban;

namespace API.Services.Email.Messages;

public class AccountDeletionDeclined : IMessage
{
    public string TemplateName => "delete-request-declined.html";
    public string Subject => "We couldn't delete your account";

    private readonly string name;
    private readonly string reason;
    private readonly string contactUrl;

    public AccountDeletionDeclined(string name, string reason, string contactUrl)
    {
        this.name = name;
        this.reason = reason;
        this.contactUrl = contactUrl;
    }

    public string Render(Template messageTemplate)
    {
        return messageTemplate.Render(new
        {
            name,
            reason,
            contactUrl
        });
    }
}