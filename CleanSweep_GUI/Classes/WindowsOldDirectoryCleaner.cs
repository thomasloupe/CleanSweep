using CleanSweep.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class WindowsOldDirectoryCleaner : ICleaner
{
    private readonly bool _showOperationWindows;
    private readonly string _windowsOldDirectoryPath = "C:\\Windows.old";
    private long _preCleanupSize;

    public WindowsOldDirectoryCleaner(bool showOperationWindows)
    {
        _showOperationWindows = showOperationWindows;
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
                            WindowStyle = _showOperationWindows ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                        }
                    };
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting Windows.old directory: {ex.Message}");
                }
            });
        }
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
