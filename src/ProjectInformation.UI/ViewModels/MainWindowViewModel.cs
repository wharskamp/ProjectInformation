using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ProjectInformation.Core.Services;

namespace ProjectInformation.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IPstImportService _pstImportService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportCommand))]
    private string? selectedPstFilePath;

    [ObservableProperty]
    private string statusMessage = "Ready.";

    public MainWindowViewModel(IPstImportService pstImportService)
    {
        _pstImportService = pstImportService;
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

        StatusMessage = "Import not implemented yet.";
        await _pstImportService.ImportAsync(SelectedPstFilePath);
    }

    private bool CanImport() => !string.IsNullOrWhiteSpace(SelectedPstFilePath);

    [RelayCommand(CanExecute = nameof(DisabledCommand))]
    private void OpenContacts()
    {
    }

    [RelayCommand(CanExecute = nameof(DisabledCommand))]
    private void ExportCsv()
    {
    }

    private static bool DisabledCommand() => false;
}
