using System.Reflection;
using System.Windows;
using UAM.Core.Api;
using UAM.Core.AppSettings;

namespace ClientLauncher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private async void App_OnStartup(object sender, StartupEventArgs e)
    {
        if (AppSettings.Get().AutoCheckUpdates)
        {
            var api = new ApiUpdate(AppSettings.Get().ServerName.FirstOrDefault()!);
            var lastVersion = await api.GetLastUpdate();
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            
            if (currentVersion < lastVersion)
            {
                MessageBox.Show("У вас устаревшая версия");
            }
        }
    }
}