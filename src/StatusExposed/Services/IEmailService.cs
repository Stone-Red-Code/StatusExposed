using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IEmailService
{
    /// <summary>
    /// Sends an E-mail to a user.
    /// </summary>
    /// <param name="to">The E-mail address of the user.</param>
    /// <param name="subject">The subject of the E-mail.</param>
    /// <param name="html">The HTML content of the E-mail.</param>
    /// <param name="from">The origin E-mail address.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task SendAsync(string to, string subject, string html, string? from = null);

    /// <summary>
    /// Sends an E-mail to multiple users.
    /// </summary>
    /// <param name="to">The E-mail addresses of the users.</param>
    /// <param name="subject">The subject of the E-mail.</param>
    /// <param name="html">The HTML content of the E-mail.</param>
    /// <param name="from">The origin E-mail address.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task SendAsync(IEnumerable<string> to, string subject, string html, string? from = null);

    /// <summary>
    /// Sends an E-mail to a user and replaces the parameters in the template with the ones specified.
    /// </summary>
    /// <param name="to">The E-mail address of the user.</param>
    /// <param name="subject">The subject of the E-mail.</param>
    /// <param name="templatePath">The HTML template file path of the E-mail.</param>
    /// <param name="templateParameters">The parameters to be replaced in the E-mail template.</param>
    /// <param name="from">The origin E-mail address.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task SendWithTemeplateAsync(string to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters);

    /// <summary>
    /// Sends an E-mail to multiple users and replaces the parameters in the template with the ones specified.
    /// </summary>
    /// <param name="to">The E-mail addresses of the users.</param>
    /// <param name="subject">The subject of the E-mail.</param>
    /// <param name="templatePath">The HTML template file path of the E-mail.</param>
    /// <param name="from">The origin E-mail address.</param>
    /// <param name="templateParameters">The parameters to be replaced in the E-mail template.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task SendWithTemeplateAsync(IEnumerable<string> to, string subject, string templatePath, string? from = null, params TemplateParameter[] templateParameters);
}