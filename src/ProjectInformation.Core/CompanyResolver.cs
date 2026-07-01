namespace ProjectInformation.Core;

public static class CompanyResolver
{
    private static readonly IReadOnlyDictionary<string, string> DomainMappings =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["heijmans.nl"] = "Heijmans",
            ["homij.nl"] = "Homij",
            ["wfsadvies.nl"] = "WFS Advies",
            ["celsius.nl"] = "Celsius"
        };

    public static string Resolve(
        string? contactCompany,
        string? exchangeCompany,
        string? signatureCompany,
        string? emailAddress)
    {
        return FirstFilled(contactCompany, exchangeCompany, signatureCompany, FromEmailDomain(emailAddress));
    }

    public static string FromEmailDomain(string? emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            return string.Empty;
        }

        var atIndex = emailAddress.LastIndexOf('@');
        if (atIndex < 0 || atIndex == emailAddress.Length - 1)
        {
            return string.Empty;
        }

        var domain = emailAddress[(atIndex + 1)..].Trim().ToLowerInvariant();
        if (DomainMappings.TryGetValue(domain, out var mappedCompany))
        {
            return mappedCompany;
        }

        var firstLabel = domain.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstLabel))
        {
            return string.Empty;
        }

        return ToTitleCase(firstLabel);
    }

    private static string FirstFilled(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim() ?? string.Empty;
    }

    private static string ToTitleCase(string value)
    {
        if (value.Length == 0)
        {
            return string.Empty;
        }

        return string.Concat(value[..1].ToUpperInvariant(), value[1..]);
    }
}
