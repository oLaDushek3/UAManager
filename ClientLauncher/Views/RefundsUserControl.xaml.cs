using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ClientLauncher.DialogViews;
using ClientLauncher.Entities;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Xceed.Words.NET;

namespace ClientLauncher.Views;

public partial class RefundsUserControl : UserControl
{
    private readonly UaClientDbContext _context = new();
    private readonly MainWindow _currentMainWindow;

    public RefundsUserControl(MainWindow mainWindow)
    {
        _currentMainWindow = mainWindow;
        InitializeComponent();
        GetData();
    }

    private void GetData()
    {
        RefundListView.ItemsSource = _context.Refunds.Include(r => r.Employee).
            Include(r => r.RefundProducts).
                ThenInclude(rp => rp.Product).
                    ThenInclude(p => p.UnitOfMeasurement).
            Include(r => r.RefundProducts).
                ThenInclude(rp => rp.Product).
                    ThenInclude(p => p.Vat).
            Include(r => r.Voucher).ToList();
    }

    private async void CreateRefundButton_OnClick(object sender, RoutedEventArgs e)
    {
        if ((bool)await _currentMainWindow.MainDialogProvider.ShowDialog(
                new CreateRefundUserControl(_currentMainWindow.MainDialogProvider, _currentMainWindow.LoginEmployee!)))
            GetData();
    }

    private async void EditRefundButton_OnClick(object sender, RoutedEventArgs e)
    {
        if ((bool)await _currentMainWindow.MainDialogProvider.ShowDialog(
                new EditRefundUserControl(_currentMainWindow.MainDialogProvider, _currentMainWindow.LoginEmployee,
                    (Refund)RefundListView.SelectedItem)))
            GetData();
    }

    private async void DeleteRefundButton_OnClick(object sender, RoutedEventArgs e)
    {
        if ((bool)await _currentMainWindow.MainDialogProvider.ShowDialog(
                new ConfirmDialogUserControl(_currentMainWindow.MainDialogProvider,
                    "Возвратная накладная будет удален")))
        {
            _context.Refunds.Remove((Refund)RefundListView.SelectedItem);
            await _context.SaveChangesAsync();
            GetData();

            DeleteRefundButton.IsEnabled = false;
            EditRefundButton.IsEnabled = false;
            SaveToDocButton.IsEnabled = true;
        }
    }

