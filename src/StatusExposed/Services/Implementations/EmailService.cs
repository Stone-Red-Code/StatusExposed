using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Options;

using MimeKit;
using MimeKit.Text;

using StatusExposed.Models;
using StatusExposed.Models.Options;

namespace StatusExposed.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly EmailOptions mailOptions;

    public EmailService(IOptions<EmailOptions> mailOptions)
    {
        this.mailOptions = mailOptions.Value;
    }

    public async Task SendAsync(string to, string subject, string html, string? from = null)
    {
        // create message
        MimeMessage email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ?? mailOptions.DefaultEmailAddress));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        // send email
        await SendMessageAsync(email);
    }

    public async Task SendAsync(IEnumerable<string> to, string subject, string html, string? from = null)
    {
        // create message
        MimeMessage email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ?? mailOptions.DefaultEmailAddress));
        email.Bcc.AddRange(to.Select(t => MailboxAddress.Parse(t)));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        // send email
        await SendMessageAsync(email);
    }

    public async Task SendWithTemeplateAsync(string to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters)
    {
        string html = ProcessEmailTemplate(templatePath, templateParameters);
        await SendAsync(to, subject, html, from);
    }

    public async Task SendWithTemeplateAsync(IEnumerable<string> to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters)
    {
        string html = ProcessEmailTemplate(templatePath, templateParameters);
        await SendAsync(to, subject, html, from);
    }

    private static string ProcessEmailTemplate(string templatePath, params TemplateParameter[] templateParameters)
    {
        string html = File.ReadAllText(templatePath);
        foreach (TemplateParameter templateParameter in templateParameters)
        {
            html = html.Replace($"{{{templateParameter.Name}}}", templateParameter.Value);
        }

        return html;
    }

    private async Task SendMessageAsync(MimeMessage email)
    {
        using SmtpClient smtp = new SmtpClient();
        await smtp.ConnectAsync(mailOptions.SmtpServer, mailOptions.SmtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(mailOptions.SmtpUser, mailOptions.SmtpPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}