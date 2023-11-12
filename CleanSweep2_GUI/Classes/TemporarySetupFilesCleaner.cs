using System;
using System.IO;
using System.Linq;

public class TemporarySetupFilesCleaner
{
    private string _tempSetupDirectory;
    private long _tempSetupDirSizeInMegaBytes;

    public TemporarySetupFilesCleaner(string tempSetupDirectory)
    {
        _tempSetupDirectory = tempSetupDirectory;
    }

    public long GetReclaimableSpace()
    {
        if (Directory.Exists(_tempSetupDirectory))
        {
            _tempSetupDirSizeInMegaBytes = Directory.GetFiles(_tempSetupDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
        }
        return _tempSetupDirSizeInMegaBytes;
    }

    public void Reclaim()
    {
        var di = new DirectoryInfo(_tempSetupDirectory);

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
        if (Directory.Exists(_tempSetupDirectory))
        {
            newSize = Directory.GetFiles(_tempSetupDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
        }
        return _tempSetupDirSizeInMegaBytes - newSize;
    }
}
