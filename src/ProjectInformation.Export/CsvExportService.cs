using ProjectInformation.Core.Services;

namespace ProjectInformation.Export;

public sealed class CsvExportService : ICsvExportService
{
    public Task ExportContactsAsync(string destinationFilePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationFilePath);

        throw new NotImplementedException("CSV export is not implemented yet.");
    }
}
