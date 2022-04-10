using Scriban;

namespace API.Services.Email;

public interface IMessage
{
    public string TemplateName { get; }
    public string Subject { get; }
    public string Render(Template messageTemplate);
}