using System.Windows;
using System.Windows.Controls;
using ClientLauncher.Entities;

namespace ClientLauncher.Views;

public partial class AuthUserControl : UserControl
{
    private readonly UaClientDbContext _context = new();
    private readonly MainWindow _currentMainWindow;

    public AuthUserControl(MainWindow mainWindow)
    {
        _currentMainWindow = mainWindow;
        InitializeComponent();
    }

    private void LoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        var employee = _context.Employees.FirstOrDefault(em => em.Login == LoginTextBox.Text && em.Password == PasswordBox.Password);

        if (employee == null)
        {
            ErrorTextBlock.Visibility = Visibility.Visible;
            ErrorTextBlock.Text = "Неверный логин или пароль!";
            return;
        }

        _currentMainWindow.LoginEmployee = employee;
        _currentMainWindow.MainContentControl.Content = new ProductsUserControl(_currentMainWindow);
        _currentMainWindow.NavigationPanel.Visibility = Visibility.Visible;
    }
}