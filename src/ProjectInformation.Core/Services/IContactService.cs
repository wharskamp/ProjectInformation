using ProjectInformation.Core.Models;

namespace ProjectInformation.Core.Services;

public interface IContactService
{
    Task<IReadOnlyList<ContactRecord>> ImportContactsAsync(
        string pstFilePath,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactRecord>> GetContactsAsync(CancellationToken cancellationToken = default);
}
