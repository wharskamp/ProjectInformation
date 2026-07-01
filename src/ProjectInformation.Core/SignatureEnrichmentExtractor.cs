using System.Text.RegularExpressions;

namespace ProjectInformation.Core;

public sealed record SignatureEnrichment(
    string Company,
    string BusinessTelephoneNumber,
    string MobileTelephoneNumber);

public static partial class SignatureEnrichmentExtractor
{
    public static SignatureEnrichment Extract(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return new SignatureEnrichment(string.Empty, string.Empty, string.Empty);
        }

        var signature = LastLines(body, 18);
        var mobile = FindMobileNumber(signature);
        var business = FindBusinessTelephoneNumber(signature, mobile);
        var company = FindCompany(signature);

        return new SignatureEnrichment(company, business, mobile);
    }

    private static string FindMobileNumber(string signature)
    {
        return MobilePattern().Matches(signature)
            .Select(match => match.Value.Trim())
            .FirstOrDefault() ?? string.Empty;
    }

    private static string FindBusinessTelephoneNumber(string signature, string mobile)
    {
        return LandlinePattern().Matches(signature)
            .Select(match => match.Value.Trim())
            .FirstOrDefault()
            ?? string.Empty;
    }

    private static string FindCompany(string signature)
    {
        var lines = signature
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => line.Length is > 1 and < 80)
            .Where(line => !line.Contains('@'))
            .Where(line => !LandlinePattern().IsMatch(line))
            .Where(line => !MobilePattern().IsMatch(line))
            .Where(line => !line.StartsWith("Tel", StringComparison.OrdinalIgnoreCase))
            .Where(line => !line.StartsWith("Mob", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return lines.FirstOrDefault(line => CompanyPattern().IsMatch(line))
            ?? lines.Skip(1).FirstOrDefault()
            ?? string.Empty;
    }

    private static string LastLines(string body, int count)
    {
        var lines = body
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        return string.Join(Environment.NewLine, lines.TakeLast(count));
    }

    [GeneratedRegex(@"(?<!\d)(?:06[-\s]?\d{8}|06\s?\d{8}|\+31\s?6\s?\d{8}|\+31\s?6\s?\d{4}\s?\d{4})(?!\d)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MobilePattern();

    [GeneratedRegex(@"(?<!\d)(?:0[1-9]\d{1,2}[-\s]?\d{6,7})(?!\d)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex LandlinePattern();

    [GeneratedRegex(@"\b(?:B\.?V\.?|N\.?V\.?|Ltd\.?|LLC|Inc\.?|GmbH|Company|Celsius)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex CompanyPattern();
}
