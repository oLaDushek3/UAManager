using System.Windows;
using ClientLauncher.Pages;

namespace ClientLauncher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContentControl.Content = new SettingsPage();
    }
}