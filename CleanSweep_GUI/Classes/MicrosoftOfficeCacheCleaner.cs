using CleanSweep.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class MicrosoftOfficeCacheCleaner : ICleaner
{
    private readonly string _msoCacheDir;
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public MicrosoftOfficeCacheCleaner(RichTextBox outputWindow)
    {
        _msoCacheDir = @"C:\Users\%USERNAME%\AppData\Local\Microsoft\Office\16.0\OfficeFileCache";
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = CalculateDirectorySize(_msoCacheDir);
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Microsoft Office Cache Files", spaceInMB);
    }

    public Task Reclaim()
    {
        try
        {
            if (Directory.Exists(_msoCacheDir))
            {
                try
                {
                    Directory.Delete(_msoCacheDir, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Microsoft Office Cache Cleaner: Error deleting Microsoft Office cache directory {_msoCacheDir}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during Microsoft Office cache cleanup: {ex.Message}");
        }

        RichTextBoxExtensions.AppendText(_outputWindow, "Microsoft Office Cache cleaned!\n", Color.Green);
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateDirectorySize(_msoCacheDir);
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
        return (int)(bytes / 1024 / 1024);
    }
}
