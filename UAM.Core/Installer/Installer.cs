using System.IO.Compression;
using UAM.Core.Api;
using UAM.Core.AppSettings;
using UAM.Core.Models;
using Settings = UAM.Core.AppSettings.AppSettings;


namespace UAM.Core.Installer;

public class Installer
{
    private readonly AppSettingsModel _appSettings = Settings.Get();
    private ApiUpdate? _apiUpdate = null;

    private async Task<Server?> GetAvailableServer()
    {
        var serverUrls = Settings.Get().ServerList;
        Server? availableServer = null;

        foreach (var server in serverUrls.Select(serverUrl => new Server(serverUrl)))
        {
            if (await server.CheckServerForAvailability())
                availableServer = server;
        }

        return availableServer;
    }
    
    public async Task Install(string version)
    {
        var availableServer = await GetAvailableServer();
        
        if(availableServer == null)
            return;

        _apiUpdate = new ApiUpdate(availableServer.ServerUrl);
        await _apiUpdate.GetUpdate(version);

        var currentDirectory = Environment.CurrentDirectory;
        
        // var files = Directory.GetFiles(currentDirectory);
        // var directories = Directory.GetDirectories(currentDirectory);
        //
        // foreach (var file in files)
        // {
        //     if (file.Contains("LoadLauncher") || file.EndsWith(".dll") || file.EndsWith(".pdb") ||
        //         file.EndsWith(".json"))
        //         continue;
        //
        //     File.Delete(file);
        // }
        //
        // foreach (var directory in directories)
        // {
        //     if (directory.Contains("updates"))
        //         continue;
        //
        //     var files1 = Directory.GetFiles(directory);
        //
        //     foreach (var file in files1)
        //     {
        //         if (file.EndsWith(".dll"))
        //             continue;
        //
        //         File.Delete(file);
        //     }
        //
        //     if (files1.Length == 0)
        //         Directory.Delete(directory);
        // }

        ZipFile.ExtractToDirectory(currentDirectory + $"/updates/{version}.zip", currentDirectory, true);

        if (!_appSettings.UseArchiver)
        {
            File.Delete(currentDirectory + $"/updates/{version}.zip");
        }
    }
}