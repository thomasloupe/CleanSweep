using System;
using System.IO;
using System.Linq;

public class TemporaryInternetFilesCleaner
{
    private string _tempInternetFilesDirectory;
    private long _tempInternetFilesSizeInMegaBytes;

    public TemporaryInternetFilesCleaner(string tempInternetFilesDirectory)
    {
        _tempInternetFilesDirectory = tempInternetFilesDirectory;
    }

    public long GetReclaimableSpace()
    {
        if (Directory.Exists(_tempInternetFilesDirectory))
        {
            _tempInternetFilesSizeInMegaBytes = Directory.GetFiles(_tempInternetFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
        }
        return _tempInternetFilesSizeInMegaBytes;
    }

    public void Reclaim()
    {
        var di = new DirectoryInfo(_tempInternetFilesDirectory);

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
        if (Directory.Exists(_tempInternetFilesDirectory))
        {
            newSize = Directory.GetFiles(_tempInternetFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
        }
        return _tempInternetFilesSizeInMegaBytes - newSize;
    }
}
