using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClientLauncher.Dialog;
using ModernWpf;

namespace ClientLauncher.Views;

public partial class ErrorDialogUserControl : UserControl
{
    private readonly DialogProvider _currentDialogProvider;
    
    public ErrorDialogUserControl(DialogProvider dialogProvider, string message)
    {
        _currentDialogProvider = dialogProvider;
        InitializeComponent();
        MainSpace.Background = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.Black
            : Brushes.White;
        MessageTextBlock.Text = message;
    }

    private void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
        _currentDialogProvider.CloseDialog();
    }
}