using CleanSweep2.Interfaces;
using System;
using System.IO;
using System.Linq;

public class TemporaryFilesCleaner : ICleaner
{
    private string _tempDirectory;
    private long _tempDirSizeInMegaBytes;

    public TemporaryFilesCleaner(string tempDirectory)
    {
        _tempDirectory = tempDirectory;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        long totalSizeBytes = 0;

        if (Directory.Exists(_tempDirectory))
        {
            totalSizeBytes = Directory.GetFiles(_tempDirectory, "*", SearchOption.AllDirectories).Sum(t => new FileInfo(t).Length);
        }

        // Convert bytes to megabytes
        long totalSizeMB = totalSizeBytes / 1024 / 1024;

        // Safely cast to int, ensuring we don't exceed int.MaxValue
        int totalSizeMBInt = totalSizeMB > int.MaxValue ? int.MaxValue : (int)totalSizeMB;

        return ("Temporary Files", totalSizeMBInt);
    }

    public void Reclaim()
    {
        var di = new DirectoryInfo(_tempDirectory);

        foreach (var file in di.GetFiles())
        {
            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
            }
        }

        foreach (var dir in di.GetDirectories())
        {
            try
            {
                dir.Delete(true);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
            }
        }
    }

    public long ReportReclaimedSpace()
    {
        long newSize = 0;
        if (Directory.Exists(_tempDirectory))
        {
            newSize = Directory.GetFiles(_tempDirectory, "*", SearchOption.AllDirectories).Sum(t => new FileInfo(t).Length) / 1024 / 1024;
        }
        return _tempDirSizeInMegaBytes - newSize;
    }
}
