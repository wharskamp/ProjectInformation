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
                    group.Count());
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
}
