using Scriban;

namespace API.Services.Email.Messages;

public class ConfirmEmail : IMessage
{
    public string TemplateName => "confirm-email.html";
    public string Subject => "Please confirm your email";

    private readonly string name;
    private readonly string code;

    public ConfirmEmail(string name, string code)
    {
        this.name = name;
        this.code = code;
    }

    public string Render(Template t)
    {
        return t.Render(new 
        { 
            name,
            code 
        });
    }
}