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
        long totalSizeBytes = 0;

        try
        {
            if (Directory.Exists(_tempInternetFilesDirectory))
            {
                totalSizeBytes = Directory.GetFiles(_tempInternetFilesDirectory, "*.*", SearchOption.AllDirectories)
                                          .Sum(t => new FileInfo(t).Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        _preCleanupSize = totalSizeBytes;

        int totalSizeMBInt = ConvertBytesToMegabytes(totalSizeBytes);
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

        RichTextBoxExtensions.AppendText(_outputWindow, "Cleaning Temporary Setup Files...\n", Color.Green);
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long newSize = 0;
        if (Directory.Exists(_tempInternetFilesDirectory))
        {
            newSize = Directory.GetFiles(_tempInternetFilesDirectory, "*", SearchOption.AllDirectories)
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
