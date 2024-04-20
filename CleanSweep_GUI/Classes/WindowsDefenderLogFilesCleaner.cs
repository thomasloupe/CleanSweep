using CleanSweep.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class WindowsDefenderLogFilesCleaner : ICleaner
{
    private readonly string _programDataDirectory;
    private readonly bool _isVerboseMode;
    private long _preCleanupSize;

    private readonly string[] _logFilePaths;

    public WindowsDefenderLogFilesCleaner(string programDataDirectory, bool isVerboseMode)
    {
        _programDataDirectory = programDataDirectory;
        _isVerboseMode = isVerboseMode;

        _logFilePaths = new[]
        {
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Network Inspection System\\Support\\",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Service\\",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\ReportLatency\\Latency",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Results\\Resource",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Results\\Quick",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\CacheManager",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\MetaStore",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Support"
        };
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = CalculateTotalLogFilesSize();
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Windows Defender Log Files", spaceInMB);
    }

    public async Task Reclaim()
    {
        await Task.Run(() =>
        {
            foreach (var directory in _logFilePaths)
            {
                if (Directory.Exists(directory))
                {
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting directory {directory}: {ex.Message}");
                    }
                }
            }
        });
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateTotalLogFilesSize();
        long reclaimedSpace = _preCleanupSize - postCleanupSize;
        return ConvertBytesToMegabytes(reclaimedSpace);
    }

    private long CalculateTotalLogFilesSize()
    {
        long totalSize = 0;

        foreach (var path in _logFilePaths)
        {
            if (Directory.Exists(path))
            {
                totalSize += Directory.GetFiles(path, "*.log", SearchOption.AllDirectories)
                                      .Sum(file => new FileInfo(file).Length);
            }
        }

        return totalSize;
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        return (int)Math.Min(bytes / 1024 / 1024, int.MaxValue);
    }
}
