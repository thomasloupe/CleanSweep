using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class ThumbnailCacheCleaner
{
    private readonly bool _showOperationWindows;
    private readonly bool _isVerboseMode;

    public ThumbnailCacheCleaner(bool showOperationWindows, bool isVerboseMode)
    {
        _showOperationWindows = showOperationWindows;
        _isVerboseMode = isVerboseMode;
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
                        Arguments = "/C taskkill /f /im explorer.exe & timeout 1 & del /f /s /q /a %LocalAppData%\\Microsoft\\Windows\\Explorer\\thumbcache_*.db & timeout 1 & start %windir%\\explorer.exe",
                        UseShellExecute = true,
                        Verb = "runas",
                        WindowStyle = _showOperationWindows ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during thumbnail cache cleanup: {ex.Message}");
            }
        });
    }
}
