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
