using System.IO.Compression;

namespace NetEntreprisesTauxAT.App.Helpers;



public class ZipHelper
{
    public static int Extract(string zipPath, string extractPath)
    {
        var fileCount = 0;
        using var zipArchive = ZipFile.OpenRead(zipPath);
        foreach (var entry in zipArchive.Entries)
        {
            if(string.IsNullOrEmpty(entry.Name)) continue;
            var fullPath = Path.GetFullPath(Path.Combine(extractPath, entry.Name));
            if (fullPath.StartsWith(extractPath, StringComparison.OrdinalIgnoreCase))
            {
                entry.ExtractToFile(fullPath);
                fileCount++;
            }
        }
        return fileCount;
    }
}