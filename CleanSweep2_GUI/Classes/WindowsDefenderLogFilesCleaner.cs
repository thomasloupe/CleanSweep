using System;
using System.IO;
using System.Threading.Tasks;

public class WindowsDefenderLogFilesCleaner
{
    private string _programDataDirectory;
    private bool _isVerboseMode;

    public WindowsDefenderLogFilesCleaner(string programDataDirectory, bool isVerboseMode)
    {
        _programDataDirectory = programDataDirectory;
        _isVerboseMode = isVerboseMode;
    }

    public long GetReclaimableSpace()
    {
        // This operation may not easily support a pre-calculation of reclaimable space
        return 0;
    }

    public async Task Reclaim()
    {
        string[] directoryPath = new[]
        {
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Network Inspection System\\Support\\",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Service\\"
        };

        string[] defenderLogDirectoryPaths = new[]
        {
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\ReportLatency\\Latency",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Results\\Resource",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Results\\Quick",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\CacheManager",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\MetaStore",
            _programDataDirectory + "\\Microsoft\\Windows Defender\\Support"
        };

        await Task.Run(() =>
        {
            foreach (var directory in directoryPath)
            {
                var dir = new DirectoryInfo(directory);
                foreach (var f in dir.GetFiles())
                {
                    if (f.Name.Contains(".log"))
                    {
                        try
                        {
                            File.Delete(f.FullName);
                        }
                        catch (Exception)
                        {
                            // Handle exceptions as needed
                        }
                    }
                }
            }

            foreach (var logFileDirectory in defenderLogDirectoryPaths)
            {
                if (Directory.Exists(logFileDirectory))
                {
                    try
                    {
                        Directory.Delete(logFileDirectory, true);
                    }
                    catch (Exception)
                    {
                        // Handle exceptions as needed
                    }
                }
            }
        });
    }

    public long ReportReclaimedSpace()
    {
        // This operation may not easily support reporting reclaimed space
        return 0;
    }
}
