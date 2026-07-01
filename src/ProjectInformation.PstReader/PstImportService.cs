using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using ProjectInformation.Core;
using ProjectInformation.Core.Models;
using ProjectInformation.Core.Services;

namespace ProjectInformation.PstReader;

public sealed class PstImportService : IPstImportService
{
    public Task<IReadOnlyList<MailSummary>> ReadMailsAsync(
        string pstFilePath,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pstFilePath);

        if (!File.Exists(pstFilePath))
        {
            throw new FileNotFoundException("The selected PST file could not be found.", pstFilePath);
        }

        var completion = new TaskCompletionSource<IReadOnlyList<MailSummary>>();
        var thread = new Thread(() =>
        {
            try
            {
                completion.SetResult(ReadMailsOnStaThread(pstFilePath, progress, cancellationToken));
            }
            catch (Exception ex)
            {
                completion.SetException(ex);
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return completion.Task;
    }

    private static IReadOnlyList<MailSummary> ReadMailsOnStaThread(
        string pstFilePath,
        IProgress<int>? progress,
        CancellationToken cancellationToken)
    {
        var outlookType = Type.GetTypeFromProgID("Outlook.Application")
            ?? throw new InvalidOperationException("Microsoft Outlook is required to import PST files.");

        dynamic? outlook = null;
        dynamic? session = null;
        dynamic? store = null;
        dynamic? rootFolder = null;

        try
        {
            outlook = Activator.CreateInstance(outlookType)
                ?? throw new InvalidOperationException("Could not start Microsoft Outlook.");
            session = outlook.GetNamespace("MAPI");

            var storeCountBefore = session.Stores.Count;
            session.AddStore(pstFilePath);
            store = session.Stores.Item(storeCountBefore + 1);
            rootFolder = store.GetRootFolder();

            var mails = new ConcurrentBag<MailSummary>();
            ReadFolder(rootFolder, mails, progress, cancellationToken);

            var result = mails.OrderByDescending(mail => mail.Date).ToArray();
            try
            {
                session.RemoveStore(rootFolder);
            }
            catch (COMException)
            {
            }

            return result;
        }
        finally
        {
            ReleaseComObject(rootFolder);
            ReleaseComObject(store);
            ReleaseComObject(session);
            ReleaseComObject(outlook);
        }
    }

    private static void ReadFolder(
        dynamic folder,
        ConcurrentBag<MailSummary> mails,
        IProgress<int>? progress,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        dynamic? items = null;
        dynamic? subFolders = null;

        try
        {
            items = folder.Items;

            for (var index = 1; index <= items.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                dynamic? item = null;
                try
                {
                    item = items.Item(index);
                    if (item.Class == 43)
                    {
                        var contactEnrichment = GetContactEnrichment(item);
                        var exchangeCompany = GetExchangeCompany(item);
                        var signatureEnrichment = SignatureEnrichmentExtractor.Extract(SafeString(item.Body));
                        var senderEmailAddress = GetSenderEmailAddress(item);

                        mails.Add(new MailSummary(
                            SafeString(item.SenderName),
                            senderEmailAddress,
                            SafeString(item.Subject),
                            SafeDate(item.ReceivedTime, item.SentOn),
                            CompanyResolver.Resolve(contactEnrichment.Company, exchangeCompany, signatureEnrichment.Company, senderEmailAddress),
                            FirstFilled(contactEnrichment.BusinessTelephoneNumber, signatureEnrichment.BusinessTelephoneNumber),
                            FirstFilled(contactEnrichment.MobileTelephoneNumber, signatureEnrichment.MobileTelephoneNumber),
                            contactEnrichment.JobTitle));
                        progress?.Report(mails.Count);
                    }
                }
                catch (COMException)
                {
                }
                finally
                {
                    ReleaseComObject(item);
                }
            }

            subFolders = folder.Folders;
            for (var index = 1; index <= subFolders.Count; index++)
            {
                dynamic? subFolder = null;
                try
                {
                    subFolder = subFolders.Item(index);
                    ReadFolder(subFolder, mails, progress, cancellationToken);
                }
                finally
                {
                    ReleaseComObject(subFolder);
                }
            }
        }
        finally
        {
            ReleaseComObject(subFolders);
            ReleaseComObject(items);
        }
    }

    private static string GetSenderEmailAddress(dynamic item)
    {
        try
        {
            var senderEmailType = SafeString(item.SenderEmailType);
            if (senderEmailType.Equals("EX", StringComparison.OrdinalIgnoreCase))
            {
                dynamic? sender = null;
                dynamic? exchangeUser = null;

                try
                {
                    sender = item.Sender;
                    exchangeUser = sender?.GetExchangeUser();
                    var primarySmtpAddress = SafeString(exchangeUser?.PrimarySmtpAddress);
                    if (!string.IsNullOrWhiteSpace(primarySmtpAddress))
                    {
                        return primarySmtpAddress;
                    }
                }
                finally
                {
                    ReleaseComObject(exchangeUser);
                    ReleaseComObject(sender);
                }
            }

            return SafeString(item.SenderEmailAddress);
        }
        catch (COMException)
        {
            return string.Empty;
        }
    }

    private static ContactEnrichment GetContactEnrichment(dynamic item)
    {
        dynamic? sender = null;
        dynamic? contact = null;

        try
        {
            sender = item.Sender;
            contact = sender?.GetContact();

            if (contact is null)
            {
                return ContactEnrichment.Empty;
            }

            return new ContactEnrichment(
                SafeString(contact.CompanyName),
                SafeString(contact.BusinessTelephoneNumber),
                SafeString(contact.MobileTelephoneNumber),
                SafeString(contact.JobTitle));
        }
        catch (COMException)
        {
            return ContactEnrichment.Empty;
        }
        finally
        {
            ReleaseComObject(contact);
            ReleaseComObject(sender);
        }
    }

    private static string GetExchangeCompany(dynamic item)
    {
        dynamic? sender = null;
        dynamic? exchangeUser = null;

        try
        {
            sender = item.Sender;
            exchangeUser = sender?.GetExchangeUser();

            return SafeString(exchangeUser?.CompanyName);
        }
        catch (COMException)
        {
            return string.Empty;
        }
        finally
        {
            ReleaseComObject(exchangeUser);
            ReleaseComObject(sender);
        }
    }

    private static string FirstFilled(params string[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim() ?? string.Empty;
    }

    private static DateTime SafeDate(dynamic primary, dynamic fallback)
    {
        try
        {
            if (primary is DateTime primaryDate)
            {
                return primaryDate;
            }

            if (fallback is DateTime fallbackDate)
            {
                return fallbackDate;
            }
        }
        catch (COMException)
        {
        }

        return DateTime.MinValue;
    }

    private static string SafeString(dynamic? value)
    {
        try
        {
            return value?.ToString() ?? string.Empty;
        }
        catch (COMException)
        {
            return string.Empty;
        }
    }

    private static void ReleaseComObject(object? value)
    {
        if (value is not null && Marshal.IsComObject(value))
        {
            Marshal.FinalReleaseComObject(value);
        }
    }

    private sealed record ContactEnrichment(
        string Company,
        string BusinessTelephoneNumber,
        string MobileTelephoneNumber,
        string JobTitle)
    {
        public static ContactEnrichment Empty { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty);
    }
}
