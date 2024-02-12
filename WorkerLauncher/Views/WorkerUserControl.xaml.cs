using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls;
using UAM.Core.Entities;

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
        _timer = new DateTime(2000, 1, 1, 1, 0, 0);

        TimerTextBlock.Text = $"Осталось времени: {_timer:hh:mm:ss}";

        while (true)
        {
            await Task.Delay(1000);

            _timer = _timer.AddSeconds(-1);

            if (_timer is { Minute: 0, Second: 0 })
            {
                MessageBox.Show("Истекло время на решение, данные присвоены автоматически");
                _timer = new DateTime(2000, 1, 1, 1, 0, 0);
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

    private void MoreTime_OnClick(object sender, RoutedEventArgs e)
    {
        _timer = _timer.AddHours(1);
    }

    private void SendSolButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentWork.Email == null)
        {
            MessageBox.Show("Почта клиента неизвестна, задача выполнена");

            _currentWork.EndTime = DateTime.UtcNow;
            _currentWork.StatusId = 4;

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
        else
        {
            var contentDialog = new ContentDialog
            {
                Title = "Подтверждение",
                Content = new SolutionUserControl(_currentWork.Email),
                CloseButtonText = "Нет",
                PrimaryButtonText = "Да",
            };

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
    }
}