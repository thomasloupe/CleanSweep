using CleanSweep.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class TemporarySetupFilesCleaner : ICleaner
{
    private readonly string _tempSetupDirectory;
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public TemporarySetupFilesCleaner(RichTextBox outputWindow)
    {
        _tempSetupDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        long totalSizeBytes = 0;

        try
        {
            if (Directory.Exists(_tempSetupDirectory))
            {
                totalSizeBytes = Directory.GetFiles(_tempSetupDirectory, "*", SearchOption.AllDirectories)
                                          .Sum(t => new FileInfo(t).Length);
            }

            _preCleanupSize = totalSizeBytes;

            int totalSizeMBInt = ConvertBytesToMegabytes(totalSizeBytes);
            return ("Temporary Setup Files", totalSizeMBInt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Temporary Setup Files Cleaner: Error during Temporary Setup Files cleanup: {ex.Message}");
            return ("Temporary Setup Files", 0);
        }
    }

    public Task Reclaim()
    {
        var di = new DirectoryInfo(_tempSetupDirectory);

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
            Console.WriteLine($"Temporary Setup Files Cleaner: Error during cleanup: {ex.Message}");
        }
        
        RichTextBoxExtensions.AppendText(_outputWindow, "Temporary Setup Files cleaned!\n", Color.Green);
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long newSize = 0;
        if (Directory.Exists(_tempSetupDirectory))
        {
            newSize = Directory.GetFiles(_tempSetupDirectory, "*", SearchOption.AllDirectories)
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
