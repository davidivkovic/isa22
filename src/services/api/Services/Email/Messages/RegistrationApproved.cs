using Scriban;

namespace API.Services.Email.Messages;

public class RegistrationApproved : IMessage
{
    public string TemplateName => "registration-approved.html";
    public string Subject => "Welcome to adventure.com";

    private readonly string name;
    private readonly string ctaUrl;

    public RegistrationApproved(string name, string ctaUrl)
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