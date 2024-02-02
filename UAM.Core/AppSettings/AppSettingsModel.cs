namespace UAM.Core.AppSettings;

public class AppSettingsModel
{
    public List<string> ServerList { get; set; } = null!;

    public bool AutoCheckUpdates { get; set; }
    
    public bool StopAutoCheckWhenErrors { get; set; }
    
    public bool UseArchiver { get; set; }
}