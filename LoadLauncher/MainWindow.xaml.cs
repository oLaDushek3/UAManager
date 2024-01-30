using System.IO;
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
        // var arguments = Environment.GetCommandLineArgs();
        //
        // var installer = new Installer();
        // await installer.Install(arguments[1]);
        //
        // var path = Path.Combine(Environment.CurrentDirectory,"ClientLauncher.exe");
        // System.Diagnostics.Process.Start(path);

        var installer = new Installer();
        await installer.Install("fff");

        Application.Current.Dispatcher.Invoke(() =>
        {
            var path = Path.Combine(Environment.CurrentDirectory,"ClientLauncher.exe");
            System.Diagnostics.Process.Start(path, "1.0.0");
            
            Application.Current.Shutdown();
        });
    }
}