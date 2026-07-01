using ProjectInformation.PstReader;
using ProjectInformation.UI.ViewModels;
using System.Windows;

namespace ProjectInformation.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(new PstImportService());
    }
}
