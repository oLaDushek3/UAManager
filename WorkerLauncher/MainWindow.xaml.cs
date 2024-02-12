using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using UAM.Core.Entities;
using WorkerApp.Views;

namespace WorkerApp;

public partial class MainWindow : Window
{
    private readonly UaVersionsContext _context = new ();

    public MainWindow()
    {
        InitializeComponent();
        MainContentControl.Content = new AuthUserControl(this);
    }
}