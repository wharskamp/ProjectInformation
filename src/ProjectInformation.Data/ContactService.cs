using ProjectInformation.Core;
using ProjectInformation.Core.Models;
using ProjectInformation.Core.Services;

namespace ProjectInformation.Data;

public sealed class ContactService : IContactService
{
    private readonly IPstImportService _pstImportService;
    private readonly ContactRepository _contactRepository;

    public ContactService(IPstImportService pstImportService, ContactRepository contactRepository)
    {
        _pstImportService = pstImportService;
        _contactRepository = contactRepository;
    }

    public async Task<IReadOnlyList<ContactRecord>> ImportContactsAsync(
        string pstFilePath,
        CancellationToken cancellationToken = default)
    {
        var mails = await _pstImportService.ReadMailsAsync(pstFilePath, null, cancellationToken);
        var contacts = ContactAggregator.BuildContacts(mails);
        await _contactRepository.ReplaceContactsAsync(contacts, cancellationToken);

        return contacts;
    }

    public Task<IReadOnlyList<ContactRecord>> GetContactsAsync(CancellationToken cancellationToken = default)
    {
        return _contactRepository.GetContactsAsync(cancellationToken);
    }
}
