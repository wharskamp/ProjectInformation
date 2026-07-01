namespace ProjectInformation.Core.Models;

public sealed record MailSummary(
    string SenderName,
    string SenderEmailAddress,
    string Subject,
    DateTime Date);
