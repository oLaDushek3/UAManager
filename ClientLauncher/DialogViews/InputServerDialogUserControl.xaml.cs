using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClientLauncher.Dialog;
using ModernWpf;
using UAM.Core.AppSettings;

namespace ClientLauncher.DialogViews;

public partial class InputServerDialogUserControl : UserControl
{
    private readonly DialogProvider _currentDialogProvider;

    public InputServerDialogUserControl(DialogProvider dialogProvider)
    {
        _currentDialogProvider = dialogProvider;
        InitializeComponent();
        MainSpace.Background = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.Black
            : Brushes.White;
    }
    
    private void YesButton_OnClick(object sender, RoutedEventArgs e)
    {
        var existingServer = AppSettings.Get().ServerList.FirstOrDefault(s => s == ServerPathTextBox.Text);
        if (existingServer != null)
        {
            ErrorTextBlock.Text = "Такой сервер уже есть в списке";
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
        else
            _currentDialogProvider.CloseDialog(ServerPathTextBox.Text);
    }

    private void NoButton_OnClick(object sender, RoutedEventArgs e)
    {
        _currentDialogProvider.CloseDialog();
    }
}