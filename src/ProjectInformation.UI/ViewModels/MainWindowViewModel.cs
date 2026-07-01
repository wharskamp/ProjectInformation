using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ProjectInformation.Core.Models;
using ProjectInformation.Core.Services;

namespace ProjectInformation.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IContactService _contactService;
    private readonly ICsvExportService _csvExportService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportCommand))]
    private string? selectedPstFilePath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportCommand))]
    [NotifyCanExecuteChangedFor(nameof(ImportPstFileCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportCsvCommand))]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Ready.";

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string statisticsText = "0 contacts | 0 projects | 0 mails";

    public ObservableCollection<ContactRecord> Contacts { get; } = new();

    public ICollectionView ContactsView { get; }

    public MainWindowViewModel(IContactService contactService, ICsvExportService csvExportService)
    {
        _contactService = contactService;
        _csvExportService = csvExportService;
        ContactsView = CollectionViewSource.GetDefaultView(Contacts);
        ContactsView.Filter = FilterContact;
    }

    public async Task LoadContactsAsync()
    {
        try
        {
            IsBusy = true;
            var contacts = await _contactService.GetContactsAsync();
            ReplaceContacts(contacts);
            StatusMessage = contacts.Count > 0
                ? $"Loaded {contacts.Count} saved contacts."
                : "Ready.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SelectPstFile()
    {
        var filePath = SelectPstFilePath();
        if (filePath is not null)
        {
            SelectedPstFilePath = filePath;
            StatusMessage = "PST file selected.";
        }
    }

    [RelayCommand(CanExecute = nameof(CanImportPstFile))]
    private async Task ImportPstFileAsync()
    {
        var filePath = SelectPstFilePath();
        if (filePath is null)
        {
            return;
        }

        SelectedPstFilePath = filePath;
        await ImportAsync();
    }

    private static string? SelectPstFilePath()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select PST file",
            Filter = "Outlook PST files (*.pst)|*.pst|All files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    [RelayCommand(CanExecute = nameof(CanImport))]
    private async Task ImportAsync()
    {
        if (SelectedPstFilePath is null)
        {
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = "Reading PST file...";

            var contacts = await _contactService.ImportContactsAsync(SelectedPstFilePath);
            ReplaceContacts(contacts);

            ExportCsvCommand.NotifyCanExecuteChanged();
            StatusMessage = $"Import complete. {Contacts.Count} contacts loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanImport() => !IsBusy && !string.IsNullOrWhiteSpace(SelectedPstFilePath);

    private bool CanImportPstFile() => !IsBusy;

    [RelayCommand(CanExecute = nameof(DisabledCommand))]
    private void OpenContacts()
    {
    }

    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task ExportCsvAsync()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Export contacts to CSV",
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            FileName = "contacts.csv",
            AddExtension = true,
            DefaultExt = ".csv"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await _csvExportService.ExportContactsAsync(dialog.FileName, FilteredContacts().ToArray());
            StatusMessage = $"CSV exported to {dialog.FileName}.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanExport() => !IsBusy && FilteredContacts().Any();

    partial void OnSearchTextChanged(string value)
    {
        ContactsView.Refresh();
        ExportCsvCommand.NotifyCanExecuteChanged();
        UpdateStatistics();
    }

    private bool FilterContact(object value)
    {
        if (value is not ContactRecord contact)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return true;
        }

        return contact.Naam.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)
            || contact.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || contact.Projecten.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
    }

    private IEnumerable<ContactRecord> FilteredContacts()
    {
        return ContactsView.Cast<ContactRecord>();
    }

    private void ReplaceContacts(IReadOnlyList<ContactRecord> contacts)
    {
        Contacts.Clear();
        foreach (var contact in contacts)
        {
            Contacts.Add(contact);
        }

        ContactsView.Refresh();
        ExportCsvCommand.NotifyCanExecuteChanged();
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        var contacts = FilteredContacts().ToArray();
        var projectCount = contacts
            .SelectMany(contact => contact.Projecten.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();
        var mailCount = contacts.Sum(contact => contact.AantalMails);

        StatisticsText = $"Aantal contacten: {contacts.Length} | Aantal projecten: {projectCount} | Aantal mails: {mailCount}";
    }

    private static bool DisabledCommand() => false;
}
