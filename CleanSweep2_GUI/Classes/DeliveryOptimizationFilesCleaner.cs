using CleanSweep2.Interfaces;
using System;
using System.IO;
using System.Linq;

public class DeliveryOptimizationFilesCleaner : ICleaner
{
    private readonly string _deliveryOptimizationFilesDirectory;
    private long _deliveryOptimizationFilesDirSizeInMegaBytes; // Added this field

    public DeliveryOptimizationFilesCleaner(string deliveryOptimizationFilesDirectory)
    {
        _deliveryOptimizationFilesDirectory = deliveryOptimizationFilesDirectory;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        long sizeInBytes = 0;
        if (Directory.Exists(_deliveryOptimizationFilesDirectory))
        {
            sizeInBytes = Directory.GetFiles(_deliveryOptimizationFilesDirectory, "*", SearchOption.AllDirectories)
                                   .Sum(file => (new FileInfo(file).Length));
        }

        // Ensure safe conversion from long to int
        int sizeInMB = ConvertBytesToMegabytes(sizeInBytes);
        _deliveryOptimizationFilesDirSizeInMegaBytes = sizeInMB; // Store the converted value
        return ("Delivery Optimization Files", sizeInMB);
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
                Console.WriteLine($"Error deleting file {file.FullName}: {ex.Message}");
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
                Console.WriteLine($"Error deleting directory {dir.FullName}: {ex.Message}");
            }
        }
    }

    public long ReportReclaimedSpace()
    {
        long newSize = 0;
        if (Directory.Exists(_deliveryOptimizationFilesDirectory))
        {
            newSize = Directory.GetFiles(_deliveryOptimizationFilesDirectory, "*", SearchOption.AllDirectories)
                               .Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
        }
        return _deliveryOptimizationFilesDirSizeInMegaBytes - newSize;
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        // Avoid overflow by capping the value at int.MaxValue
        long sizeInMB = bytes / 1024 / 1024;
        return (int)Math.Min(sizeInMB, int.MaxValue);
    }
}
