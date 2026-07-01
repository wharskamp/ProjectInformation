using System.Text.RegularExpressions;

namespace ProjectInformation.Core;

public static partial class ProjectNumberExtractor
{
    public static IReadOnlyList<string> Extract(string? subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            return Array.Empty<string>();
        }

        return ProjectNumberPattern()
            .Matches(subject)
            .Select(match => $"P{match.Groups["number"].Value}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    [GeneratedRegex(@"\b(?:P-?|Project\s+)(?<number>\d{5})\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ProjectNumberPattern();
}
