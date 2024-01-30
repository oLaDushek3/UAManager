using System.IO.Compression;
using UAM.Core.Api;

namespace UAM.Core.Installer;

public class Installer
{
    private readonly ApiUpdate _apiUpdate = new();

    public async Task Install(string version)
    {
        await _apiUpdate.GetUpdate(version);

        var path = "C:\\Users\\oLaDushek\\RiderProjects\\UAManager\\ClientLauncher\\bin\\Debug\\net8.0-windows";
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);
        
        foreach (var file in files)
        {
            if (file.EndsWith("LoadLauncher.exe") || file.EndsWith(".dll"))
                continue;
        
            File.Delete(file);
        }
        
        foreach (var directory in directories)
        {
            if (directory.EndsWith("updates"))
                continue;

            var files1 = Directory.GetFiles(directory);
                
            foreach (var file in files1)
            {
                if (file.EndsWith(".dll"))
                    continue;
                
                File.Delete(file);
            }
        
            if(files1.Length == 0)
                Directory.Delete(directory);
        }
        
        ZipFile.ExtractToDirectory($"updates/{version}.zip", path, true);
    }
}