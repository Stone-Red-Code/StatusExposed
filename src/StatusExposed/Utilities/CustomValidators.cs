using Blazorise;

using System.Text.RegularExpressions;

namespace StatusExposed.Utilities;

public static class CustomValidators
{
    public static void ValidateDomain(ValidatorEventArgs e)
    {
        string? domain = e.Value?.ToString();

        if (string.IsNullOrEmpty(domain))
        {
            e.Status = ValidationStatus.Error;
            return;
        }

        if (Regex.IsMatch(domain, @"(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]"))
        {
            e.Status = ValidationStatus.Success;
            return;
        }

        e.Status = ValidationStatus.Error;
    }

    public static void ValidateUrl(ValidatorEventArgs e)
    {
        string? url = e.Value?.ToString();

        if (string.IsNullOrWhiteSpace(url))
        {
            e.Status = ValidationStatus.None;
            return;
        }

        if (Regex.IsMatch(url, @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$", RegexOptions.None, TimeSpan.FromSeconds(10)) && url.Contains('.'))
        {
            e.Status = ValidationStatus.Success;
            return;
        }

        e.Status = ValidationStatus.Error;
    }

    public static void ValidatePermission(ValidatorEventArgs e)
    {
        string? permission = e.Value?.ToString();

        if (string.IsNullOrWhiteSpace(permission))
        {
            e.Status = ValidationStatus.Error;
            return;
        }

        if (Regex.IsMatch(permission, @"^[a-z0-9]{2,}[:]{1}.{1,}", RegexOptions.None, TimeSpan.FromSeconds(10)))
        {
            e.Status = ValidationStatus.Success;
            return;
        }

        e.Status = ValidationStatus.Error;
    }
}