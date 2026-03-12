using CleanSweep.Interfaces;
using System;
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
        _outputWindow = outputWindow;

        try
        {
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data");

            // Check if the base path exists before proceeding
            if (!Directory.Exists(basePath))
            {
                RichTextBoxExtensions.AppendText(_outputWindow, $"Chrome user data directory not found at {basePath}.\n");
                return; // Exit the constructor early if base path doesn't exist
            }

            var directories = new DirectoryInfo(basePath).GetDirectories()
                .Where(dir => dir.Name.StartsWith("Profile ") || dir.Name == "Default")
                .Select(dir => Path.Combine(dir.FullName, "Cache"))
                .ToList();

            if (directories.Count == 0)
            {
                // If no directories found, add default cache path
                directories.Add(Path.Combine(basePath, "Default", "Cache"));
            }

            _chromeCacheDirectories = directories.ToArray();
        }
        catch (Exception ex)
        {
            RichTextBoxExtensions.AppendText(_outputWindow, $"Error initializing ChromeCacheCleaner: {ex.Message}\n");
            _chromeCacheDirectories = Array.Empty<string>(); // Initialize as empty array to prevent further errors
        }
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        if (_chromeCacheDirectories == null || _chromeCacheDirectories.Length == 0)
        {
            return ("Chrome Cache Files", 0); // Return 0 space if no directories are found
        }

        _preCleanupSize = _chromeCacheDirectories.Sum(CalculateDirectorySize);
        int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
        return ("Chrome Cache Files", spaceInMB);
    }

    public async Task Reclaim()
    {
        if (_chromeCacheDirectories == null || _chromeCacheDirectories.Length == 0)
        {
            RichTextBoxExtensions.AppendText(_outputWindow, "No Chrome cache directories to clean.\n");
            return; // Exit if no directories are found
        }

        await Task.Run(() =>
        {
            foreach (var chromeDirectory in _chromeCacheDirectories)
            {
                if (Directory.Exists(chromeDirectory))
                {
                    try
                    {
                        Directory.Delete(chromeDirectory, true);
                        RichTextBoxExtensions.AppendText(_outputWindow, $"Deleted Chrome cache directory: {chromeDirectory}\n");
                    }
                    catch (Exception ex)
                    {
                        RichTextBoxExtensions.AppendText(_outputWindow, $"Error deleting Chrome cache directory {chromeDirectory}: {ex.Message}\n");
                    }
                }
                else
                {
                    RichTextBoxExtensions.AppendText(_outputWindow, $"Chrome cache directory not found: {chromeDirectory}\n");
                }
            }
        });

        RichTextBoxExtensions.AppendText(_outputWindow, "Chrome Cache cleaned!\n");
    }

    private long CalculateDirectorySize(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return 0;

        try
        {
            return new DirectoryInfo(directoryPath)
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Sum(file => file.Length);
        }
        catch (Exception ex)
        {
            RichTextBoxExtensions.AppendText(_outputWindow, $"Error calculating size for directory {directoryPath}: {ex.Message}\n");
            return 0;
        }
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        return (int)(bytes / 1024 / 1024);
    }
}
