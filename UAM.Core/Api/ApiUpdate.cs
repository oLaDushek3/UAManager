using System.Text.Json;
using VersionModel = UAM.Core.Models.Version;

namespace UAM.Core.Api;

public class ApiUpdate : ApiBase
{
    public ApiUpdate(string baseUrl) : base(baseUrl)
    {
    }
    
    public async Task<List<VersionModel>> GetAllVersions()
    {
        var client = HttpClient;
        var response = await client.GetStringAsync("api/Update/GetAllVersions");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<List<VersionModel>>(response, options)!;
    }

    public async Task GetUpdate(string build)
    {
        var client = HttpClient;
        var response = await client.GetAsync($"api/Update/GetUpdate?build={build}");
        
        const string filePath = "updates";

        if (response.IsSuccessStatusCode)
        {
            var fileStream = await response.Content.ReadAsStreamAsync();
            SaveStreamAsFile(filePath, fileStream, $"{build}.zip");
        }
        else
        {
            throw new Exception("Error receiving the file");
        }
    }

    public async Task GetUpdateById(Guid id)
    {
        var client = HttpClient;
        var response = await client.GetAsync($"api/Update/GetUpdate?build={id}");

        const string filePath = "updates";

        if (response.IsSuccessStatusCode)
        {
            var fileStream = await response.Content.ReadAsStreamAsync();
            SaveStreamAsFile(filePath, fileStream, $"{id}.zip");
        }
        else
        {
            throw new Exception("Error receiving the file");
        }
    }

    public async Task<Version> GetLastUpdate()
    {
        var client = HttpClient;
        var response = await client.GetStringAsync("api/Update/GetLastUpdate");
        
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        
        return ConvertStringToVersion(JsonSerializer.Deserialize<VersionModel>(response, options)!.Build);
    }

    private void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
    {
        DirectoryInfo info = new DirectoryInfo(filePath);
        if (!info.Exists)
        {
            info.Create();
        }

        var path = Path.Combine(filePath, fileName);
        using FileStream outputFileStream = new FileStream(path, FileMode.Create);
        inputStream.CopyTo(outputFileStream);
    }

    private Version ConvertStringToVersion(string version)
    {
        int major, minor, build, revision = 0;
        
        var splitVersion = version.Split(".");

        if (splitVersion.Length is > 4 or < 3)
            throw new Exception("Wrong version view");
        
        if (int.TryParse(splitVersion[0], out int parseMajor))
            major = parseMajor;
        else
            throw new Exception("Wrong version view");
        
        if (int.TryParse(splitVersion[1], out int parseMinor))
            minor = parseMinor;
        else
            throw new Exception("Wrong version view");
        
        if (int.TryParse(splitVersion[2], out int parseBuild))
            build = parseBuild;
        else
            throw new Exception("Wrong version view");

        if (splitVersion.Length == 4)
            if(int.TryParse(splitVersion[2], out int parseRevision))
                revision = parseRevision;
            else
                throw new Exception("Wrong version view");

        
        return new Version(major, minor, build, revision);
    }
}