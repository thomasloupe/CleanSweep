using CleanSweep2.Interfaces;
using System;
using System.IO;
using System.Linq;

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

        _preCleanupSize = totalSizeBytes; // Store pre-cleanup size in bytes

        // Convert bytes to megabytes and safely cast to int
        int totalSizeMBInt = ConvertBytesToMegabytes(totalSizeBytes);
        return ("Windows Error Reports", totalSizeMBInt);
    }

    public void Reclaim()
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
                // Enhanced exception handling
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
                // Enhanced exception handling
                Console.WriteLine($"Error deleting directory {dir.FullName}: {ex.Message}");
            }
        }
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
