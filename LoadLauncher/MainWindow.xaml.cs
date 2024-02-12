using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ModernWpf;
using UAM.Core.Installer;
namespace LoadLauncher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainSpace.Background = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.Black
            : Brushes.White;
        MainTextBlock.Foreground= ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.White
            : Brushes.Black;

        var installThread = new Thread(Install);
        installThread.Start();
    }

    private static async void Install()
    {
        Thread.Sleep(2000);
        
        var arguments = Environment.GetCommandLineArgs();
        var installer = new Installer();
        await installer.Install(arguments[1]);

        Application.Current.Dispatcher.Invoke(() =>
        {
            Process.Start("ClientLauncher.exe");
            Application.Current.Shutdown();
        });
    }
}