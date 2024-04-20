using CleanSweep.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class WindowsErrorReportsCleaner : ICleaner
{
    private readonly string _windowsErrorReportsDirectory;
    private long _preCleanupSize;

    public WindowsErrorReportsCleaner(string windowsErrorReportsDirectory)
    {
        _windowsErrorReportsDirectory = windowsErrorReportsDirectory;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        long totalSizeBytes = 0;

        if (Directory.Exists(_windowsErrorReportsDirectory))
        {
            totalSizeBytes = Directory.GetFiles(_windowsErrorReportsDirectory, "*", SearchOption.AllDirectories)
                                      .Sum(t => new FileInfo(t).Length);
        }

        _preCleanupSize = totalSizeBytes;

        int totalSizeMBInt = ConvertBytesToMegabytes(totalSizeBytes);
        return ("Windows Error Reports", totalSizeMBInt);
    }

    public Task Reclaim()
    {
        var di = new DirectoryInfo(_windowsErrorReportsDirectory);

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
        if (Directory.Exists(_windowsErrorReportsDirectory))
        {
            newSize = Directory.GetFiles(_windowsErrorReportsDirectory, "*", SearchOption.AllDirectories)
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
