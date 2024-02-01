using System.Windows;
using ClientLauncher.Dialog;
using ClientLauncher.Views;

namespace ClientLauncher;

public partial class MainWindow : Window
{
    public readonly DialogProvider CurrentDialogProvider;
    
    public MainWindow()
    {
        CurrentDialogProvider = new DialogProvider(this);
        InitializeComponent();
        MainContentControl.Content = new SettingsUserControl();
    }
}