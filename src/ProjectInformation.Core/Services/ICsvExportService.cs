namespace ProjectInformation.Core.Services;

public interface ICsvExportService
{
    Task ExportContactsAsync(string destinationFilePath, CancellationToken cancellationToken = default);
}
