using Microsoft.Data.Sqlite;
using ProjectInformation.Core.Models;

namespace ProjectInformation.Data;

public sealed class ContactRepository
{
    private readonly DatabaseOptions _options;
    private readonly ProjectInformationDbInitializer _initializer;

    public ContactRepository(DatabaseOptions options)
    {
        _options = options;
        _initializer = new ProjectInformationDbInitializer(options);
    }

    public async Task ReplaceContactsAsync(IReadOnlyList<ContactRecord> contacts, CancellationToken cancellationToken = default)
    {
        await _initializer.InitializeAsync(cancellationToken);

        await using var connection = new SqliteConnection(BuildConnectionString());
        await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await using (var deleteCommand = connection.CreateCommand())
        {
            deleteCommand.Transaction = (SqliteTransaction)transaction;
            deleteCommand.CommandText = "DELETE FROM Contacts;";
            await deleteCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        foreach (var contact in contacts)
        {
            await using var insertCommand = connection.CreateCommand();
            insertCommand.Transaction = (SqliteTransaction)transaction;
            insertCommand.CommandText = """
                INSERT INTO Contacts (
                    Naam,
                    Email,
                    Projecten,
                    LaatsteContact,
                    AantalMails,
                    Company,
                    BusinessTelephoneNumber,
                    MobileTelephoneNumber,
                    JobTitle)
                VALUES (
                    $naam,
                    $email,
                    $projecten,
                    $laatsteContact,
                    $aantalMails,
                    $company,
                    $businessTelephoneNumber,
                    $mobileTelephoneNumber,
                    $jobTitle);
                """;
            insertCommand.Parameters.AddWithValue("$naam", contact.Naam);
            insertCommand.Parameters.AddWithValue("$email", contact.Email);
            insertCommand.Parameters.AddWithValue("$projecten", contact.Projecten);
            insertCommand.Parameters.AddWithValue("$laatsteContact", contact.LaatsteContact.ToString("O"));
            insertCommand.Parameters.AddWithValue("$aantalMails", contact.AantalMails);
            insertCommand.Parameters.AddWithValue("$company", contact.Company);
            insertCommand.Parameters.AddWithValue("$businessTelephoneNumber", contact.BusinessTelephoneNumber);
            insertCommand.Parameters.AddWithValue("$mobileTelephoneNumber", contact.MobileTelephoneNumber);
            insertCommand.Parameters.AddWithValue("$jobTitle", contact.JobTitle);

            await insertCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContactRecord>> GetContactsAsync(CancellationToken cancellationToken = default)
    {
        await _initializer.InitializeAsync(cancellationToken);

        var contacts = new List<ContactRecord>();

        await using var connection = new SqliteConnection(BuildConnectionString());
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT
                Naam,
                Email,
                Projecten,
                LaatsteContact,
                AantalMails,
                Company,
                BusinessTelephoneNumber,
                MobileTelephoneNumber,
                JobTitle
            FROM Contacts
            ORDER BY Naam, Email;
            """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            contacts.Add(new ContactRecord(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                DateTime.Parse(reader.GetString(3)),
                reader.GetInt32(4),
                reader.GetString(5),
                reader.GetString(6),
                reader.GetString(7),
                reader.GetString(8)));
        }

        return contacts;
    }

    private string BuildConnectionString()
    {
        return new SqliteConnectionStringBuilder
        {
            DataSource = _options.DatabasePath
        }.ToString();
    }
}
