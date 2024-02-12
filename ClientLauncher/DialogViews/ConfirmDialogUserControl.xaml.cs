using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClientLauncher.Dialog;
using ModernWpf;

namespace ClientLauncher.DialogViews;

public partial class ConfirmDialogUserControl : UserControl
{
    private readonly DialogProvider _currentDialogProvider;
    
    public ConfirmDialogUserControl(DialogProvider dialogProvider, string message)
    {
        _currentDialogProvider = dialogProvider;
        InitializeComponent();
        MainSpace.Background = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.Black
            : Brushes.White;
        MessageTextBlock.Text = message;
    }

    private void YesButton_OnClick(object sender, RoutedEventArgs e) =>  _currentDialogProvider.CloseDialog(true);

    private void NoButton_OnClick(object sender, RoutedEventArgs e) => _currentDialogProvider.CloseDialog(false);
}