using ProjectInformation.Data;
using ProjectInformation.Export;
using ProjectInformation.PstReader;
using ProjectInformation.UI.ViewModels;
using System.Windows;

namespace ProjectInformation.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var databaseOptions = new DatabaseOptions
        {
            DatabasePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ProjectInformation",
                "projectinformation.sqlite")
        };

        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(databaseOptions.DatabasePath)!);

        var contactRepository = new ContactRepository(databaseOptions);
        var contactService = new ContactService(new PstImportService(), contactRepository);
        DataContext = new MainWindowViewModel(contactService, new CsvExportService());
    }

    private async void WindowLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.LoadContactsAsync();
        }
    }

    private void ContactsDataGridMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is System.Windows.Controls.DataGrid { SelectedItem: ProjectInformation.Core.Models.ContactRecord contact })
        {
            new ContactDetailWindow(contact) { Owner = this }.ShowDialog();
        }
    }

    private void ExitMenuItemClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void AboutMenuItemClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "ProjectInformation\nPractical contact manager for PST project contacts.",
            "Over ProjectInformation",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
