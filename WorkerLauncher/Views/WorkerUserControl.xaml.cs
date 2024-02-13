using System.Windows;
using System.Windows.Controls;
using UAM.Core.Entities;
using System.Windows;
using System.Windows.Controls;
using MailKit.Net.Smtp;
using MimeKit;

namespace WorkerApp.Views;

public partial class WorkerUserControl : UserControl
{
    private readonly UaVersionsContext _context = new();
    private readonly Worker _worker;
    private Problem? _currentWork;
    private DateTime _timer;

    public WorkerUserControl(Worker worker)
    {
        _worker = worker;
        
        InitializeComponent();
        RefreshVersions();
        
        WorkerTextBlock.Text = worker.FullName;
        
        _currentWork = _context.Problems.OrderBy(c => c.PriorityId)
            .FirstOrDefault(c => c.WorkerId == worker.Id && c.EndTime == null);
        if (_currentWork == null)
        {
            NothingShowStackPanel.Visibility = Visibility.Visible;
            TaskStackPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            ShowWork(_currentWork);

            NothingShowStackPanel.Visibility = Visibility.Collapsed;
            TaskStackPanel.Visibility = Visibility.Visible;
        }
    }

    private void RefreshVersions()
    {
        VersionComboBox.ItemsSource = _context.Versions.ToList();
    }
    
    private void ShowWork(Problem work)
    {
        TaskTextBlock.Text = work.ProblemText;

        if (work.Email != null)
        {
            EmailTextBlock.Text = $"Почта клиента: {work.Email}";
        }

        StartTimer();
    }

    private async void StartTimer()
    {
        _timer = DateTime.MinValue;
        _timer = _timer.AddHours(1);

        TimerTextBlock.Text = $"Осталось времени: {_timer:hh:mm:ss}";

        while (true)
        {
            await Task.Delay(1000);

            _timer = _timer.AddSeconds(-1);

            if (_timer is { Hour: 0, Minute: 0, Second: 0 })
            {
                MessageBox.Show("Истекло время на решение, данные присвоены автоматически");
                _timer = DateTime.MinValue;
            }

            TimerTextBlock.Text = $"Осталось времени: {_timer:hh:mm:ss}";
        }
    }

    private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
    {
        _currentWork = _context.Problems.FirstOrDefault(c => c.WorkerId == _worker.Id && c.EndTime == null);

        if (_currentWork == null)
        {
            NothingShowStackPanel.Visibility = Visibility.Visible;
            TaskStackPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            ShowWork(_currentWork);

            NothingShowStackPanel.Visibility = Visibility.Collapsed;
            TaskStackPanel.Visibility = Visibility.Visible;
        }
    }

    private void NotWorkButton_OnClick(object sender, RoutedEventArgs e)
    {
        var newWorker = _context.Workers.FirstOrDefault(c => c.Id != _worker.Id);

        _currentWork.WorkerId = newWorker.Id;
        _context.Problems.Update(_currentWork);
        _context.SaveChanges();

        var nextWork = _context.Problems.FirstOrDefault(c => c.WorkerId == _worker.Id && c.EndTime == null);

        if (nextWork == null)
        {
            NothingShowStackPanel.Visibility = Visibility.Visible;
            TaskStackPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            ShowWork(nextWork);

            NothingShowStackPanel.Visibility = Visibility.Collapsed;
            TaskStackPanel.Visibility = Visibility.Visible;
        }
    }

    private void MoreTime_OnClick(object sender, RoutedEventArgs e) => _timer = _timer.AddHours(1);

    private async void SendSolButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentWork!.Email != null)
        {
            var login = "supuamtest@gmail.com";
            var password = "mxnb ggra bloc dfhk";

            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Support", login));
            emailMessage.To.Add(new MailboxAddress("User", _currentWork.Email));
            emailMessage.Subject = "Bug";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = SolutionTextBox.Text
            };
            
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync(login, password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

        _currentWork.Version = VersionComboBox.Text;
        _currentWork.Solution = SolutionTextBox.Text;
        _currentWork.EndTime = DateTime.UtcNow;
        _currentWork.StatusId = 2;

        _context.Problems.Update(_currentWork);
        _context.SaveChanges();

        _currentWork =
            _context.Problems.FirstOrDefault(c => c.WorkerId == _worker.Id && c.EndTime == null);

        if (_currentWork == null)
        {
            NothingShowStackPanel.Visibility = Visibility.Visible;
            TaskStackPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            ShowWork(_currentWork);

            NothingShowStackPanel.Visibility = Visibility.Collapsed;
            TaskStackPanel.Visibility = Visibility.Visible;
        }
    }

    private void RefreshVersionsButton_OnClick(object sender, RoutedEventArgs e) => RefreshVersions();

    private void SolutionTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(SolutionTextBox.Text) && VersionComboBox.SelectionBoxItem != null)
            SendSolButton.IsEnabled = true;
    }

    private void VersionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(SolutionTextBox.Text) && VersionComboBox.SelectionBoxItem != null)
            SendSolButton.IsEnabled = true;
    }
}