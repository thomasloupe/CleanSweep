using System;
using System.IO;
using System.Linq;

public class WindowsErrorReportsCleaner
{
    private string _windowsErrorReportsDirectory;
    private long _windowsErrorReportsDirSizeInMegaBytes;

    public WindowsErrorReportsCleaner(string windowsErrorReportsDirectory)
    {
        _windowsErrorReportsDirectory = windowsErrorReportsDirectory;
    }

    public long GetReclaimableSpace()
    {
        if (Directory.Exists(_windowsErrorReportsDirectory))
        {
            _windowsErrorReportsDirSizeInMegaBytes = Directory.GetFiles(_windowsErrorReportsDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
        }
        return _windowsErrorReportsDirSizeInMegaBytes;
    }

    public void Reclaim()
    {
        var di = new DirectoryInfo(_windowsErrorReportsDirectory);

        foreach (var file in di.GetFiles())
        {
            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
            }
        }

        foreach (var dir in di.GetDirectories())
        {
            try
            {
                dir.Delete(true);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
            }
        }
    }

    public long ReportReclaimedSpace()
    {
        long newSize = 0;
        if (Directory.Exists(_windowsErrorReportsDirectory))
        {
            newSize = Directory.GetFiles(_windowsErrorReportsDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
        }
        return _windowsErrorReportsDirSizeInMegaBytes - newSize;
    }
}
