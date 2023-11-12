using CleanSweep2.Interfaces;
using System.Diagnostics;
using System.Threading.Tasks;

public class UserFileHistoryCleaner : ICleaner
{
    private bool _showOperationWindows;

    public UserFileHistoryCleaner(bool showOperationWindows)
    {
        _showOperationWindows = showOperationWindows;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        return ("User File History", 0);
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
                    Arguments = "/C FhManagew.exe -cleanup 0",
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = _showOperationWindows ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            process.WaitForExit();
        });
    }

    public long ReportReclaimedSpace()
    {
        // This operation may not easily support reporting reclaimed space
        return 0;
    }
}
