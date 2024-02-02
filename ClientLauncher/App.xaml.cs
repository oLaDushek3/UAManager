using System.Diagnostics;
using System.Reflection;
using System.Windows;
using ClientLauncher.Views;
using UAM.Core.Api;
using UAM.Core.AppSettings;
using UAM.Core.Models;

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

        var settings = AppSettings.Get();
        try
        {
            if (!settings.AutoCheckUpdates) return;
            
            var serverUrls = AppSettings.Get().ServerList;
            Server? availableServer = null;

            foreach (var server in serverUrls.Select(serverUrl => new Server(serverUrl)))
            {
                if (await server.CheckServerForAvailability())
                    availableServer = server;
            }

            if (availableServer == null)
            {
                await mainWindow.CurrentDialogProvider.ShowDialog(
                    new ErrorDialogUserControl(mainWindow.CurrentDialogProvider,
                        "Нет доступных серверов"));
                return;
            }
            
            var api = new ApiUpdate(availableServer.ServerUrl);
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
        catch
        {
            settings.AutoCheckUpdates = false;
            AppSettings.Set(settings);
        }
        
    }
}