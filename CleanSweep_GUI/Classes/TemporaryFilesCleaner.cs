using CleanSweep.Interfaces;
using Octokit;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class TemporaryFilesCleaner : ICleaner
{
    private readonly string _tempDirectory;
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public TemporaryFilesCleaner(RichTextBox outputWindow)
    {
        _tempDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        long totalSizeBytes = 0;
        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                totalSizeBytes = Directory.GetFiles(_tempDirectory, "*.*", SearchOption.AllDirectories)
                                          .Sum(t => new FileInfo(t).Length);
            }

            _preCleanupSize = totalSizeBytes;

            int totalSizeMBInt = ConvertBytesToMegabytes(totalSizeBytes);
            return ("Temporary Files", totalSizeMBInt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Temporary Files Cleaner: Error during Temporary Files cleanup: {ex.Message}");
            return ("Temporary Files", 0);
        }
    }

    public Task Reclaim()
    {
        var di = new DirectoryInfo(_tempDirectory);

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
            Console.WriteLine($"Temporary Files Cleaner: Error during Temporary Files cleanup: {ex.Message}");
        }
        
        RichTextBoxExtensions.AppendText(_outputWindow, "Temporary Files cleaned!\n", Color.YellowGreen);
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long newSize = 0;
        if (Directory.Exists(_tempDirectory))
        {
            newSize = Directory.GetFiles(_tempDirectory, "*", SearchOption.AllDirectories)
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
