namespace UAM.Core.Models;

public class Server
{
    public string ServerUrl { get; set; } = null!;
    
    public bool Available { get; set; }

    public Server(string serverUrl)
    {
        ServerUrl = serverUrl;
    }
    
    public async Task<bool> CheckServerForAvailability()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(ServerUrl) };
        
        try
        {
            var response = await httpClient.GetAsync("/");
            if (response.IsSuccessStatusCode)
            {
                Available = true;
            }
            else
                throw new Exception();
        }
        catch
        {
            Available = false;
        }

        return Available;
    }
}