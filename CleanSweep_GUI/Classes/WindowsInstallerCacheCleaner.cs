using CleanSweep.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class WindowsInstallerCacheCleaner : ICleaner
{
    private readonly string _windowsDirectory;
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public WindowsInstallerCacheCleaner(RichTextBox outputWindow)
    {
        _windowsDirectory = @"C:\Windows\Installer";
        _outputWindow = outputWindow;
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
            try
            {
                if (Directory.Exists(patchCacheDir))
                {
                    Directory.Delete(patchCacheDir, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Windows Installer Cache Cleaner: Error during Windows Installer Cache cleanup: {ex.Message}");
            }
        });
        RichTextBoxExtensions.AppendText(_outputWindow, "Windows Installer Cache cleaned!\n");
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

        long totalSize = 0;
        try
        {
            foreach (var file in new DirectoryInfo(directoryPath).EnumerateFiles("*", SearchOption.AllDirectories))
            {
                try { totalSize += file.Length; }
                catch { }
            }
        }
        catch { }
        return totalSize;
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        return (int)Math.Min(bytes / 1024 / 1024, int.MaxValue);
    }
}
