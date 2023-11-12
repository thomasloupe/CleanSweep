using CleanSweep2.Interfaces;
using System;
using System.IO;
using System.Linq;

public class TemporarySetupFilesCleaner : ICleaner
{
    private readonly string _tempSetupDirectory;
    private long _preCleanupSize;

    public TemporarySetupFilesCleaner(string tempSetupDirectory)
    {
        _tempSetupDirectory = tempSetupDirectory;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        long totalSizeBytes = 0;

        if (Directory.Exists(_tempSetupDirectory))
        {
            totalSizeBytes = Directory.GetFiles(_tempSetupDirectory, "*", SearchOption.AllDirectories)
                                      .Sum(t => new FileInfo(t).Length);
        }

        _preCleanupSize = totalSizeBytes; // Store pre-cleanup size in bytes

        // Convert bytes to megabytes and safely cast to int
        int totalSizeMBInt = ConvertBytesToMegabytes(totalSizeBytes);
        return ("Temporary Setup Files", totalSizeMBInt);
    }

    public void Reclaim()
    {
        var di = new DirectoryInfo(_tempSetupDirectory);

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
        if (Directory.Exists(_tempSetupDirectory))
        {
            newSize = Directory.GetFiles(_tempSetupDirectory, "*", SearchOption.AllDirectories)
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
