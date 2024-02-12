using System.Windows;
using System.Windows.Controls;
using UAM.Core.Entities;

namespace WorkerApp.Views;

public partial class AuthUserControl : UserControl
{
    private readonly UaVersionsContext _context = new();
    private readonly MainWindow _currentMainWindow;

    public AuthUserControl(MainWindow mainWindow)
    {
        _currentMainWindow = mainWindow;
        InitializeComponent();
    }

    private void LoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        var worker = _context.Workers.FirstOrDefault(w => w.FullName == LoginTextBox.Text);

        if (worker.Password != PasswordBox.Password)
            throw new Exception("Неверный пароль!");

        if (worker.RoleId == 1)
            _currentMainWindow.MainContentControl.Content = new ProblemEditUserControl();
        else
            _currentMainWindow.MainContentControl.Content = new WorkerUserControl(worker);
    }
}