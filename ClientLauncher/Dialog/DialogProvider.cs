using System.Windows;
using System.Windows.Controls;

namespace ClientLauncher.Dialog;

public class DialogProvider(MainWindow mainWindow)
{
    private object? _dialogResult;
    
    private delegate void CloseDialogDelegate();
    private event CloseDialogDelegate? CloseDialogEvent;

    public Task<object> ShowDialog(UserControl currentContentControl)
    {
        mainWindow.DialogPanel.Visibility = Visibility.Visible;
        mainWindow.DialogContentControl.Content = currentContentControl;
        mainWindow.MainContentControl.IsEnabled = false;

        var completion = new TaskCompletionSource<object>();

        CloseDialogEvent += () => completion.TrySetResult(_dialogResult!);
        return completion.Task;
    }
    
    public void CloseDialog(object dialogResult)
    {
        _dialogResult = dialogResult;
        mainWindow.DialogPanel.Visibility = Visibility.Collapsed;
        mainWindow.DialogContentControl.Content = null;
        mainWindow.MainContentControl.IsEnabled = true;

        CloseDialogEvent?.Invoke();
    }
    
    public void CloseDialog()
    {
        _dialogResult = null;
        mainWindow.DialogPanel.Visibility = Visibility.Collapsed;
        mainWindow.DialogContentControl.Content = null;
        mainWindow.MainContentControl.IsEnabled = true;

        CloseDialogEvent?.Invoke();
    }
}