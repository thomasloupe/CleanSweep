using CleanSweep.Interfaces;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

public class ThumbnailCacheCleaner : ICleaner
{
    private readonly RichTextBox _outputWindow;

    public ThumbnailCacheCleaner(RichTextBox outputWindow)
    {
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        return ("Thumbnail Cache", 0);
    }

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
                        FileName = "cmd.exe",
                        Arguments = "/C taskkill /f /im explorer.exe & timeout 1 & del /f /s /q /a %LocalAppData%\\Microsoft\\Windows\\Explorer\\thumbcache_*.db & timeout 1 & start %windir%\\explorer.exe",
                        UseShellExecute = true,
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Thumbnail Cache Cleaner: Error during thumbnail cache cleanup: {ex.Message}");
            }
        });
        RichTextBoxExtensions.AppendText(_outputWindow, "Windows Thumbnail Cache Cleaned...\n", Color.Green);
    }
}
