using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IEmailService
{
    void Send(string to, string subject, string html, string? from = null);

    void Send(IEnumerable<string> to, string subject, string html, string? from = null);

    void SendWithTemeplate(string to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters);

    void SendWithTemeplate(IEnumerable<string> to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters);
}