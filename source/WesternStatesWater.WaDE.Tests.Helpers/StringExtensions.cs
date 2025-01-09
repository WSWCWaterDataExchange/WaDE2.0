using System.Text.RegularExpressions;

namespace WesternStatesWater.WaDE.Tests.Helpers;

public static class StringExtensions
{
    public static string PascalToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return Regex.Replace(
                value,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])",
                "-$1",
                RegexOptions.Compiled)
            .Trim()
            .ToLower();
    }
}