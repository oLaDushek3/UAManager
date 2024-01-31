using System.IO.Compression;
using UAM.Core.Api;

namespace UAM.Core.Installer;

public class Installer
{
    private readonly ApiUpdate _apiUpdate = new();

    public async Task Install(string version)
    {
        Thread.Sleep(2000);
        
        await _apiUpdate.GetUpdate(version);

        var path = Environment.CurrentDirectory;
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);
        
        foreach (var file in files)
        {
            if (file.Contains("LoadLauncher") || file.EndsWith(".dll") || file.EndsWith(".pdb") || file.EndsWith(".json"))
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
        
            if(files1.Length == 0)
                Directory.Delete(directory);
        }
        
        ZipFile.ExtractToDirectory($"updates/{version}.zip", path, true);
    }
}