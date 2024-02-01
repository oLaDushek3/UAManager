using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace UAM.Core.AppSettings;

public static class AppSettings
{
    private static string AppSettingsPath => Environment.CurrentDirectory + "/appsettings.json"; 

    private static AppSettingsModel DefaultSettings => new()
    {
        ServerName = new List<string>{"https://localhost:7206/"},
        AutoCheckUpdates = true,
        StopAutoCheckWhenErrors = true,
        UseArchiver = false
    };

    public static void SetAppSettingsDefault()
    {
        Set(DefaultSettings);
    }

    public static AppSettingsModel Get()
    {
        try
        {
            return JsonSerializer.Deserialize<AppSettingsModel>(File.ReadAllText(AppSettingsPath))!;
        }
        catch
        {
            SetAppSettingsDefault();
            return JsonSerializer.Deserialize<AppSettingsModel>(File.ReadAllText(AppSettingsPath))!;
        }
    }
    
    public static void Set(AppSettingsModel appSettings)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
        
        var json = JsonSerializer.Serialize(appSettings, options);
        File.WriteAllText(AppSettingsPath, json);
    }
}