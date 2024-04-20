using CleanSweep.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class TemporaryInternetFilesCleaner : ICleaner
{
    private readonly string _tempInternetFilesDirectory;
    private long _preCleanupSize;

    public TemporaryInternetFilesCleaner()
    {
        _tempInternetFilesDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft", "Windows", "INetCache"
        );
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        long totalSizeBytes = 0;

        try
        {
            if (Directory.Exists(_tempInternetFilesDirectory))
            {
                totalSizeBytes = Directory.GetFiles(_tempInternetFilesDirectory, "*.*", SearchOption.AllDirectories)
                                          .Sum(t => new FileInfo(t).Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        _preCleanupSize = totalSizeBytes;

        int totalSizeMBInt = ConvertBytesToMegabytes(totalSizeBytes);
        return ("Temporary Internet Files", totalSizeMBInt);
    }

    public Task Reclaim()
    {
        var di = new DirectoryInfo(_tempInternetFilesDirectory);

        foreach (var file in di.GetFiles())
        {
            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file {file.FullName}: {ex.Message}");
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
                Console.WriteLine($"Error deleting directory {dir.FullName}: {ex.Message}");
            }
        }

        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long newSize = 0;
        if (Directory.Exists(_tempInternetFilesDirectory))
        {
            newSize = Directory.GetFiles(_tempInternetFilesDirectory, "*", SearchOption.AllDirectories)
                               .Sum(t => new FileInfo(t).Length);
        }

        long reclaimedSpace = _preCleanupSize - newSize;
        return ConvertBytesToMegabytes(reclaimedSpace);
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        return (int)Math.Min(bytes / 1024 / 1024, int.MaxValue);
    }
}
