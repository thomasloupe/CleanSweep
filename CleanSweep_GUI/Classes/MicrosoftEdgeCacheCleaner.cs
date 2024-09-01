using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CleanSweep.Interfaces;

public class MicrosoftEdgeCacheCleaner : ICleaner
{
    private readonly string[] _edgeCacheDirectories;
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public MicrosoftEdgeCacheCleaner(RichTextBox outputWindow)
    {
        _edgeCacheDirectories = new[]
        { 
            @"C:\Users\%USERNAME%\AppData\Local\Microsoft\Edge\User Data\Default\Cache", 
            @"C:\Users\%USERNAME%\AppData\Local\Microsoft\Edge\User Data\Profile 1\Cache" 
        };
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = _edgeCacheDirectories.Sum(dir => CalculateDirectorySize(dir));
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Microsoft Edge Cache Files", spaceInMB);
    }

    public async Task Reclaim()
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
                        Arguments = "/C TASKKILL /F /IM msedge.exe",
                        UseShellExecute = true,
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();

                foreach (var edgeDirectory in _edgeCacheDirectories)
                {
                    if (Directory.Exists(edgeDirectory))
                    {
                        try
                        {
                            Directory.Delete(edgeDirectory, true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Microsoft Edge Cache Claner: Error deleting Edge cache directory {edgeDirectory}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Edge cache cleanup: {ex.Message}");
            }
        });
        RichTextBoxExtensions.AppendText(_outputWindow, "Edge Cache cleaned!\n", Color.Green);
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = _edgeCacheDirectories.Sum(dir => CalculateDirectorySize(dir));
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
