using ProjectInformation.Core.Models;

namespace ProjectInformation.Core.Services;

public interface IPstImportService
{
    Task<ImportResult> ImportAsync(string pstFilePath, CancellationToken cancellationToken = default);
}
