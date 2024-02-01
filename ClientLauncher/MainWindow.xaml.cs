using System.Windows;
using ClientLauncher.Views;

namespace ClientLauncher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Content = new SettingsPage();
    }
}