using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class WindowsOldDirectoryCleaner
{
    private bool _showOperationWindows;

    public WindowsOldDirectoryCleaner(bool showOperationWindows)
    {
        _showOperationWindows = showOperationWindows;
    }

    public long GetReclaimableSpace()
    {
        // This operation may not easily support a pre-calculation of reclaimable space
        return 0;
    }

    public async Task Reclaim()
    {
        if (Directory.Exists("C:\\windows.old"))
        {
            await Task.Run(() =>
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
            });
        }
    }

    public long ReportReclaimedSpace()
    {
        // This operation may not easily support reporting reclaimed space
        return 0;
    }
}
