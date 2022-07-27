namespace StatusExposed.Models.Options;

public class EmailSettings
{
    public string? DefaultEmailAddress { get; set; }
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string? SmtpUser { get; set; }
    public string? SmtpPassword { get; set; }
    public EmailTemplatePaths? TemplatePaths { get; set; }
}