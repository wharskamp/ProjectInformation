# ProjectInformation

ProjectInformation is a Windows desktop application foundation for importing project information from PST files, managing contacts, and exporting CSV data.

## Sprint 1 Foundation

- C# / .NET 8
- WPF UI
- MVVM with CommunityToolkit.Mvvm
- SQLite data layer foundation
- Separate Core, Data, PST reader, Export, UI, and Tests projects

The PST reader is intentionally not implemented in Sprint 1. The application currently provides the project structure and a start window with disabled follow-up actions.

## Projects

- `ProjectInformation.UI` - WPF application shell and MVVM view models
- `ProjectInformation.Core` - shared domain models and service contracts
- `ProjectInformation.Data` - SQLite initialization foundation
- `ProjectInformation.PstReader` - PST import service placeholder
- `ProjectInformation.Export` - CSV export service placeholder
- `ProjectInformation.Tests` - foundation tests

## Build

Install the .NET 8 SDK and run:

```powershell
dotnet build ProjectInformation.sln
```
