using API.Core.Model;
using MailKit.Net.Smtp;
using MimeKit;
using Scriban;

namespace API.Services.Email;

public class Mailer
{
    private static readonly Action<ILogger, string, Guid, Exception> _emailDeliveryFailed = 
    LoggerMessage.Define<string, Guid>(
        LogLevel.Error,
        new EventId(0, "email-delivery-failed"), 
        "Delivering email '{EmailName}' to User with Id='{Id}' failed"
    );

    private readonly ILogger _logger;
    private readonly Dictionary<string, Template> _templates;

    public Mailer(ILogger<Mailer> logger)
    {
        string templateDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Services/Email/Templates"
        );
        _templates = Directory
            .GetFiles(templateDir)
            .ToDictionary(
                f => Path.GetFileName(f),
                f => Template.Parse(File.ReadAllText(f))
            );
        _logger = logger;
    }

    public void Send(User user, IMessage message)
    {
        var template = _templates.GetValueOrDefault(message.TemplateName);
        if (template is null) return;

        MimeMessage email = new();
        email.From.Add(new MailboxAddress("adventure.com", "noreply@adventure.com"));
        email.To.Add(new MailboxAddress(user.FullName, user.Email));
        email.Subject = message.Subject;
        email.Body = new TextPart("html")
        {
            Text = message.Render(template)
        };

        Task.Run(() =>
        {
            try
            {
                using var client = new SmtpClient();
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate(
                    "isamrsadventure@gmail.com",
                    "mihnbnibbhukszrp"
                );
                client.Send(email);
                client.Disconnect(true);
            }
            catch (Exception e)
            {
                _emailDeliveryFailed(_logger, message.Subject, user.Id, e);
            }
        });
    }
}
