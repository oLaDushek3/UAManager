using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UAM.Core.Api;

namespace ClientLauncher.Views;

public partial class SettingsPage : Page
{
    private readonly ApiUpdate _apiUpdate = new ApiUpdate();
    
    public SettingsPage()
    {
        InitializeComponent();
        GetAllVersion();
    }

    private async void GetAllVersion()
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