using CleanSweep.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class TemporaryInternetFilesCleaner : ICleaner
{
    private readonly string _tempInternetFilesDirectory;
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public TemporaryInternetFilesCleaner(RichTextBox outputWindow)
    {
        _tempInternetFilesDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft", "Windows", "INetCache"
        );
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = CalculateDirectorySize(_tempInternetFilesDirectory);
        int totalSizeMBInt = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Temporary Internet Files", totalSizeMBInt);
    }

    public Task Reclaim()
    {
        var di = new DirectoryInfo(_tempInternetFilesDirectory);

        try
        {
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Temporary Internet Files Cleaner: Failed to delete file or directory { ex.Message}");
        }

        RichTextBoxExtensions.AppendText(_outputWindow, "Cleaning Temporary Setup Files...\n");
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateDirectorySize(_tempInternetFilesDirectory);
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
