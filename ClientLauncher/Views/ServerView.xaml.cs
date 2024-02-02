using System.Windows.Controls;
using System.Windows.Media;
using UAM.Core.Models;

namespace ClientLauncher.Views;

public partial class ServerView : UserControl
{
    public readonly Server Server;
    
    public ServerView(Server server)
    {
        Server = server;
        
        InitializeComponent();
        PathTextBlock.Text = Server.ServerUrl;
    }

    public async Task<bool> FillingElementWithData()
    {
        await Server.CheckServerForAvailability();
           
        if (Server.Available)
        {
            StatusTextBlock.Text = "Доступен";
            StatusTextBlock.Foreground = Brushes.Green;
            return true;
        }
        else
        {
            StatusTextBlock.Text = "Недоступен";
            StatusTextBlock.Foreground = Brushes.Red; 
            return false;
        }
    }
}