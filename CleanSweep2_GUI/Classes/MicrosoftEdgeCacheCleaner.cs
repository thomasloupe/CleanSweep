using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanSweep2.Interfaces;

public class MicrosoftEdgeCacheCleaner : ICleaner
{
    private string[] _edgeCacheDirectories;
    private bool _showOperationWindows;
    private bool _isVerboseMode;
    private long _preCleanupSize;

    public MicrosoftEdgeCacheCleaner(string[] edgeCacheDirectories, bool showOperationWindows, bool isVerboseMode)
    {
        _edgeCacheDirectories = edgeCacheDirectories;
        _showOperationWindows = showOperationWindows;
        _isVerboseMode = isVerboseMode;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = _edgeCacheDirectories.Sum(dir => CalculateDirectorySize(dir));
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Microsoft Edge Cache Files", spaceInMB);
    }

    public async Task Reclaim()
    {
        await Task.Run(() =>
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C TASKKILL /F /IM msedge.exe",
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = _showOperationWindows ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            process.WaitForExit();

            foreach (var edgeDirectory in _edgeCacheDirectories)
            {
                if (Directory.Exists(edgeDirectory))
                {
                    try
                    {
                        Directory.Delete(edgeDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        // Handle or log exceptions
                        Console.WriteLine($"Error deleting Edge cache directory {edgeDirectory}: {ex.Message}");
                    }
                }
            }
        });
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = _edgeCacheDirectories.Sum(dir => CalculateDirectorySize(dir));
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
