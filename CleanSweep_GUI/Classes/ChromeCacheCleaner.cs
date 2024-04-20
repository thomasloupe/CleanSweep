using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class ChromeCacheCleaner
{
    private readonly string[] _chromeCacheDirectories;
    private readonly bool _showOperationWindows;
    private readonly bool _isVerboseMode;
    private long _preCleanupSize;

    public ChromeCacheCleaner(string[] chromeCacheDirectories, bool showOperationWindows, bool isVerboseMode)
    {
        _chromeCacheDirectories = chromeCacheDirectories;
        _showOperationWindows = showOperationWindows;
        _isVerboseMode = isVerboseMode;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = _chromeCacheDirectories.Sum(dir => CalculateDirectorySize(dir));
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Chrome Cache Files", spaceInMB);
    }

    public async Task Reclaim()
    {
        await Task.Run(() =>
        {
            // Your existing process code

            foreach (var chromeDirectory in _chromeCacheDirectories)
            {
                if (Directory.Exists(chromeDirectory))
                {
                    try
                    {
                        Directory.Delete(chromeDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting Chrome cache directory {chromeDirectory}: {ex.Message}");
                    }
                }
            }
        });
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = _chromeCacheDirectories.Sum(dir => CalculateDirectorySize(dir));
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
        return (int)Math.Min(bytes / 1024 / 1024, int.MaxValue);
    }
}
