using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClientLauncher.Dialog;
using ClientLauncher.Entities;
using Microsoft.EntityFrameworkCore;
using ModernWpf;
using ModernWpf.Controls;

namespace ClientLauncher.DialogViews;

public partial class CreateRefundUserControl : UserControl
{
    private readonly DialogProvider _currentDialogProvider;
    private readonly Employee _loginEmployee;
    private readonly UaClientDbContext _context = new();
    private readonly List<RefundProduct> _selectedProductList = new();
    private readonly Refund _createdRefund = new();

    public CreateRefundUserControl(DialogProvider dialogProvider, Employee loginEmployee)
    {
        _currentDialogProvider = dialogProvider;
        _loginEmployee = loginEmployee;
        InitializeComponent();
        GetData();
        MainSpace.Background = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.Black
            : Brushes.White;
    }

    private void GetData()
    {
        ProductListView.ItemsSource = _context.Products.Include(p => p.UnitOfMeasurement).Include(p => p.Vat).ToList();
    }

    private void ProductListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProductListView.SelectedItem != null && QuantityNumberBox.Value != 0 &&
            !double.IsNaN(QuantityNumberBox.Value))
            AddProductButton.IsEnabled = true;
    }

    private void QuantityNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (ProductListView.SelectedItem != null && QuantityNumberBox.Value != 0 &&
            !double.IsNaN(QuantityNumberBox.Value))
            AddProductButton.IsEnabled = true;
    }
    
    private void AddProductButton_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedProduct = (Product)ProductListView.SelectedItem;
        _selectedProductList.Add(new RefundProduct
        {
            Product = selectedProduct,
            Refund = _createdRefund,
            Quantity = Convert.ToDecimal(QuantityNumberBox.Value),
            Amount = (selectedProduct.Price + selectedProduct.Price * selectedProduct.Vat.Value / 100) * Convert.ToDecimal(QuantityNumberBox.Value)
        });
        SelectedProductListView.ItemsSource = null;
        SelectedProductListView.ItemsSource = _selectedProductList;
        CounterTextBox.Text =
            $"Всего товаров: {_selectedProductList.Count} На сумму: {_selectedProductList.Sum(sp => sp.Amount)}";
        
        AddProductButton.IsEnabled = false;
    }
    
    private void RemoveProductButton_OnClick(object sender, RoutedEventArgs e)
    {
        _selectedProductList.Remove((RefundProduct)SelectedProductListView.SelectedItem);
        SelectedProductListView.ItemsSource = null;
        SelectedProductListView.ItemsSource = _selectedProductList;
        CounterTextBox.Text =
            $"Всего товаров: {_selectedProductList.Count} На сумму: {_selectedProductList.Sum(sp => sp.Amount)}";
        
        RemoveProductButton.IsEnabled = false;
    }

    private void CreateButton_OnClick(object sender, RoutedEventArgs e)
    {
        _createdRefund.Customer = CustomerTextBox.Text;
        _createdRefund.EmployeeId = _loginEmployee.Id;
        _createdRefund.Date = ((DateTime)DatePicker.SelectedDate!).ToUniversalTime();
        _createdRefund.Voucher = new Voucher
        {
            Number = (int)VoucherNumberBox.Value,
            Date =  ((DateTime)VoucherDatePicker.SelectedDate!).ToUniversalTime()
        };
        _createdRefund.AnnexDate = ((DateTime)AnnexDatePicker.SelectedDate!).ToUniversalTime();
        _context.Refunds.Add(_createdRefund);

        foreach (var refundProduct in _selectedProductList)
            _context.RefundProducts.Add(refundProduct);

        _context.SaveChanges();

        _currentDialogProvider.CloseDialog(true);
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e) => _currentDialogProvider.CloseDialog(false);

    private void SelectedProductListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedProductListView.SelectedItem != null)
            RemoveProductButton.IsEnabled = true;
    }
}