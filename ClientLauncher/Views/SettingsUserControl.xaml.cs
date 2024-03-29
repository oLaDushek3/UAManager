﻿using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using ClientLauncher.DialogViews;
using UAM.Core.Api;
using UAM.Core.AppSettings;
using UAM.Core.Models;

namespace ClientLauncher.Views;

public partial class SettingsUserControl : UserControl
{
    private readonly MainWindow _currentMainWindow;
    private ApiUpdate? _apiUpdate;
    private AppSettingsModel _appSettings;
    private Server? _availableServer;

    public SettingsUserControl(MainWindow mainWindow)
    {
        try
        {
            _currentMainWindow = mainWindow;
            InitializeComponent();
            GetSettings();
            GetServerList();
            
            CurrentVersionTextBlock.Text = $"Текущая версия приложения: {Assembly.GetExecutingAssembly().GetName().Version}";
        }
        catch (Exception e)
        {
            mainWindow.MainDialogProvider.ShowDialog(
                new CriticalErrorDialogUserControl(mainWindow.MainDialogProvider, e.Message));
        }
    }

    private void GetSettings()
    {
        _appSettings = AppSettings.Get();
        AutoCheckUpdatesCheckBox.IsChecked = _appSettings.AutoCheckUpdates;
        StopAutoCheckWhenErrorsCheckBox.IsChecked = _appSettings.StopAutoCheckWhenErrors;
        UseArchiverCheckBox.IsChecked = _appSettings.UseArchiver;
    }

    private async void GetServerList()
    {
        try
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
        catch (Exception exception)
        {
            _currentMainWindow.MainDialogProvider.ShowDialog(
                new CriticalErrorDialogUserControl(_currentMainWindow.MainDialogProvider, exception.Message));
        }
    }

    private async void GetVersionList()
    {
        try
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
        catch (Exception exception)
        {
            _currentMainWindow.MainDialogProvider.ShowDialog(
                new CriticalErrorDialogUserControl(_currentMainWindow.MainDialogProvider, exception.Message));
        }
    }

    private void DownloadVersionButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var selectedVersion = ((VersionView)VersionListView.SelectedItem).CurrentVersion;
            Process.Start("LoadLauncher.exe", selectedVersion.Build);
            Application.Current.Shutdown();
        }
        catch (Exception exception)
        {
            _currentMainWindow.MainDialogProvider.ShowDialog(
                new CriticalErrorDialogUserControl(_currentMainWindow.MainDialogProvider, exception.Message));
        }
    }

    private void SaveSettingButton_OnClick(object sender, RoutedEventArgs e)
    {
        _appSettings.AutoCheckUpdates = (bool)AutoCheckUpdatesCheckBox.IsChecked!;
        _appSettings.StopAutoCheckWhenErrors = (bool)StopAutoCheckWhenErrorsCheckBox.IsChecked!;
        _appSettings.UseArchiver = (bool)UseArchiverCheckBox.IsChecked!;
        AppSettings.Set(_appSettings);
    }

    private void ResetSettingButton_OnClick(object sender, RoutedEventArgs e)
    {
        AppSettings.SetAppSettingsDefault();
        GetSettings();
    } 

    private void ServerListView_OnMouseLeftButtonDown(object sender, SelectionChangedEventArgs e)
    {
        if (ServerListView.SelectedItem != null)
            RemoveServerButton.IsEnabled = true;
    }

    private async void AddServerButton_OnClick(object sender, RoutedEventArgs e)
    {
        var newServer =
            await _currentMainWindow.MainDialogProvider.ShowDialog(
                new InputServerDialogUserControl(_currentMainWindow.MainDialogProvider));
        if (newServer is string server)
        {
            _appSettings.ServerList.Add(server);
            AppSettings.Set(_appSettings);
            GetServerList();
        }
    }

    private async void RemoveServerButton_OnClick(object sender, RoutedEventArgs e)
    {
        if ((bool)await _currentMainWindow.MainDialogProvider.ShowDialog(new ConfirmDialogUserControl(
                _currentMainWindow.MainDialogProvider,
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