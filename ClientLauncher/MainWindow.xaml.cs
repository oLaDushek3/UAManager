using System.Windows;
using ClientLauncher.Dialog;
using ClientLauncher.Entities;
using ClientLauncher.Views;

namespace ClientLauncher;

public partial class MainWindow : Window
{
    public readonly DialogProvider MainDialogProvider;
    public Employee? LoginEmployee;

    public MainWindow()
    {
        MainDialogProvider = new DialogProvider(this);
        InitializeComponent();
        MainContentControl.Content = new AuthUserControl(this);
    }

    private void ProductsButton_OnClick(object sender, RoutedEventArgs e) =>
        MainContentControl.Content = new ProductsUserControl(this);
    
    private void SettingsButton_OnClick(object sender, RoutedEventArgs e) =>
        MainContentControl.Content = new SettingsUserControl(this);

    private void RefundsBase_OnClick(object sender, RoutedEventArgs e) =>
        MainContentControl.Content = new RefundsUserControl(this);
}