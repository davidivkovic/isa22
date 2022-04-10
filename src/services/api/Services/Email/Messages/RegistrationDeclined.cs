using Scriban;

namespace API.Services.Email.Messages;

public class RegistrationDeclined : IMessage
{
    public string TemplateName => "registration-declined.html";
    public string Subject => "Your registration was declined";

    private readonly string name;
    private readonly string reason;
    private readonly string contactUrl;

    public RegistrationDeclined(string name, string reason, string contactUrl)
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