using ProjectInformation.Core.Models;

namespace ProjectInformation.Core.Services;

public interface ICsvExportService
{
    Task ExportContactsAsync(
        string destinationFilePath,
        IReadOnlyList<ContactRecord> contacts,
        CancellationToken cancellationToken = default);
}
