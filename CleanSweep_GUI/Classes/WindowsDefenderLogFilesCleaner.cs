using CleanSweep.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class WindowsDefenderLogFilesCleaner : ICleaner
{
    private readonly string _programDataDirectory;
    private long _preCleanupSize;
    private readonly string[] _logFilePaths;
    private readonly RichTextBox _outputWindow;

    public WindowsDefenderLogFilesCleaner(RichTextBox outputWindow)
    {
        _programDataDirectory = @"C:\ProgramData\Microsoft\Windows Defender\Scans\History\Results";

        _logFilePaths = new[]
        {
            _programDataDirectory + @"\Microsoft\Windows Defender\Network Inspection System\Support\",
            _programDataDirectory + @"\Microsoft\Windows Defender\Scans\History\Service\",
            _programDataDirectory + @"\Microsoft\Windows Defender\Scans\History\ReportLatency\Latency",
            _programDataDirectory + @"\Microsoft\Windows Defender\Scans\History\Results\Resource",
            _programDataDirectory + @"\Microsoft\Windows Defender\Scans\History\Results\Quick",
            _programDataDirectory + @"\Microsoft\Windows Defender\Scans\History\CacheManager",
            _programDataDirectory + @"\Microsoft\Windows Defender\Scans\MetaStore",
            _programDataDirectory + @"\Microsoft\Windows Defender\Support"
        };
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = CalculateTotalLogFilesSize();
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Windows Defender Log Files", spaceInMB);
    }

    public async Task Reclaim()
    {
        await Task.Run(() =>
        {
            try
            {
                foreach (var directory in _logFilePaths)
                {
                    if (Directory.Exists(directory))
                    {
                        try
                        {
                            Directory.Delete(directory, true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting directory {directory}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Windows Defender Log Files Cleaner: Error during Windows Defender log files cleanup: {ex.Message}");
            }
        });
        RichTextBoxExtensions.AppendText(_outputWindow, "Windows Defender cleaned!\n", Color.Green);
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateTotalLogFilesSize();
        long reclaimedSpace = _preCleanupSize - postCleanupSize;
        return ConvertBytesToMegabytes(reclaimedSpace);
    }

    private long CalculateTotalLogFilesSize()
    {
        long totalSize = 0;

        foreach (var path in _logFilePaths)
        {
            if (Directory.Exists(path))
            {
                totalSize += Directory.GetFiles(path, "*.log", SearchOption.AllDirectories)
                                      .Sum(file => new FileInfo(file).Length);
            }
        }

        return totalSize;
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        return (int)Math.Min(bytes / 1024 / 1024, int.MaxValue);
    }
}
