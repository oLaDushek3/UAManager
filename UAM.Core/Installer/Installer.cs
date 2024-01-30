using System.IO.Compression;
using UAM.Core.Api;

namespace UAM.Core.Installer;

public class Installer
{
    private readonly ApiUpdate _apiUpdate = new();

    public async Task Install(string version)
    {
        Thread.Sleep(5000);

        // await _apiUpdate.GetUpdate(version);
        //
        // var path = Environment.CurrentDirectory;
        // var files = Directory.GetFiles(path);
        // var directories = Directory.GetDirectories(path);
        //
        // foreach (var file in files)
        // {
        //     if (file.EndsWith(".exe") || file.EndsWith(".dll"))
        //         continue;
        //
        //     File.Delete(file);
        // }
        //
        // foreach (var directory in directories)
        // {
        //     if (directory.EndsWith("updates"))
        //         continue;
        //
        //     foreach (var file in Directory.GetFiles(directory))
        //     {
        //         File.Delete(file);
        //     }
        //
        //     Directory.Delete(directory);
        // }
        //
        // ZipFile.ExtractToDirectory($"updates/{version}.zip", path);
    }
}