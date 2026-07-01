using Microsoft.Data.Sqlite;

namespace ProjectInformation.Data;

public sealed class ProjectInformationDbInitializer
{
    private readonly DatabaseOptions _options;

    public ProjectInformationDbInitializer(DatabaseOptions options)
    {
        _options = options;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = _options.DatabasePath
        }.ToString();

        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Contacts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Naam TEXT NOT NULL,
                Email TEXT NOT NULL,
                Projecten TEXT NOT NULL,
                LaatsteContact TEXT NOT NULL,
                AantalMails INTEGER NOT NULL,
                Company TEXT NOT NULL DEFAULT '',
                BusinessTelephoneNumber TEXT NOT NULL DEFAULT '',
                MobileTelephoneNumber TEXT NOT NULL DEFAULT '',
                JobTitle TEXT NOT NULL DEFAULT ''
            );
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);

        foreach (var column in new[] { "Company", "BusinessTelephoneNumber", "MobileTelephoneNumber", "JobTitle" })
        {
            await using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = $"ALTER TABLE Contacts ADD COLUMN {column} TEXT NOT NULL DEFAULT '';";

            try
            {
                await alterCommand.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 1)
            {
            }
        }
    }
}
