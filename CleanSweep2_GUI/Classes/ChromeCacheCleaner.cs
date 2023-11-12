using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class ChromeCacheCleaner
{
    private string[] _chromeCacheDirectories;
    private bool _showOperationWindows;
    private bool _isVerboseMode;

    public ChromeCacheCleaner(string[] chromeCacheDirectories, bool showOperationWindows, bool isVerboseMode)
    {
        _chromeCacheDirectories = chromeCacheDirectories;
        _showOperationWindows = showOperationWindows;
        _isVerboseMode = isVerboseMode;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        // For now, returning 0 as space and a description
        return ("Chrome Cache Files", 0);
    }

    public async Task Reclaim()
    {
        await Task.Run(() =>
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C TASKKILL /F /IM Chrome.exe",
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = _isVerboseMode ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            process.WaitForExit();

            foreach (var chromeDirectory in _chromeCacheDirectories)
            {
                if (Directory.Exists(chromeDirectory))
                {
                    try
                    {
                        Directory.Delete(chromeDirectory, true);
                    }
                    catch (Exception)
                    {
                        // Handle exceptions as needed
                    }
                }
            }
        });
    }

    public long ReportReclaimedSpace()
    {
        // This operation may not easily support reporting reclaimed space
        return 0;
    }
}
