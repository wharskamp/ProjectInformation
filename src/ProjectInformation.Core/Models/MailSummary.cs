namespace ProjectInformation.Core.Models;

public sealed record MailSummary(
    string SenderName,
    string SenderEmailAddress,
    string Subject,
    DateTime Date,
    string Company,
    string BusinessTelephoneNumber,
    string MobileTelephoneNumber,
    string JobTitle);
