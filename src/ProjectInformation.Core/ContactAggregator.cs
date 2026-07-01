using ProjectInformation.Core.Models;

namespace ProjectInformation.Core;

public static class ContactAggregator
{
    public static IReadOnlyList<ContactRecord> BuildContacts(IEnumerable<MailSummary> mails)
    {
        return mails
            .Where(mail => !string.IsNullOrWhiteSpace(mail.SenderName) || !string.IsNullOrWhiteSpace(mail.SenderEmailAddress))
            .GroupBy(mail => ContactKey(mail), StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var first = group.First();
                var projects = group
                    .SelectMany(mail => ProjectNumberExtractor.Extract(mail.Subject))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Order(StringComparer.OrdinalIgnoreCase);

                return new ContactRecord(
                    first.SenderName.Trim(),
                    first.SenderEmailAddress.Trim(),
                    string.Join("; ", projects),
                    group.Max(mail => mail.Date),
                    group.Count(),
                    BestValue(group.Select(mail => mail.Company)),
                    BestValue(group.Select(mail => mail.BusinessTelephoneNumber)),
                    BestValue(group.Select(mail => mail.MobileTelephoneNumber)),
                    BestValue(group.Select(mail => mail.JobTitle)));
            })
            .OrderBy(contact => contact.Naam, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(contact => contact.Email, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static IReadOnlyList<ContactRecord> MergeContacts(
        IEnumerable<ContactRecord> existingContacts,
        IEnumerable<ContactRecord> importedContacts)
    {
        return existingContacts
            .Concat(importedContacts)
            .GroupBy(ContactKey, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var latest = group.OrderByDescending(contact => contact.LaatsteContact).First();
                var projects = group
                    .SelectMany(contact => contact.Projecten.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Order(StringComparer.OrdinalIgnoreCase);

                return new ContactRecord(
                    BestValue(group.Select(contact => contact.Naam), latest.Naam),
                    BestValue(group.Select(contact => contact.Email), latest.Email),
                    string.Join("; ", projects),
                    group.Max(contact => contact.LaatsteContact),
                    group.Sum(contact => contact.AantalMails),
                    BestValue(group.Select(contact => contact.Company)),
                    BestValue(group.Select(contact => contact.BusinessTelephoneNumber)),
                    BestValue(group.Select(contact => contact.MobileTelephoneNumber)),
                    BestValue(group.Select(contact => contact.JobTitle)));
            })
            .OrderBy(contact => contact.Naam, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(contact => contact.Email, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string ContactKey(MailSummary mail)
    {
        if (!string.IsNullOrWhiteSpace(mail.SenderEmailAddress))
        {
            return mail.SenderEmailAddress.Trim();
        }

        return mail.SenderName.Trim();
    }

    private static string ContactKey(ContactRecord contact)
    {
        if (!string.IsNullOrWhiteSpace(contact.Email))
        {
            return contact.Email.Trim();
        }

        return contact.Naam.Trim();
    }

    private static string BestValue(IEnumerable<string> values, string fallback = "")
    {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .OrderByDescending(value => value.Trim().Length)
            .Select(value => value.Trim())
            .FirstOrDefault()
            ?? fallback;
    }
}
