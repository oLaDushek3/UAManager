using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ClientLauncher.Pages;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var path = Path.Combine(Environment.CurrentDirectory,"LoadLauncher.exe");
        System.Diagnostics.Process.Start(path, "1.0.0");

        Application.Current.Shutdown();
    }
}