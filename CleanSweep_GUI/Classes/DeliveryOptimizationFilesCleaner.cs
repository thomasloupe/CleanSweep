﻿using CleanSweep.Interfaces;
using System;
using System.Drawing;
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
        long sizeInBytes = 0;
        if (Directory.Exists(_deliveryOptimizationFilesDirectory))
        {
            sizeInBytes = Directory.GetFiles(_deliveryOptimizationFilesDirectory, "*", SearchOption.AllDirectories)
                                   .Sum(file => (new FileInfo(file).Length));
        }

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

        RichTextBoxExtensions.AppendText(_outputWindow, "Delivery Optimization Files cleaned!\n", Color.Green);
        return Task.CompletedTask;
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
        long sizeInMB = bytes / 1024 / 1024;
        return (int)Math.Min(sizeInMB, int.MaxValue);
    }
}
