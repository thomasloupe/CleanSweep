using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class EventViewerLogsCleaner
{
    public async Task Reclaim()
    {
        await Task.Run(() =>
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "wevtutil.exe",
                        Arguments = "cl Application && wevtutil.exe cl Security && wevtutil.exe cl System",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Event Viewer log cleanup: {ex.Message}");
            }
        });
    }
}
