using System.Windows;
using System.Windows.Controls;
using MailKit.Net.Smtp;
using MimeKit;
using UAM.Core.Entities;

namespace WorkerApp.Views;

public partial class ProblemEditUserControl : UserControl
{
	private List<Problem> _works;
	private Problem? _currentWork;
	private readonly UaVersionsContext _context = new();
	
    public ProblemEditUserControl()
    {
        InitializeComponent();
        _works = _context.Problems.Where(c => c.WorkerId == null).ToList();

        if (_works.Count == 0)
        {
	        NothingShowPanel.Visibility = Visibility.Visible;
	        TaskPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
	        _currentWork = _works.FirstOrDefault();
	        ShowWork(_currentWork!);
        }
    }

	private async void StartTimer()
	{
		var startTime = new DateTime(2000,1,1,1,5,0);

		TimerTextBlock.Text = $"Осталось времени: {startTime:mm:ss}";

		while (true)
		{
			await Task.Delay(1000);

			startTime = startTime.AddSeconds(-1);

			if (startTime is {Minute: 0, Second: 0})
			{
				MessageBox.Show("Истекло время на решение, данные присвоены автоматически");

				_currentWork.StatusId = 3;
				_currentWork.StartTime = DateTime.UtcNow;

				_context.Problems.Update(_currentWork);
				await _context.SaveChangesAsync();

				if (_currentWork.Email != null)
				{
					await SendEmailAsync(_currentWork.Email, "Назначен исполнитель");
				}

				_works.RemoveAt(0);

				if (_works.Count == 0)
				{
					NothingShowPanel.Visibility = Visibility.Visible;
					TaskPanel.Visibility = Visibility.Collapsed;
					return;
				}

				_currentWork = _works.FirstOrDefault();
				ShowWork(_currentWork!);
			}

			TimerTextBlock.Text = $"Осталось времени: {startTime:mm:ss}";
		}
	}

	private void ShowWork(Problem problem)
	{
		PriorityComboBox.ItemsSource = _context.Priorities.ToList();

		WorkerComboBox.ItemsSource = _context.Workers.ToList();

		PriorityComboBox.SelectedIndex = 0;

		WorkerComboBox.SelectedIndex = 0;
		_currentWork.WorkerId = 1;

		TaskTextBlock.Text = problem.ProblemText;

		if (problem.Email != null)
		{
			EmailTextBlock.Text = $"Почта клиента: {problem.Email}";
		}

		StartTimer();
	}

	private void PriorityComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		_currentWork.PriorityId = (PriorityComboBox.SelectedValue as Priority).Id;
	}

	private void WorkerComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		_currentWork.WorkerId = (WorkerComboBox.SelectedValue as Worker).Id;
	}

	private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
	{
		_currentWork.StatusId = 3;
		_currentWork.StartTime = DateTime.UtcNow;

		_context.Problems.Update(_currentWork);
		await _context.SaveChangesAsync();

		_works.RemoveAt(0);

		MessageBox.Show("Сохранено!");

		if (_works.Count == 0)
		{
			NothingShowPanel.Visibility = Visibility.Visible;
			TaskPanel.Visibility = Visibility.Collapsed;
			return;
		}

		if (_currentWork.Email != null)
		{
			await SendEmailAsync(_currentWork.Email, "Назначен исполнитель");
		}

		_currentWork = _works.FirstOrDefault();
		ShowWork(_currentWork!);
	}

	private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
	{
		_works = _context.Problems.Where(c => c.WorkerId == null).ToList();

		if (_works.Count == 0)
		{
			NothingShowPanel.Visibility = Visibility.Visible;
			TaskPanel.Visibility = Visibility.Collapsed;
		}
		else
		{
			NothingShowPanel.Visibility = Visibility.Collapsed;
			TaskPanel.Visibility = Visibility.Visible;
		}
	}

	public static async Task SendEmailAsync(string email, string text)
	{
		string login = "supprtinstallerapk220@mail.ru";
		string password = "vi8Lz1QjZxpZ0R9qzM21";

		using var emailMessage = new MimeMessage();

		emailMessage.From.Add(new MailboxAddress("Support", login));
		emailMessage.To.Add(new MailboxAddress("User", email));
		emailMessage.Subject = "Bug";
		emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
		{
			Text = text
		};

		using (var client = new SmtpClient())
		{
			await client.ConnectAsync("smtp.mail.ru", 465, true);
			await client.AuthenticateAsync(login, password);
			await client.SendAsync(emailMessage);

			await client.DisconnectAsync(true);
		}
	}
}