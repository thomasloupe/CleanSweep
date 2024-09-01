using CleanSweep.Interfaces;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

public class UserFileHistoryCleaner : ICleaner
{
    private readonly RichTextBox _outputWindow;

    public UserFileHistoryCleaner(RichTextBox outputWindow)
    {
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        return ("User File History", 0);
    }

    public async Task Reclaim()
    {
        if (!IsFileHistoryEnabled())
        {
            Console.WriteLine("User File History is not enabled. Skipping cleanup.");
            return;
        }

        try
        {
            await Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/C FhManagew.exe -cleanup 0",
                        UseShellExecute = true,
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"User File History Cleaner: Error during User File History cleanup: {ex.Message}");
        }
        RichTextBoxExtensions.AppendText(_outputWindow, "if enabled, Windows File History has been removed!\n", Color.Green);
    }

    private bool IsFileHistoryEnabled()
    {
        const string fileHistoryRegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\FileHistory";

        try
        {
            object enabledValue = Registry.GetValue(fileHistoryRegistryKey, "Enabled", 0);
            return enabledValue != null && (int)enabledValue == 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking File History status: {ex.Message}");
            return false;
        }
    }
}
