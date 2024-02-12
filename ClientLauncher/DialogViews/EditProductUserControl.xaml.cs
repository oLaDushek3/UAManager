using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClientLauncher.Dialog;
using ClientLauncher.Entities;
using ModernWpf;

namespace ClientLauncher.DialogViews;

public partial class EditProductUserControl : UserControl
{
    private readonly UaClientDbContext _context = new();
    private readonly DialogProvider _currentDialogProvider;
    private readonly Product _editableProduct;
    
    private static readonly Regex Regex = new("[^0-9],");
    
    public EditProductUserControl(DialogProvider dialogProvider, Product selectedProduct)
    {
        _currentDialogProvider = dialogProvider;
        _editableProduct = _context.Products.First(p => p.Id == selectedProduct.Id);

        InitializeComponent();
        GetData();

        MainSpace.Background = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.Black
            : Brushes.White;
    }
    
    private void GetData()
    {
        UnitOfMeasurementComboBox.ItemsSource = _context.UnitOfMeasurements.ToList();
        VatComboBox.ItemsSource = _context.Vats.ToList();

        NameTextBox.Text = _editableProduct.Name;
        UnitOfMeasurementComboBox.SelectedItem = _editableProduct.UnitOfMeasurement;
        PriceTextBox.Text = _editableProduct.Price.ToString(CultureInfo.CurrentCulture);
        VatComboBox.SelectedItem = _editableProduct.Vat;
    }

    private void NumberOnlyTextBox_OnTextInput(object sender, TextCompositionEventArgs e) =>
        e.Handled = Regex.IsMatch(e.Text);
    
    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        _editableProduct.Name = NameTextBox.Text;
        _editableProduct.UnitOfMeasurement = (UnitOfMeasurement)UnitOfMeasurementComboBox.SelectedItem;
        _editableProduct.Price = Convert.ToDecimal(PriceTextBox.Text);
        _editableProduct.Vat = (Vat)VatComboBox.SelectedItem;
        _context.SaveChanges();
            
        _currentDialogProvider.CloseDialog(true);
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e) => _currentDialogProvider.CloseDialog(false);
}