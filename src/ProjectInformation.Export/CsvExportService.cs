using ProjectInformation.Core.Services;
using ProjectInformation.Core.Models;
using System.Globalization;
using System.Text;

namespace ProjectInformation.Export;

public sealed class CsvExportService : ICsvExportService
{
    public async Task ExportContactsAsync(
        string destinationFilePath,
        IReadOnlyList<ContactRecord> contacts,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationFilePath);

        var builder = new StringBuilder();
        builder.AppendLine("Naam,Email,Projecten,LaatsteContact,AantalMails");

        foreach (var contact in contacts)
        {
            builder.Append(Csv(contact.Naam)).Append(',');
            builder.Append(Csv(contact.Email)).Append(',');
            builder.Append(Csv(contact.Projecten)).Append(',');
            builder.Append(Csv(contact.LaatsteContact.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))).Append(',');
            builder.AppendLine(contact.AantalMails.ToString(CultureInfo.InvariantCulture));
        }

        await File.WriteAllTextAsync(destinationFilePath, builder.ToString(), Encoding.UTF8, cancellationToken);
    }

    private static string Csv(string value)
    {
        if (!value.Contains(',') && !value.Contains('"') && !value.Contains('\n') && !value.Contains('\r'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
