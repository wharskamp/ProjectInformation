# ProjectInformation

ProjectInformation is a Windows desktop application foundation for importing project information from PST files, managing contacts, and exporting CSV data.

## Sprint 1 Foundation

- C# / .NET 8
- WPF UI
- MVVM with CommunityToolkit.Mvvm
- SQLite data layer foundation
- Separate Core, Data, PST reader, Export, UI, and Tests projects

## Sprint 2 Working Contact Import

- Select an Outlook PST file
- Read sender name, sender email address, subject, and date from all PST emails
- Detect project numbers in `P12345`, `P-12345`, and `Project 12345` formats
- Merge projects per contact
- Store imported contacts temporarily in SQLite
- Show contacts directly in a WPF DataGrid
- Export contacts to CSV

PST import uses Microsoft Outlook on the local machine to open and read PST files.

## Sprint 3 Practical Contact Manager

- Contacts are stored between sessions in a local SQLite database
- Multiple PST files can be imported after each other
- Contacts are merged by email address
- Search filters contacts in real time by name, email, and project number
- DataGrid columns are sortable
- Double-clicking a contact opens a detail window
- CSV export exports only the currently filtered contacts
- Status bar shows contact, project, and mail counts
- Application menu includes PST import, CSV export, exit, and about

## Sprint 4 Contact Enrichment

- Imports company, business phone, mobile phone, and job title
- Uses Outlook contact properties first when available
- Falls back to signature analysis for company and phone numbers
- Supports Dutch phone formats such as `06-12345678`, `+31 6 12345678`, `0318-123456`, and `088-1234567`
- Keeps the most complete contact information when merging imports
- Shows enrichment fields in SQLite, DataGrid, detail view, and CSV export

## Sprint 5 Contact Quality

- Resolves company names by priority: Outlook contact, Exchange user, signature, then email domain
- Adds a simple domain-to-company mapping for known domains
- Separates landline and mobile phone detection
- Keeps only the first detected business phone and mobile phone from signatures
- Merges contacts by email without overwriting existing values with empty data
- Stores unique alphabetically sorted project numbers
- Exports CSV as `Naam`, `Bedrijf`, `Email`, `Telefoon`, `Mobiel`, `Projecten`, `LaatsteContact`, `AantalMails`

## Projects

- `ProjectInformation.UI` - WPF application shell and MVVM view models
- `ProjectInformation.Core` - shared domain models and service contracts
- `ProjectInformation.Data` - SQLite initialization foundation
- `ProjectInformation.PstReader` - Outlook-backed PST import service
- `ProjectInformation.Export` - CSV export service
- `ProjectInformation.Tests` - core processing tests

## Build

Install the .NET 8 SDK and run:

```powershell
dotnet build ProjectInformation.sln
```
