using CleanSweep.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class WindowsInstallerCacheCleaner : ICleaner
{
    private readonly string _windowsDirectory;
    private readonly bool _showOperationWindows;
    private long _preCleanupSize;

    public WindowsInstallerCacheCleaner(string windowsDirectory, bool showOperationWindows)
    {
        _windowsDirectory = windowsDirectory;
        _showOperationWindows = showOperationWindows;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        string patchCacheDir = Path.Combine(_windowsDirectory, "Installer", "$PatchCache$", "Managed");
        _preCleanupSize = CalculateDirectorySize(patchCacheDir);
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Windows Installer Cache", spaceInMB);
    }

    public async Task Reclaim()
    {
        string patchCacheDir = Path.Combine(_windowsDirectory, "Installer", "$PatchCache$", "Managed");

        await Task.Run(() =>
        {
            if (Directory.Exists(patchCacheDir))
            {
                try
                {
                    Directory.Delete(patchCacheDir, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting Windows Installer Cache: {ex.Message}");
                }
            }
        });
    }

    public long ReportReclaimedSpace()
    {
        string patchCacheDir = Path.Combine(_windowsDirectory, "Installer", "$PatchCache$", "Managed");
        long postCleanupSize = CalculateDirectorySize(patchCacheDir);
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
