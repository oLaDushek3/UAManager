using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UAM.Core.Api;
using UAM.Core.AppSettings;
using UAM.Core.Models;

namespace ClientLauncher.Views;

public partial class SettingsUserControl : UserControl
{
    private readonly MainWindow _currentMainWindow;
    private ApiUpdate? _apiUpdate;
    private readonly AppSettingsModel _appSettings;
    private Server? _availableServer;

    public SettingsUserControl(MainWindow mainWindow)
    {
        _currentMainWindow = mainWindow;
        InitializeComponent();
        GetServerList();

        _appSettings = AppSettings.Get();
        AutoCheckUpdatesCheckBox.IsChecked = _appSettings.AutoCheckUpdates;
        StopAutoCheckWhenErrorsCheckBox.IsChecked = _appSettings.StopAutoCheckWhenErrors;
        UseArchiverCheckBox.IsChecked = _appSettings.UseArchiver;
    }

    private async void GetServerList()
    {
        var servers = AppSettings.Get().ServerList;
        var serverViewList = servers.Select(server => new ServerView(new Server(server))).ToList();
        if (serverViewList.Count == 0)
        {
            ServerListViewEmptyMessage.Visibility = Visibility.Visible;
            VersionListViewEmptyMessage.Text = "Нет доступных серверов";
            return;
        }
        ServerListView.ItemsSource = serverViewList;

        foreach (var serverView in serverViewList)
        {
            if (await serverView.FillingElementWithData() && _availableServer == null)
                _availableServer = serverView.Server;
        }
        
        GetVersionList();
    }

    private async void GetVersionList()
    {
        if (_availableServer != null)
        {
            _apiUpdate = new ApiUpdate(_availableServer.ServerUrl);
            var versions = await _apiUpdate.GetAllVersions();

            var versionsList = versions.Select(version => new VersionView(version)).ToList();
            VersionListView.ItemsSource = versionsList;

            VersionListViewEmptyMessage.Visibility = Visibility.Collapsed;
        }
        else
            VersionListViewEmptyMessage.Text = "Нет доступных серверов";
    }

    private void DownloadVersionButton_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedVersion = ((VersionView)VersionListView.SelectedItem).CurrentVersion;
        Process.Start("LoadLauncher.exe", selectedVersion.Build);
        Application.Current.Shutdown();
    }

    private void SaveSettingButton_OnClick(object sender, RoutedEventArgs e)
    {
        _appSettings.AutoCheckUpdates = (bool)AutoCheckUpdatesCheckBox.IsChecked!;
        _appSettings.StopAutoCheckWhenErrors = (bool)StopAutoCheckWhenErrorsCheckBox.IsChecked!;
        _appSettings.UseArchiver = (bool)UseArchiverCheckBox.IsChecked!;
        AppSettings.Set(_appSettings);
    }

    private void ResetSettingButton_OnClick(object sender, RoutedEventArgs e) => AppSettings.SetAppSettingsDefault();

    private void ServerListView_OnMouseLeftButtonDown(object sender, SelectionChangedEventArgs e)
    {
        if (ServerListView.SelectedItem != null)
            RemoveServerButton.IsEnabled = true;
    }

    private async void AddServerButton_OnClick(object sender, RoutedEventArgs e)
    {
        var newServer =
            await _currentMainWindow.CurrentDialogProvider.ShowDialog(
                new InputServerDialogUserControl(_currentMainWindow.CurrentDialogProvider));
        if (newServer is string server)
        {
            _appSettings.ServerList.Add(server);
            AppSettings.Set(_appSettings);
            GetServerList();
        }
    }

    private async void RemoveServerButton_OnClick(object sender, RoutedEventArgs e)
    {
        if ((bool)await _currentMainWindow.CurrentDialogProvider.ShowDialog(new ConfirmDialogUserControl(
                _currentMainWindow.CurrentDialogProvider,
                "Сервер будет удален! \n Вы уверены?")))
        {
            var selectedServerIndex =
                _appSettings.ServerList.IndexOf(((ServerView)ServerListView.SelectedItem).Server.ServerUrl);
            _appSettings.ServerList.Remove(_appSettings.ServerList[selectedServerIndex]);
            AppSettings.Set(_appSettings);
            GetServerList();
            RemoveServerButton.IsEnabled = false;
        }
    }

    private void UpdateServerListButton_OnClick(object sender, RoutedEventArgs e)
    {
        GetServerList();
    }

    private void UpdateVersionListButton_OnClick(object sender, RoutedEventArgs e)
    {
        GetVersionList();
    }
}