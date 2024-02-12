using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClientLauncher.Dialog;
using ClientLauncher.Entities;
using ModernWpf;

namespace ClientLauncher.DialogViews;

public partial class CreateProductUserControl : UserControl
{
    private readonly UaClientDbContext _context = new();
    private readonly DialogProvider _currentDialogProvider;
    
    private static readonly Regex Regex = new("[^0-9],");

    public CreateProductUserControl(DialogProvider dialogProvider)
    {
        _currentDialogProvider = dialogProvider;

        InitializeComponent();
        GetData();

        MainSpace.Background = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
            ? Brushes.Black
            : Brushes.White;
    }

    private void GetData()
    {
        UnitOfMeasurementComboBox.ItemsSource = null;
        UnitOfMeasurementComboBox.ItemsSource = _context.UnitOfMeasurements.ToList();
        VatComboBox.ItemsSource = _context.Vats.ToList();
    }

    private void NumberOnlyTextBox_OnTextInput(object sender, TextCompositionEventArgs e) =>
        e.Handled = Regex.IsMatch(e.Text);
    
    private void CreateButton_OnClick(object sender, RoutedEventArgs e)
    {
        _context.Products.Add(new Product
        {
            Name = NameTextBox.Text,
            UnitOfMeasurement = (UnitOfMeasurement)UnitOfMeasurementComboBox.SelectedItem,
            Price = Convert.ToDecimal(PriceTextBox.Text),
            Vat = (Vat)VatComboBox.SelectedItem
        });
        _context.SaveChanges();
            
        _currentDialogProvider.CloseDialog(true);
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e) => _currentDialogProvider.CloseDialog(false);
}