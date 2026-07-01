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
                System.IO.Path.GetTempPath(),
                "ProjectInformation",
                "projectinformation.sqlite")
        };

        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(databaseOptions.DatabasePath)!);

        var contactRepository = new ContactRepository(databaseOptions);
        var contactService = new ContactService(new PstImportService(), contactRepository);
        DataContext = new MainWindowViewModel(contactService, new CsvExportService());
    }
}
