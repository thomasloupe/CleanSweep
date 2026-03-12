using CleanSweep.Interfaces;
using System;
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
        _preCleanupSize = CalculateDirectorySize(_tempDirectory);
        int totalSizeMBInt = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Temporary Files", totalSizeMBInt);
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
        
        RichTextBoxExtensions.AppendText(_outputWindow, "Temporary Files cleaned!\n");
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateDirectorySize(_tempDirectory);
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
