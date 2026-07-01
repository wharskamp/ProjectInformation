using ProjectInformation.Core.Models;

namespace ProjectInformation.Core.Services;

public interface IPstImportService
{
    Task<IReadOnlyList<MailSummary>> ReadMailsAsync(
        string pstFilePath,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);
}
