namespace StatusExposed.Utilities;

public static class StringExtensions
{
    public static TimeSpan ToTimeSpan(this string timeSpan)
    {
        int l = timeSpan.Length - 1;
        string? value = timeSpan[..l];
        string? type = timeSpan.Substring(l, 1);

        return type switch
        {
            "d" => TimeSpan.FromDays(double.Parse(value)),
            "h" => TimeSpan.FromHours(double.Parse(value)),
            "m" => TimeSpan.FromMinutes(double.Parse(value)),
            "s" => TimeSpan.FromSeconds(double.Parse(value)),
            _ => throw new FormatException($"{timeSpan} can't be converted to TimeSpan, unknown type {type}"),
        };
    }
}