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

    public void Send(string to, string subject, string html, string? from = null)
    {
        // create message
        MimeMessage email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ?? mailOptions.DefaultEmailAddress));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        // send email
        using SmtpClient smtp = new SmtpClient();
        smtp.Connect(mailOptions.SmtpServer, mailOptions.SmtpPort, SecureSocketOptions.StartTls);
        smtp.Authenticate(mailOptions.SmtpUser, mailOptions.SmtpPassword);
        smtp.Send(email);
        smtp.Disconnect(true);
    }

    public void Send(IEnumerable<string> to, string subject, string html, string? from = null)
    {
        // create message
        MimeMessage email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ?? mailOptions.DefaultEmailAddress));
        email.Bcc.AddRange(to.Select(t => MailboxAddress.Parse(t)));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        // send email
        using SmtpClient smtp = new SmtpClient();
        smtp.Connect(mailOptions.SmtpServer, mailOptions.SmtpPort, SecureSocketOptions.StartTls);
        smtp.Authenticate(mailOptions.SmtpUser, mailOptions.SmtpPassword);
        smtp.Send(email);
        smtp.Disconnect(true);
    }

    public void SendWithTemeplate(string to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters)
    {
        string html = ProcessEmailTemplate(templatePath, templateParameters);
        Send(to, subject, html, from);
    }

    public void SendWithTemeplate(IEnumerable<string> to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters)
    {
        string html = ProcessEmailTemplate(templatePath, templateParameters);
        Send(to, subject, html, from);
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
}