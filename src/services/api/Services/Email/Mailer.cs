using API.Core.Model;
using MailKit.Net.Smtp;
using MimeKit;
using Scriban;

namespace API.Services.Email;

public class Mailer
{
    private readonly Dictionary<string, Template> _templates;

    public Mailer()
    {
        string templateDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Services/Email/Templates"
        );
        _templates = Directory.GetFiles(templateDir)
                              .ToDictionary(
                                  f => Path.GetFileName(f),
                                  f => Template.Parse(File.ReadAllText(f))
                              );
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
            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate(
                "isamrsadventure@gmail.com",
                "da438bd680f84f90a66a61eb2bee482b"
            );
            client.Send(email);
            client.Disconnect(true);
        });
    }
}
