using ProjectInformation.Core.Models;
using ProjectInformation.Core.Services;

namespace ProjectInformation.PstReader;

public sealed class PstImportService : IPstImportService
{
    public Task<ImportResult> ImportAsync(string pstFilePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pstFilePath);

        return Task.FromResult(new ImportResult(false, "PST import is not implemented yet."));
    }
}
