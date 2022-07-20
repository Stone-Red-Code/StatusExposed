using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string html, string? from = null);

    Task SendAsync(IEnumerable<string> to, string subject, string html, string? from = null);

    Task SendWithTemeplateAsync(string to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters);

    Task SendWithTemeplateAsync(IEnumerable<string> to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters);
}