    private void RefundListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RefundListView != null)
        {
            DeleteRefundButton.IsEnabled = true;
            EditRefundButton.IsEnabled = true;
            SaveToDocButton.IsEnabled = true;
        }
    }

    private void SaveToDocButton_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedRefund = RefundListView.SelectedItem as Refund;

        var path = $"C:\\Users\\{Environment.UserName}\\Documents\\{DateTime.Now.Ticks}.docx";

        File.Copy("template.docx", path);
        
        var patterns = new Dictionary<string, string>()
        {
            { "total_products", selectedRefund.RefundProducts.Count.ToString() },
            {
                "total_products_sum", 
                selectedRefund.RefundProducts.Sum(sp => sp.Amount).ToString(CultureInfo.CurrentCulture)
            },
            {
                "total_products_sum_letters", 
                Convert.ToInt32(selectedRefund.RefundProducts.Sum(sp => sp.Quantity)).ToWords(new CultureInfo("ru-RU"))
            },
            { "date_attachment_day", selectedRefund.AnnexDate.Day.ToString() },
            {
                "date_attachment_month",
                selectedRefund.AnnexDate.ToString("MMMM", new CultureInfo("ru-RU"))
            },
            { "date_attachment_year_last_two", selectedRefund.AnnexDate.Year.ToString().Substring(2) },
            { "buyer_and_signature", selectedRefund.Customer + string.Join("", Enumerable.Repeat("_", 10)) },
            { "provider_and_signature", selectedRefund.Employee.FullName + string.Join("", Enumerable.Repeat("_", 10)) },
            { "comeback_sum", selectedRefund.RefundProducts.Sum(sp => sp.Amount).ToString(CultureInfo.CurrentCulture) },
            {
                "comeback_sum_letters",
                Convert.ToInt32(selectedRefund.RefundProducts.Sum(sp => sp.Amount)).ToWords(new CultureInfo("ru-RU"))
            },
            { "chek_number", selectedRefund.Voucher.Number.ToString() },
            { "date_check_day", selectedRefund.Voucher.Date.Day.ToString() },
            {
                "date_check_month",
                selectedRefund.Voucher.Date.ToString("MMMM", new CultureInfo("ru-RU"))
            },
            { "date_check_year_last_two", selectedRefund.Voucher.Date.Year.ToString().Substring(2) },
            { "date_footer_day", selectedRefund.Date.Day.ToString() },
            {
                "date_footer_month",
                selectedRefund.Date.ToString("MMMM", new CultureInfo("ru-RU"))
            },
            { "date_footer_year_last_two", selectedRefund.Date.Year.ToString().Substring(2) },
        };

        // Load a document.
        using (DocX document = DocX.Load(path))
        {
            var table = document.AddTable(selectedRefund.RefundProducts.Count + 2, 7);

            table.Rows[0]
                .Cells[0]
                .Paragraphs
                .First()
                .Append("Наименование товара")
                .Font("Times New Roman")
                .FontSize(12)
                .Bold();

            table.MergeCellsInColumn(0, 0, 1);

            table.Rows[0]
                .Cells[1]
                .Paragraphs
                .First()
                .Append("Еденица измерения")
                .Font("Times New Roman")
                .FontSize(12)
                .Bold();

            table.MergeCellsInColumn(1, 0, 1);

            table.Rows[0]
                .Cells[2]
                .Paragraphs
                .First()
                .Append("Колличество")
                .Font("Times New Roman")
                .FontSize(12)
                .Bold();

            table.MergeCellsInColumn(2, 0, 1);

            table.Rows[0]
                .Cells[3]
                .Paragraphs
                .First()
                .Append("Цена за еденицу руб .коп")
                .Font("Times New Roman")
                .FontSize(12)
                .Bold();

            table.MergeCellsInColumn(3, 0, 1);

            table.Rows[0]
                .Cells[4]
                .Paragraphs
                .First()
                .Append("НДС")
                .Font("Times New Roman")
                .FontSize(12)
                .Bold();

            table.Rows[0].MergeCells(4, 5);

            table.Rows[1]
                .Cells[4]
                .Paragraphs
                .First()
                .Append("Ставка, %")
                .Font("Times New Roman")
                .FontSize(12)
                .Bold();

            table.Rows[1]
                .Cells[5]
                .Paragraphs
                .First()
                .Append("Сумма, руб. коп.")
                .Font("Times New Roman")
                .FontSize(12)
                .Bold();

            table.Rows[1]
                .Cells[6]
                .Paragraphs
                .First()
                .Append("Сумма с учтом ндс, руб. коп.")
                .Font("Times New Roman")
                .FontSize(12)
                .Bold();

            table.MergeCellsInColumn(5, 0, 1);

            int count = 2;

            foreach (var refundProduct in selectedRefund.RefundProducts)
            {
                table.Rows[count]
                    .Cells[0]
                    .Paragraphs
                    .First()
                    .Append(refundProduct.Product.Name)
                    .Font("Times New Roman")
                    .FontSize(12);

                table.Rows[count]
                    .Cells[1]
                    .Paragraphs
                    .First()
                    .Append(refundProduct.Product.UnitOfMeasurement.Name)
                    .Font("Times New Roman")
                    .FontSize(12);

                table.Rows[count]
                    .Cells[2]
                    .Paragraphs
                    .First()
                    .Append(refundProduct.Quantity.ToString(new CultureInfo("ru-RU")))
                    .Font("Times New Roman")
                    .FontSize(12);

                table.Rows[count]
                    .Cells[3]
                    .Paragraphs
                    .First()
                    .Append(refundProduct.Product.Price.ToString(new CultureInfo("ru-RU")))
                    .Font("Times New Roman")
                    .FontSize(12);

                table.Rows[count]
                    .Cells[4]
                    .Paragraphs
                    .First()
                    .Append(refundProduct.Product.Vat.Value.ToString())
                    .Font("Times New Roman")
                    .FontSize(12);

                table.Rows[count]
                    .Cells[5]
                    .Paragraphs
                    .First()
                    .Append(Math.Round((refundProduct.Product.Price * refundProduct.Product.Vat.Value / 100) * refundProduct.Quantity, 2).ToString(new CultureInfo("ru-RU")))
                    .Font("Times New Roman")
                    .FontSize(12);

                table.Rows[count]
                    .Cells[6]
                    .Paragraphs
                    .First()
                    .Append(Math.Round((refundProduct.Product.Price + refundProduct.Product.Price * refundProduct.Product.Vat.Value / 100) *  refundProduct.Quantity, 2).ToString(new CultureInfo("ru-RU")))
                    .Font("Times New Roman")
                    .FontSize(12);

                count++;
            }
            
            document.ReplaceTextWithObject("{table}", table, false, RegexOptions.IgnoreCase);

            foreach (var pattern in patterns)
            {
                document.ReplaceText("{" + pattern.Key + "}", pattern.Value);
            }

            document.Save();

            MessageBox.Show("Документ сохранен в документы!");
        }
    }
}