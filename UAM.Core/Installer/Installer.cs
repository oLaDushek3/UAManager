using System.IO.Compression;
using UAM.Core.Api;
using UAM.Core.AppSettings;
using Settings = UAM.Core.AppSettings.AppSettings;

namespace UAM.Core.Installer;

public class Installer
{
    private readonly AppSettingsModel _appSettings = Settings.Get();
    private ApiUpdate ApiUpdate => new(_appSettings.ServerName.First());

    public async Task Install(string version)
    {
        await ApiUpdate.GetUpdate(version);

        var currentDirectory = Environment.CurrentDirectory;
        var files = Directory.GetFiles(currentDirectory);
        var directories = Directory.GetDirectories(currentDirectory);

        foreach (var file in files)
        {
            if (file.Contains("LoadLauncher") || file.EndsWith(".dll") || file.EndsWith(".pdb") ||
                file.EndsWith(".json"))
                continue;

            File.Delete(file);
        }

        foreach (var directory in directories)
        {
            if (directory.Contains("updates"))
                continue;

            var files1 = Directory.GetFiles(directory);

            foreach (var file in files1)
            {
                if (file.EndsWith(".dll"))
                    continue;

                File.Delete(file);
            }

            if (files1.Length == 0)
                Directory.Delete(directory);
        }

        ZipFile.ExtractToDirectory(currentDirectory + $"/updates/{version}.zip", currentDirectory, true);

        if (!_appSettings.UseArchiver)
        {
            File.Delete(currentDirectory + $"/updates/{version}.zip");
        }
    }
}