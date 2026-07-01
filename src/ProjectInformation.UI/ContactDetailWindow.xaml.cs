using ProjectInformation.Core.Models;
using System.Windows;

namespace ProjectInformation.UI;

public partial class ContactDetailWindow : Window
{
    public ContactDetailWindow(ContactRecord contact)
    {
        InitializeComponent();
        DataContext = contact;
    }
}
