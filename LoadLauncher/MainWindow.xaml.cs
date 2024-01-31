using System.Diagnostics;
using System.Windows;
using UAM.Core.Installer;

namespace LoadLauncher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var installThread = new Thread(Install);
        installThread.Start();
    }

    private static async void Install()
    {
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