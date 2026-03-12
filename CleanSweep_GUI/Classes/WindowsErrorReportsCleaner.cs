using CleanSweep.Interfaces;
using System;
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
        _preCleanupSize = CalculateDirectorySize(_windowsErrorReportsDirectory);

        int totalSizeMBInt = ConvertBytesToMegabytes(_preCleanupSize);
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

        RichTextBoxExtensions.AppendText(_outputWindow, "Windows Error Reports Cleaned!\n");
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateDirectorySize(_windowsErrorReportsDirectory);
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
