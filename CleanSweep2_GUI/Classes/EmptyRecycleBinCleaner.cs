using System.Diagnostics;
using System.Threading.Tasks;
using CleanSweep2.Interfaces;
using System.IO;
using System.Linq;

public class EmptyRecycleBinCleaner : ICleaner
{
    private long _preCleanupSize;

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = CalculateRecycleBinSize();
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Recycle Bin", spaceInMB);
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
                    Arguments = "/C rd /s %systemdrive%\\$Recycle.bin",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();
        });
    }

    public long ReportReclaimedSpace()
    {
        long postCleanupSize = CalculateRecycleBinSize();
        long reclaimedSpace = _preCleanupSize - postCleanupSize;
        return ConvertBytesToMegabytes(reclaimedSpace);
    }

    private long CalculateRecycleBinSize()
    {
        long totalSize = 0;

        foreach (var drive in DriveInfo.GetDrives())
        {
            if (drive.IsReady)
            {
                string recycleBinPath = Path.Combine(drive.RootDirectory.FullName, "$Recycle.Bin");
                if (Directory.Exists(recycleBinPath))
                {
                    totalSize += Directory.GetFiles(recycleBinPath, "*", SearchOption.AllDirectories)
                                          .Sum(file => new FileInfo(file).Length);
                }
            }
        }

        return totalSize;
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        return (int)(bytes / 1024 / 1024);
    }
}
