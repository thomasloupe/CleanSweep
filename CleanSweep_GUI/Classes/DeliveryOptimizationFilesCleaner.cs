using CleanSweep.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class DeliveryOptimizationFilesCleaner : ICleaner
{
    private readonly string _deliveryOptimizationFilesDirectory;
    private long _deliveryOptimizationFilesDirSizeInMegaBytes;
    private readonly RichTextBox _outputWindow;

    public DeliveryOptimizationFilesCleaner(RichTextBox outputWindow)
    {
        _deliveryOptimizationFilesDirectory = @"C:\Windows\SoftwareDistribution\Download";
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        long sizeInBytes = CalculateDirectorySize(_deliveryOptimizationFilesDirectory);

        int sizeInMB = ConvertBytesToMegabytes(sizeInBytes);
        _deliveryOptimizationFilesDirSizeInMegaBytes = sizeInMB;
        return ("Delivery Optimization Files", sizeInMB);
    }

    public Task Reclaim()
    {
        var di = new DirectoryInfo(_deliveryOptimizationFilesDirectory);

        try
        {
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
        catch (Exception ex)
        {
            Console.WriteLine($"Delivery Optimization Files Cleaner: Error during Delivery Optimization files cleanup: {ex.Message}");
        }

        RichTextBoxExtensions.AppendText(_outputWindow, "Delivery Optimization Files cleaned!\n");
        return Task.CompletedTask;
    }

    public long ReportReclaimedSpace()
    {
        long newSize = ConvertBytesToMegabytes(CalculateDirectorySize(_deliveryOptimizationFilesDirectory));
        return _deliveryOptimizationFilesDirSizeInMegaBytes - newSize;
    }

    private long CalculateDirectorySize(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return 0;

        long totalSize = 0;
        try
        {
            foreach (var file in new DirectoryInfo(directoryPath).EnumerateFiles("*", SearchOption.AllDirectories))
            {
                try { totalSize += file.Length; }
                catch { }
            }
        }
        catch { }
        return totalSize;
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        long sizeInMB = bytes / 1024 / 1024;
        return (int)Math.Min(sizeInMB, int.MaxValue);
    }
}
