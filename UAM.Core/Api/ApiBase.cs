namespace UAM.Core.Api;

public class ApiBase
{
    private readonly string _baseUrl;
    
    protected HttpClient HttpClient => new(){BaseAddress = new Uri(_baseUrl)};

    protected ApiBase(string baseUrl)
    {
        _baseUrl = baseUrl;
    }
}