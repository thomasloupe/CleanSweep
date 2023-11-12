using CleanSweep2.Interfaces;
using System;
using System.IO;
using System.Linq;

public class MicrosoftOfficeCacheCleaner : ICleaner
{
    private string _msoCacheDir;
    private long _preCleanupSize;

    public MicrosoftOfficeCacheCleaner(string msoCacheDir)
    {
        _msoCacheDir = msoCacheDir;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = CalculateDirectorySize(_msoCacheDir);
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Microsoft Office Cache Files", spaceInMB);
    }

    public void Reclaim()
    {
        if (Directory.Exists(_msoCacheDir))
        {
            try
            {
                Directory.Delete(_msoCacheDir, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting Microsoft Office cache directory {_msoCacheDir}: {ex.Message}");
            }
        }
    }

    public long ReportReclaimedSpace()
    {
        // Calculate the space after cleanup
        long postCleanupSize = CalculateDirectorySize(_msoCacheDir);
        long reclaimedSpace = _preCleanupSize - postCleanupSize;
        return ConvertBytesToMegabytes(reclaimedSpace);
    }

    private long CalculateDirectorySize(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return 0;

        return Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories)
                        .Sum(file => new FileInfo(file).Length);
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        return (int)(bytes / 1024 / 1024);
    }
}
