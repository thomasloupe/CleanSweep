﻿using System.Diagnostics;
using System.Threading.Tasks;

public class EmptyRecycleBinCleaner
{
    public long GetReclaimableSpace()
    {
        // This operation may not easily support a pre-calculation of reclaimable space
        return 0;
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
        // This operation may not easily support reporting reclaimed space
        return 0;
    }
}
