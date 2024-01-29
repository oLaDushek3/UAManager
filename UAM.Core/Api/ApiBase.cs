namespace UAM.Core.Api;

public class ApiBase
{
    private static string BaseUrl => "https://localhost:7206/";
    
    protected HttpClient HttpClient => new(){BaseAddress = new Uri(BaseUrl)};
}