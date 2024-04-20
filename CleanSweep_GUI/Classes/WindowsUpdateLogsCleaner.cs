using CleanSweep.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class WindowsUpdateLogsCleaner : ICleaner
{
    private readonly string _windowsUpdateLogDir;
    private readonly bool _isVerboseMode;
    private long _preCleanupSize;

    public WindowsUpdateLogsCleaner(string windowsUpdateLogDir, bool isVerboseMode)
    {
        _windowsUpdateLogDir = windowsUpdateLogDir;
        _isVerboseMode = isVerboseMode;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = CalculateDirectorySize(_windowsUpdateLogDir);
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Windows Update Logs", spaceInMB);
    }

    public async Task Reclaim()
    {
        if (Directory.Exists(_windowsUpdateLogDir))
        {
            await Task.Run(() =>
            {
                var di = new DirectoryInfo(_windowsUpdateLogDir);
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
            });
        }
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateDirectorySize(_windowsUpdateLogDir);
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
