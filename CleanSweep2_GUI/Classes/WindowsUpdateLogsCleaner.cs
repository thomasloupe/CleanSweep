using System;
using System.IO;
using System.Threading.Tasks;

public class WindowsUpdateLogsCleaner
{
    private string _windowsUpdateLogDir;
    private bool _isVerboseMode;

    public WindowsUpdateLogsCleaner(string windowsUpdateLogDir, bool isVerboseMode)
    {
        _windowsUpdateLogDir = windowsUpdateLogDir;
        _isVerboseMode = isVerboseMode;
    }

    public long GetReclaimableSpace()
    {
        long reclaimableSpace = 0;
        if (Directory.Exists(_windowsUpdateLogDir))
        {
            var di = new DirectoryInfo(_windowsUpdateLogDir);
            foreach (var file in di.GetFiles())
            {
                reclaimableSpace += file.Length;
            }
        }
        return reclaimableSpace;
    }

    public async Task Reclaim()
    {
        if (Directory.Exists(_windowsUpdateLogDir))
        {
            var di = new DirectoryInfo(_windowsUpdateLogDir);
            foreach (var file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {
                    // Log or handle exceptions as needed
                }
            }
        }
    }

    public long ReportReclaimedSpace()
    {
        long reclaimedSpace = 0;
        if (Directory.Exists(_windowsUpdateLogDir))
        {
            var di = new DirectoryInfo(_windowsUpdateLogDir);
            foreach (var file in di.GetFiles())
            {
                reclaimedSpace += file.Length;
            }
        }
        return reclaimedSpace;
    }
}
