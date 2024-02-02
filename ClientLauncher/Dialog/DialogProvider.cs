using System.Windows;
using System.Windows.Controls;

namespace ClientLauncher.Dialog;

public class DialogProvider
{
    private readonly MainWindow _currentMainWindow;
    private object? _dialogResult;
    
    private delegate void CloseDialogDelegate();
    private event CloseDialogDelegate? CloseDialogEvent;

    public DialogProvider(MainWindow mainWindow)
    {
        _currentMainWindow = mainWindow;
    }
    
    public Task<object> ShowDialog(UserControl currentContentControl)
    {
        _currentMainWindow.DialogPanel.Visibility = Visibility.Visible;
        _currentMainWindow.DialogContentControl.Content = currentContentControl;
        _currentMainWindow.MainContentControl.IsEnabled = false;

        var completion = new TaskCompletionSource<object>();

        CloseDialogEvent += () => completion.TrySetResult(_dialogResult!);
        return completion.Task;
    }
    
    public void CloseDialog(object dialogResult)
    {
        _dialogResult = dialogResult;
        _currentMainWindow.DialogPanel.Visibility = Visibility.Collapsed;
        _currentMainWindow.DialogContentControl.Content = null;
        _currentMainWindow.MainContentControl.IsEnabled = true;

        CloseDialogEvent?.Invoke();
    }
    
    public void CloseDialog()
    {
        _dialogResult = null;
        _currentMainWindow.DialogPanel.Visibility = Visibility.Collapsed;
        _currentMainWindow.DialogContentControl.Content = null;
        _currentMainWindow.MainContentControl.IsEnabled = true;

        CloseDialogEvent?.Invoke();
    }
}