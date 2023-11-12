using System;
using System.IO;
using System.Linq;

public class DeliveryOptimizationFilesCleaner
{
    private string _deliveryOptimizationFilesDirectory;
    private long _deliveryOptimizationFilesDirSizeInMegaBytes;

    public DeliveryOptimizationFilesCleaner(string deliveryOptimizationFilesDirectory)
    {
        _deliveryOptimizationFilesDirectory = deliveryOptimizationFilesDirectory;
    }

    public long GetReclaimableSpace()
    {
        if (Directory.Exists(_deliveryOptimizationFilesDirectory))
        {
            _deliveryOptimizationFilesDirSizeInMegaBytes = Directory.GetFiles(_deliveryOptimizationFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
        }
        return _deliveryOptimizationFilesDirSizeInMegaBytes;
    }

    public void Reclaim()
    {
        var di = new DirectoryInfo(_deliveryOptimizationFilesDirectory);

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
        if (Directory.Exists(_deliveryOptimizationFilesDirectory))
        {
            newSize = Directory.GetFiles(_deliveryOptimizationFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
        }
        return _deliveryOptimizationFilesDirSizeInMegaBytes - newSize;
    }
}
