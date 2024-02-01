using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UAM.Core.Api;
using UAM.Core.AppSettings;

namespace ClientLauncher.Views;

public partial class SettingsUserControl : UserControl
{
    private readonly ApiUpdate _apiUpdate = new(AppSettings.Get().ServerName.First());
    
    public SettingsUserControl()
    {
        InitializeComponent();
        GetData();

        var appSettingsModel = AppSettings.Get();
        AutoCheckUpdatesCheckBox.IsChecked = appSettingsModel.AutoCheckUpdates;
        StopAutoCheckWhenErrorsCheckBox.IsChecked = appSettingsModel.StopAutoCheckWhenErrors;
        UseArchiverCheckBox.IsChecked = appSettingsModel.UseArchiver;
    }

    private async void GetData()
    {
        var versions = await _apiUpdate.GetAllVersions();
        var versionsList = versions.Select(version => new VersionView(version)).ToList();
        VersionListView.ItemsSource = versionsList;
    } 
    
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedVersion = ((VersionView)VersionListView.SelectedItem).CurrentVersion;
        Process.Start("LoadLauncher.exe", selectedVersion.Build);
        Application.Current.Shutdown();
    }
}