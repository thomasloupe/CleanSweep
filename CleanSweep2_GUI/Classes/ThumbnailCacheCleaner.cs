using System.Diagnostics;
using System.Threading.Tasks;

public class ThumbnailCacheCleaner
{
    private bool _showOperationWindows;
    private bool _isVerboseMode;

    public ThumbnailCacheCleaner(bool showOperationWindows, bool isVerboseMode)
    {
        _showOperationWindows = showOperationWindows;
        _isVerboseMode = isVerboseMode;
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
                    Arguments = "/C taskkill /f /im explorer.exe & timeout 1 & del / f / s / q / a %LocalAppData%\\Microsoft\\Windows\\Explorer\\thumbcache_ *.db & timeout 1 & start %windir%\\explorer.exe",
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = _showOperationWindows ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            process.WaitForExit();
        });
    }
}
