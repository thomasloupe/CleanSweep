using CleanSweep.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class ChromeCacheCleaner : ICleaner
{
    private readonly string[] _chromeCacheDirectories;
    private long _preCleanupSize;
    private readonly RichTextBox _outputWindow;

    public ChromeCacheCleaner(RichTextBox outputWindow)
    {
        var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data");
        var directories = new DirectoryInfo(basePath).GetDirectories()
                                   .Where(dir => dir.Name.StartsWith("Profile ") || dir.Name == "Default")
                                   .Select(dir => Path.Combine(dir.FullName, "Cache")).ToList();
        if (directories.Count == 0)
        {
            directories.Add(Path.Combine(basePath, "Default", "Cache"));
        }
        _chromeCacheDirectories = directories.ToArray();
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        _preCleanupSize = _chromeCacheDirectories.Sum(CalculateDirectorySize);
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Chrome Cache Files", spaceInMB);
    }

    public async Task Reclaim()
    {
        await Task.Run(() =>
        {
            foreach (var chromeDirectory in _chromeCacheDirectories)
            {
                if (Directory.Exists(chromeDirectory))
                {
                    try
                    {
                        Directory.Delete(chromeDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting Chrome cache directory {chromeDirectory}: {ex.Message}");
                    }
                }
            }
        });

        RichTextBoxExtensions.AppendText(_outputWindow, "Chrome Cache cleaned!\n", Color.Green);
    }

    private long CalculateDirectorySize(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return 0;

        return new DirectoryInfo(directoryPath)
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Sum(file => file.Length);
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        return (int)(bytes / 1024 / 1024);
    }
}
