using System.Diagnostics;
using System.Reflection;
using System.Windows;
using ClientLauncher.Views;
using UAM.Core.Api;
using UAM.Core.AppSettings;

namespace ClientLauncher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var mainWindow = new MainWindow();
        mainWindow.Show();

        if (AppSettings.Get().AutoCheckUpdates)
        {
            var api = new ApiUpdate(AppSettings.Get().ServerName.FirstOrDefault()!);
            var lastVersion = await api.GetLastUpdate();
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            if (currentVersion < lastVersion)
            {
                if ((bool)await mainWindow.CurrentDialogProvider.ShowDialog(
                        new ConfirmDialogUserControl(mainWindow.CurrentDialogProvider,
                            "У вас устаревшая версия. \n Хотите обновить на актуальную?")))
                {
                    var ver = lastVersion.ToString();
                    Process.Start("LoadLauncher.exe", lastVersion.ToString());
                    Current.Shutdown();
                }
            }
        }
    }
}