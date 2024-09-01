using CleanSweep.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class WindowsUpdateLogsCleaner : ICleaner
{
    private readonly string _windowsUpdateLogDir;
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public WindowsUpdateLogsCleaner(RichTextBox outputWindow)
    {
        _windowsUpdateLogDir = @"C:\Windows\Logs\WindowsUpdate";
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = CalculateDirectorySize(_windowsUpdateLogDir);
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Windows Update Logs", spaceInMB);
    }

    public async Task Reclaim()
    {
        if (Directory.Exists(_windowsUpdateLogDir))
        {
            await Task.Run(() =>
            {
                var di = new DirectoryInfo(_windowsUpdateLogDir);
                try
                {
                    foreach (var file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file {file.FullName}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Windows Update Logs Cleaner: Error deleting files in {di.FullName}: {ex.Message}");
                }
            });
        }
        RichTextBoxExtensions.AppendText(_outputWindow, "Windows Update Logs cleaned!\n", Color.Green);
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateDirectorySize(_windowsUpdateLogDir);
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
        return (int)Math.Min(bytes / 1024 / 1024, int.MaxValue);
    }
}
