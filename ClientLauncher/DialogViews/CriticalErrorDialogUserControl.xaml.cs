using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClientLauncher.Dialog;
using ModernWpf;
using UAM.Core.EmailException;

namespace ClientLauncher.DialogViews;

public partial class CriticalErrorDialogUserControl : UserControl
{
    private readonly DialogProvider _currentDialogProvider;
    private readonly string _error;
    
    public CriticalErrorDialogUserControl(DialogProvider dialogProvider, string error)
    {
        _currentDialogProvider = dialogProvider;
        _error = error;
        
        InitializeComponent();
        MainSpace.Background = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.Black
            : Brushes.White;
    }
    
    private async void ReportButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(EmailTextBox.Text) && !new EmailAddressAttribute().IsValid(EmailTextBox.Text))
        {
            ErrorTextBlock.Visibility = Visibility.Visible;
            ErrorTextBlock.Text="Неверный формат почты";
            return;
        }
        
        await ExceptionSender.SendEmailAsync(_error, EmailTextBox.Text);
        _currentDialogProvider.CloseDialog();
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e) => _currentDialogProvider.CloseDialog();
}