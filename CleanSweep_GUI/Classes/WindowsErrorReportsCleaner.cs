using CleanSweep.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class WindowsErrorReportsCleaner : ICleaner
{
    private readonly string _windowsErrorReportsDirectory;
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public WindowsErrorReportsCleaner(RichTextBox outputWindow)
    {
        _windowsErrorReportsDirectory = @"C:\ProgramData\Microsoft\Windows\WER";
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        long totalSizeBytes = 0;

        if (Directory.Exists(_windowsErrorReportsDirectory))
        {
            totalSizeBytes = Directory.GetFiles(_windowsErrorReportsDirectory, "*", SearchOption.AllDirectories)
                                      .Sum(t => new FileInfo(t).Length);
        }

        _preCleanupSize = totalSizeBytes;

        int totalSizeMBInt = ConvertBytesToMegabytes(totalSizeBytes);
        return ("Windows Error Reports", totalSizeMBInt);
    }

    public Task Reclaim()
    {
        var di = new DirectoryInfo(_windowsErrorReportsDirectory);

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
            Console.WriteLine($"Windows Error Reports Cleaner: {ex.Message}");
        }

        RichTextBoxExtensions.AppendText(_outputWindow, "Windows Error Reports Cleaned!\n", Color.Green);
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long newSize = 0;
        if (Directory.Exists(_windowsErrorReportsDirectory))
        {
            newSize = Directory.GetFiles(_windowsErrorReportsDirectory, "*", SearchOption.AllDirectories)
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
