namespace ProjectInformation.Core.Models;

public sealed record ContactRecord(
    string Naam,
    string Email,
    string Projecten,
    DateTime LaatsteContact,
    int AantalMails);
