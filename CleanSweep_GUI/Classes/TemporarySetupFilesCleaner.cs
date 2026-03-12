using CleanSweep.Interfaces;
using System;
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
        _preCleanupSize = CalculateDirectorySize(_tempSetupDirectory);
        int totalSizeMBInt = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Temporary Setup Files", totalSizeMBInt);
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
        
        RichTextBoxExtensions.AppendText(_outputWindow, "Temporary Setup Files cleaned!\n");
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateDirectorySize(_tempSetupDirectory);
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
