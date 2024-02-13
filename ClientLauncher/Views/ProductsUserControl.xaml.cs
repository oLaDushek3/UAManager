using System.Windows;
using System.Windows.Controls;
using ClientLauncher.DialogViews;
using ClientLauncher.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClientLauncher.Views;

public partial class ProductsUserControl : UserControl
{
    private readonly UaClientDbContext _context = new();
    private readonly MainWindow _currentMainWindow;

    public ProductsUserControl(MainWindow mainWindow)
    {
        try
        {
            _currentMainWindow = mainWindow;
            InitializeComponent();
            GetData();
        }
        catch (Exception exception)
        {
            mainWindow.MainDialogProvider.ShowDialog(
                new CriticalErrorDialogUserControl(mainWindow.MainDialogProvider, exception.Message));
        }
    }

    private void GetData()
    {
        try
        {

            ProductListView.ItemsSource = _context.Products.Include(p => p.UnitOfMeasurement).Include(p => p.Vat).ToList();
        }
        catch (Exception exception)
        {
            _currentMainWindow.MainDialogProvider.ShowDialog(
                new CriticalErrorDialogUserControl(_currentMainWindow.MainDialogProvider, exception.Message));
        }
    }

    private async void CreateProductButton_OnClick(object sender, RoutedEventArgs e)
    {
        if ((bool)await _currentMainWindow.MainDialogProvider.ShowDialog(
                new CreateProductUserControl(_currentMainWindow.MainDialogProvider)))
            GetData();
    }

    private async void EditProductButton_OnClick(object sender, RoutedEventArgs e)
    {
        if ((bool)await _currentMainWindow.MainDialogProvider.ShowDialog(
                new EditProductUserControl(_currentMainWindow.MainDialogProvider,
                    (Product)ProductListView.SelectedItem)))
            GetData();
    }

    private async void DeleteProductButton_OnClick(object sender, RoutedEventArgs e)
    {
        if ((bool)await _currentMainWindow.MainDialogProvider.ShowDialog(
                new ConfirmDialogUserControl(_currentMainWindow.MainDialogProvider, "Продукт будет удален")))
        {
            _context.Products.Remove((Product)ProductListView.SelectedItem);
            await _context.SaveChangesAsync();
            GetData();
            
            DeleteProductButton.IsEnabled = false;
            EditProductButton.IsEnabled = false;
        }
    }

    private void ProductListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProductListView != null)
        {
            DeleteProductButton.IsEnabled = true;
            EditProductButton.IsEnabled = true;
        }
    }
}