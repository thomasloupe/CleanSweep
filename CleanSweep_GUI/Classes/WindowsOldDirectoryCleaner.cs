using CleanSweep.Interfaces;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class WindowsOldDirectoryCleaner : ICleaner
{
    private readonly string _windowsOldDirectoryPath = "C:\\Windows.old";
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public WindowsOldDirectoryCleaner(RichTextBox outputWindow)
    {
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = CalculateDirectorySize(_windowsOldDirectoryPath);
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Windows Old Directory", spaceInMB);
    }

    public async Task Reclaim()
    {
        if (Directory.Exists(_windowsOldDirectoryPath))
        {
            await Task.Run(() =>
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/C takeown /F C:\\Windows.old* /R /A /D Y & cacls C:\\Windows.old*.* /T /grant administrators:F & rmdir /S /Q C:\\Windows.old",
                            UseShellExecute = true,
                            Verb = "runas",
                            WorkingDirectory = "C:\\Windows\\",
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    };
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Windows OLD Directory Cleaner: Error deleting Windows.old directory: {ex.Message}");
                }
            });
        }
        else
        {
            Console.WriteLine("WindowsOldDirectoryCleaner(): The Windows .old directory doesn't exist.");
        }
        RichTextBoxExtensions.AppendText(_outputWindow, "Windows OLD Directory cleaned!\n", Color.Green);
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateDirectorySize(_windowsOldDirectoryPath);
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
