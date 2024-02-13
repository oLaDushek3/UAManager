using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClientLauncher.Dialog;
using ClientLauncher.Entities;
using Microsoft.EntityFrameworkCore;
using ModernWpf;
using ModernWpf.Controls;

namespace ClientLauncher.DialogViews;

public partial class EditRefundUserControl : UserControl
{
    private readonly DialogProvider _currentDialogProvider;
    private readonly Employee _loginEmployee;
    private readonly UaClientDbContext _context = new();
    private readonly List<RefundProduct> _selectedProductList;
    private readonly Refund _editableRefund;

    public EditRefundUserControl(DialogProvider dialogProvider, Employee loginEmployee, Refund selectedRefund)
    {
        _currentDialogProvider = dialogProvider;
        _loginEmployee = loginEmployee;
        _editableRefund = _context.Refunds.Include(r => r.Voucher).
            Include(r => r.RefundProducts).
            First(r => r.Id == selectedRefund.Id);
        InitializeComponent();
        GetData();
        MainSpace.Background = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.Black
            : Brushes.White;

        CustomerTextBox.Text = _editableRefund.Customer;
        DatePicker.SelectedDate = _editableRefund.Date;
        VoucherNumberBox.Value = _editableRefund.Voucher.Number;
        VoucherDatePicker.SelectedDate = _editableRefund.Date;
        AnnexDatePicker.SelectedDate = _editableRefund.AnnexDate;

        _selectedProductList = _editableRefund.RefundProducts.ToList();
        SelectedProductListView.ItemsSource = _selectedProductList;
    }

    private void GetData()
    {
        ProductListView.ItemsSource = _context.Products.Include(p => p.UnitOfMeasurement)
            .Include(p => p.Vat).ToList();
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
            Refund = _editableRefund,
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

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        _editableRefund.Customer = CustomerTextBox.Text;
        _editableRefund.EmployeeId = _loginEmployee.Id;
        _editableRefund.Date = ((DateTime)DatePicker.SelectedDate!).AddDays(1).ToUniversalTime();
        _editableRefund.Voucher = new Voucher
        {
            Number = (int)VoucherNumberBox.Value,
            Date =  ((DateTime)VoucherDatePicker.SelectedDate!).ToUniversalTime()
        };
        _editableRefund.AnnexDate = ((DateTime)AnnexDatePicker.SelectedDate!).AddDays(1).ToUniversalTime();

        var deleteRefundProduct = _editableRefund.RefundProducts.Except(_selectedProductList).ToList();
        foreach (var refundProduct in deleteRefundProduct)
            _context.RefundProducts.Remove(refundProduct);

        var addRefundProduct = _selectedProductList.Except(_editableRefund.RefundProducts).ToList();
        foreach (var refundProduct in addRefundProduct)
        {
            refundProduct.Refund = _editableRefund;
            _context.RefundProducts.Add(refundProduct);
        }
        
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