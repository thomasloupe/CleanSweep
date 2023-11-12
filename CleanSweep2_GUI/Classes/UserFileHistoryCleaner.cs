using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class UserFileHistoryCleaner
{
    private readonly bool _showOperationWindows;

    public UserFileHistoryCleaner(bool showOperationWindows)
    {
        _showOperationWindows = showOperationWindows;
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
                        WindowStyle = _showOperationWindows ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();
            });
        }
        catch (Exception ex)
        {
            // Enhanced exception handling
            Console.WriteLine($"Error during User File History cleanup: {ex.Message}");
        }
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
