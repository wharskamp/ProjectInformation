using System.Collections.ObjectModel;
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
    [NotifyCanExecuteChangedFor(nameof(ExportCsvCommand))]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Ready.";

    public ObservableCollection<ContactRecord> Contacts { get; } = new();

    public MainWindowViewModel(IContactService contactService, ICsvExportService csvExportService)
    {
        _contactService = contactService;
        _csvExportService = csvExportService;
    }

    [RelayCommand]
    private void SelectPstFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select PST file",
            Filter = "Outlook PST files (*.pst)|*.pst|All files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            SelectedPstFilePath = dialog.FileName;
            StatusMessage = "PST file selected.";
        }
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
            Contacts.Clear();
            foreach (var contact in contacts)
            {
                Contacts.Add(contact);
            }

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
            await _csvExportService.ExportContactsAsync(dialog.FileName, Contacts.ToArray());
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

    private bool CanExport() => !IsBusy && Contacts.Count > 0;

    private static bool DisabledCommand() => false;
}
