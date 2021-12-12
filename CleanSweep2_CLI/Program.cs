using Octokit;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using CleanSweep2_CLI.Properties;
using System.Text.RegularExpressions;

namespace CleanSweep2_CLI
{
    internal class CleanSweep2_CLI
    {
        #region Declarations
        private const string CurrentVersion = "v2.3.0";
        private readonly string _userName = Environment.UserName;
        private readonly string _systemDrive = Path.GetPathRoot(Environment.SystemDirectory);
        private readonly string _windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        private readonly string _programDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        private readonly string _localAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private bool _isVerboseMode;
        private bool _showOperationWindows;
        private bool _tempFilesWereRemoved = false;
        private bool _tempSetupFilesWereRemoved = false;
        private bool _tempInternetFilesWereRemoved = false;
        private bool _eventLogsCleared = false;
        private bool _isRecycleBinEmpty = false;
        private bool _windowsErrorReportsCleared = false;
        private bool _wereDeliveryOptimizationFilesRemoved = false;
        private bool _thumbnailCacheCleared = false;
        private bool _deletedFileHistory = false;
        private bool _windowsOldCleaned = false;
        private bool _windowsDefenderLogsCleared = false;
        private bool _sweptChromeCache = false;
        private bool _sweptEdgeCache = false;
        private bool _msoCacheCleared = false;
        private bool _windowsInstallerCacheCleared = false;
        private bool _windowsUpdateLogsCleared = false;
        private long _totalSpaceSaved;
        private string _logPath;

        // Temporary Files
        private string _tempDirectory;
        private long _tempDirSize;
        private long _tempDirSizeInMegaBytes;
        private long _tempSizeBeforeDelete;
        private long _newTempDirSize;

        // Temporary Setup Files
        private string _tempSetupDirectory;
        private long _tempSetupDirSize;
        private long _tempSetupSizeInMegabytes;
        private long _tempSetupFilesSizeBeforeDelete;
        private long _newTempSetupDirSize;

        // Temporary Internet Files
        private string _tempInternetFilesDirectory;
        private long _tempInternetFilesDirSize;
        private long _tempInternetSizeInMegabytes;
        private long _tempInternetFilesBeforeDelete;
        private long _newTempInternetFilesDirSize;

        // Windows Error Reports
        private string _windowsErrorReportsDirectory;
        private long _windowsErrorReportsDirSize;
        private long _windowsErrorReportsDirSizeInMegabytes;
        private long _windowsErrorReportsDirSizeBeforeDelete;
        private long _newWindowsErrorReportsDirSize;

        // Delivery Optimization Files
        private string _deliveryOptimizationFilesDirectory;
        private long _deliveryOptimizationFilesDirSize;
        private long _deliveryOptimizationFilesDirSizeInMegabytes;
        private long _deliveryOptimizationFilesDirSizeBeforeDelete;
        private long _newDeliveryOptimizationSize;

        // Chrome Directories
        private readonly string[] _chromeCacheDirectories = new string[6];
        private long _totalChromeDirSize = 0;

        // Edge Directories
        private readonly string[] _edgeCacheDirectories = new string[12];
        private long _totalEdgeDirSize = 0;

        // MSO Cache Directories
        private string _msoCacheDir;
        private long _msoCacheDirSize = 0;
        private long _newMsoCacheDirSize;

        // Windows Installer Cache Directories
        private string _windowsInstallerCacheDir;
        private long _windowsInstallerCacheDirSize;
        private long _newWindowsInstallerCacheDirSize;

        // Windows Update Log Directories.
        private string _windowsUpdateLogDir;
        private long _windowsUpdateLogDirSize;
        private long _newWindowsUpdateLogDirSize;
        #endregion
        private static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            var cs2Cli = new CleanSweep2_CLI();
            ShowApplicationInfo();
            cs2Cli.GetSizeInformation();
            Console.WriteLine();


            // TODO: Disable verbosity level 1/2 dependent on which was already selected in args.
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-1":
                        {
                            cs2Cli.Option1();
                            break;
                        }
                    case "-2":
                        {
                            cs2Cli.Option2();
                            break;
                        }
                    case "-3":
                        {
                            cs2Cli.Option3();
                            break;
                        }
                    case "-4":
                        {
                            cs2Cli.Option4();
                            break;
                        }
                    case "-5":
                        {
                            cs2Cli.Option5();
                            break;
                        }
                    case "-6":
                        {
                            cs2Cli.Option6();
                            break;
                        }
                    case "-7":
                        {
                            cs2Cli.Option7();
                            break;
                        }
                    case "-8":
                        {
                            cs2Cli.Option8();
                            break;
                        }
                    case "-9":
                        {
                            cs2Cli.Option9();
                            break;
                        }
                    case "-10":
                        {
                            cs2Cli.Option10();
                            break;
                        }
                    case "-11":
                        {
                            cs2Cli.Option11();
                            break;
                        }
                    case "-12":
                        {
                            cs2Cli.Option12();
                            break;
                        }
                    case "-13":
                        {
                            cs2Cli.Option13();
                            break;
                        }
                    case "-14":
                        {
                            cs2Cli.Option14();
                            break;
                        }
                    case "-15":
                        {
                            cs2Cli.Option15();
                            break;
                        }
                    case "-16":
                        {
                            cs2Cli.Option16();
                            break;
                        }
                    case "-log":
                        {
                            // Set the path for logging.
                            foreach (Match match in Regex.Matches(cs2Cli._logPath, "\"([^\"]*)\""))
                            {
                                var tempLogPath = cs2Cli._logPath.Replace(@"""", "");
                                cs2Cli._logPath = tempLogPath;
                            }
                            break;
                        }
                    case "-update":
                    {
                        CheckForUpdates();
                        break;
                    }
                    case "-v1":
                        {
                            // Set verbosity to low. Default to highest level passed.
                            cs2Cli._isVerboseMode = args.Contains("v2");
                            break;
                        }
                    case "-v2":
                        {
                            // Set verbosity to low. Default to highest level passed.
                            cs2Cli._isVerboseMode = args.Contains("v1");
                            break;
                        }
                    case "-visible":
                        {
                            // Show CMD window via method.
                            cs2Cli._showOperationWindows = false;
                            break;
                        }
                }
            }
            cs2Cli.CalculateSpaceSaved();
        }
        #region Get Size Information
        private void GetSizeInformation()
        {
            // Set Chrome cache directories.
            _chromeCacheDirectories[0] = _localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\Cache";
            _chromeCacheDirectories[1] = _localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\Media Cache";
            _chromeCacheDirectories[2] = _localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\GPUCache";
            _chromeCacheDirectories[3] = _localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\Storage\\ext";
            _chromeCacheDirectories[4] = _localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\Service Worker";
            _chromeCacheDirectories[5] = _localAppDataDirectory + "\\Google\\Chrome\\User Data\\ShaderCache";

            // Set Edge cache directories.
            _edgeCacheDirectories[0] = _localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\Cache";
            _edgeCacheDirectories[1] = _localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\Media Cache";
            _edgeCacheDirectories[2] = _localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\GPUCache";
            _edgeCacheDirectories[3] = _localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\Storage\\ext";
            _edgeCacheDirectories[4] = _localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\Service Worker";
            _edgeCacheDirectories[5] = _localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\ShaderCache";
            _edgeCacheDirectories[6] = _localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\Cache";
            _edgeCacheDirectories[7] = _localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\Media Cache";
            _edgeCacheDirectories[8] = _localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\GPUCache";
            _edgeCacheDirectories[9] = _localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\Storage\\ext";
            _edgeCacheDirectories[10] = _localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\Service Worker";
            _edgeCacheDirectories[11] = _localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\ShaderCache";

            // Set MSOCache Directory.
            _msoCacheDir = _systemDrive + "MSOCache";

            // Set Windows Installer Cache Directory.
            _windowsInstallerCacheDir = _windowsDirectory + "\\Installer\\$PatchCache$\\Managed";

            // Set Windows Log Directory.
            _windowsUpdateLogDir = _windowsDirectory + "\\Logs\\";

            // Get size of Temporary Files.
            _tempDirectory = "C:\\Users\\" + _userName + "\\AppData\\Local\\Temp\\";
            try
            {
                _tempDirSize = Directory.GetFiles(_tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
            _tempDirSizeInMegaBytes = _tempDirSize / 1024 / 1024;

            // Get size of Temporary Setup Files.
            _tempSetupDirectory = _windowsDirectory + "\\temp";
            _tempSetupDirSize = Directory.GetFiles(_tempSetupDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            _tempSetupSizeInMegabytes = _tempSetupDirSize / 1024 / 1024;

            // Get size of Temporary Internet Files.
            _tempInternetFilesDirectory = "C:\\Users\\" + _userName + "\\AppData\\Local\\Microsoft\\Windows\\INetCache\\";
            try
            {
                _tempInternetFilesDirSize = Directory.GetFiles(_tempInternetFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
            _tempInternetSizeInMegabytes = _tempInternetFilesDirSize / 1024 / 1024;

            // Get size of Windows Error Reports
            _windowsErrorReportsDirectory = _programDataDirectory + "\\Microsoft\\Windows\\WER\\ReportArchive";
            if (Directory.Exists(_windowsErrorReportsDirectory))
            {
                _windowsErrorReportsDirSize = Directory.GetFiles(_windowsErrorReportsDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                _windowsErrorReportsDirSizeInMegabytes = _windowsErrorReportsDirSize / 1024 / 1024;
            }
            else
            {
                _windowsErrorReportsDirSizeInMegabytes = 0;
            }

            // Get size of Windows Delivery Optimization Files
            _deliveryOptimizationFilesDirectory = _windowsDirectory + "\\SoftwareDistribution\\";
            _deliveryOptimizationFilesDirSize = Directory.GetFiles(_deliveryOptimizationFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            _deliveryOptimizationFilesDirSizeInMegabytes = _deliveryOptimizationFilesDirSize / 1024 / 1024;

            // Get size of Chrome cache directories.
            foreach (string chromeDirectory in _chromeCacheDirectories)
            {
                if (Directory.Exists(chromeDirectory))
                {
                    long thisChromeDirSize = 0;
                    thisChromeDirSize = Directory.GetFiles(chromeDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                    _totalChromeDirSize += thisChromeDirSize;
                }
            }
            // Get size of Edge cache directories.
            foreach (string edgeDirectory in _edgeCacheDirectories)
            {
                if (Directory.Exists(edgeDirectory))
                {
                    long thisEdgeDirSize = 0;
                    thisEdgeDirSize = Directory.GetFiles(edgeDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                    _totalEdgeDirSize += thisEdgeDirSize;
                }
            }

            // Get size of MSOCache.
            if (Directory.Exists(_msoCacheDir))
            {
                try
                {
                    _msoCacheDirSize = Directory.GetFiles(_msoCacheDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IOException))
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else if (ex.GetType() == typeof(UnauthorizedAccessException))
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }

            // Get size of Windows Installer Cache.
            if (Directory.Exists(_windowsInstallerCacheDir))
            {
                _windowsInstallerCacheDirSize = Directory.GetFiles(_windowsInstallerCacheDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
            }

            // Get size of Windows Update Logs directory.
            if (Directory.Exists(_windowsUpdateLogDir))
            {
                _windowsUpdateLogDirSize = Directory.GetFiles(_windowsUpdateLogDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
            }

            // Show potential reclamation, then each individual category.
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Resources.Potential_space_to_reclaim +
                              (_tempDirSizeInMegaBytes + _tempSetupSizeInMegabytes + _tempInternetSizeInMegabytes + 
                               _windowsErrorReportsDirSizeInMegabytes + _deliveryOptimizationFilesDirSizeInMegabytes + 
                               _totalChromeDirSize + _totalEdgeDirSize + _msoCacheDirSize + _windowsInstallerCacheDirSize + 
                               _windowsUpdateLogDirSize) + Resources.MB);
            Console.WriteLine(Resources.Categorical_breakdown);
            Console.WriteLine(Resources.LineSeparator);

            // List out total space reclamation per category.
            Console.WriteLine(Resources.Temporary_Directory_size + _tempDirSizeInMegaBytes + Resources.MB);
            Console.WriteLine(Resources.Temporary_Setup_Files_directory_size + _tempSetupSizeInMegabytes + Resources.MB);
            Console.WriteLine(Resources.Temporary_Internet_Files_directory_size + _tempInternetSizeInMegabytes + Resources.MB);
            Console.WriteLine(Resources.Windows_Error_Reports_size + _windowsErrorReportsDirSizeInMegabytes + Resources.MB);
            Console.WriteLine(Resources.Windows_Delivery_Optimization_File_size + _deliveryOptimizationFilesDirSizeInMegabytes + Resources.MB);
            Console.WriteLine(Resources.Chrome_Data_Size + _totalChromeDirSize + Resources.MB);
            Console.WriteLine(Resources.Edge_Data_Size + _totalEdgeDirSize + Resources.MB);
            Console.WriteLine(Resources.MSO_Cache_size + _msoCacheDirSize + Resources.MB);
            Console.WriteLine(Resources.Windows_Installer_Cache_size + _windowsInstallerCacheDirSize + Resources.MB);
            Console.WriteLine(Resources.Windows_Update_Logs_size + _windowsUpdateLogDirSize + Resources.MB);
        }
        #endregion

        #region Temporary Files Removal (1)
        private void Option1()
        {
            // Temporary Files removal.
            _tempSizeBeforeDelete = _tempDirSizeInMegaBytes;
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Resources.Sweeping_Temporary_Files);
            var di = new DirectoryInfo(_tempDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    // If file no longer exists, append success to rich text box.
                    if (!File.Exists(file.Name))
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Resources.Deleted + file.Name);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Resources.o);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed files.
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Resources.Couldn_t_delete + file.Name + Resources.Colon_Symbol + ex.Message + Resources.Skipping);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Resources.x);
                    }
                }
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    if (!Directory.Exists(dir.Name))
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Resources.Deleted + dir.Name);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Resources.Folder_o);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed directories.
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Resources.Couldn_t_delete + dir.Name + Resources.Colon_Symbol + ex.Message + Resources.Skipping);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Resources.Folder_x);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.Write(Resources.Swept_Temporary_Files);
            Console.WriteLine();
            Console.WriteLine();
            _tempFilesWereRemoved = true;
        }
        #endregion

        #region Temporary Setup Files Removal (2)
        private void Option2()
        {
            // Temporary Setup Files removal
            _tempSetupFilesSizeBeforeDelete = _tempSetupSizeInMegabytes;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Resources.Sweeping_Temporary_Setup_Files);
            Console.WriteLine();
            var di = new DirectoryInfo(_tempSetupDirectory);

            foreach (var file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    if (!File.Exists(file.Name))
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Resources.Deleted + file.Name);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Resources.o);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed files.
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Resources.Couldn_t_delete + file.Name + Resources.Colon_Symbol + ex.Message + Resources.Skipping);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Resources.x);
                    }
                }
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    if (Directory.Exists(dir.Name))
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Resources.Deleted + dir.Name);
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Resources.Folder_o);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed directories.
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Resources.Couldn_t_delete + dir.Name + Resources.Colon_Symbol + ex.Message + Resources.Skipping);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Resources.Folder_x);
                    }
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Swept_Temporary_Setup_Files);
            Console.WriteLine();
            _tempSetupFilesWereRemoved = true;
        }
        #endregion

        #region Temporary Internet Files Removal (3)
        private void Option3()
        {
            // Temporary Setup Files removal
            _tempInternetFilesBeforeDelete = _tempInternetSizeInMegabytes;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Sweeping_Temporary_Internet_Files);
            var di = new DirectoryInfo(_tempInternetFilesDirectory);

            foreach (var file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    if (!File.Exists(file.Name))
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Resources.Deleted + file.Name);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Resources.o);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed files.
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Resources.Couldn_t_delete + file.Name + Resources.Colon_Symbol + ex.Message);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Resources.x);
                    }
                }
            }
            foreach (var dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    if (Directory.Exists(dir.Name))
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Resources.Deleted + dir.Name);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Resources.Folder_o);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed directories.
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Resources.Couldn_t_delete + dir.Name + Resources.Colon_Symbol + ex.Message + Resources.Skipping);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Resources.Folder_x);

                    }
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Swept_Temporary_Internet_Files);
            Console.WriteLine();
            _tempInternetFilesWereRemoved = true;
        }
        #endregion

        #region Event Viewer Logs Removal (4)
        private void Option4()
        {
            // Event Viewer Logs Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Resources.Sweeping_Event_Viewer_Logs);
            AddWaitText();
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (_showOperationWindows)
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            }
            else
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }
            startInfo.FileName = "cmd.exe";
            startInfo.UseShellExecute = true;
            startInfo.Arguments = "/C for /F \"tokens=*\" %1 in ('wevtutil.exe el') DO wevtutil.exe cl \"%1\"";
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();

            while (!process.HasExited)
            {
                Thread.Sleep(200);
                AddWaitText();
                if (process.HasExited)
                {
                    break;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.Write(Resources.Swept_Event_Viewer_Logs);
            Console.WriteLine();
            Console.WriteLine();
            _eventLogsCleared = true;
        }
        #endregion

        #region Empty Recycle Bin (5)
        private void Option5()
        {
            // Empty Recycle Bin.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Resources.Emptying_Recycle_Bin);
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (_showOperationWindows)
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            }
            else
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }
            startInfo.FileName = "cmd.exe";
            startInfo.UseShellExecute = true;
            startInfo.Arguments = "/C echo y| rd /s %systemdrive%\\$Recycle.bin";
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();

            while (!process.HasExited)
            {
                Thread.Sleep(200);
                AddWaitText();
                if (process.HasExited)
                {
                    break;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.Write(Resources.Emptied_Recycle_Bin);
            Console.WriteLine();
            Console.WriteLine();
            _isRecycleBinEmpty = true;
        }
        #endregion

        #region Windows Error Reports Removal (6)
        private void Option6()
        {
            // Temporary Setup Files removal
            _windowsErrorReportsDirSizeBeforeDelete = _windowsErrorReportsDirSizeInMegabytes;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Sweeping_Windows_Error_Reports);
            if (Directory.Exists(_windowsErrorReportsDirectory))
            {
                var di = new DirectoryInfo(_windowsErrorReportsDirectory);
                foreach (var file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        if (!File.Exists(file.Name))
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(Resources.Deleted + file.Name);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Resources.o);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Skip all failed files.
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(file.Name + Resources.appears_to_be_in_use_or_locked__Skipping);
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(Resources.x);
                        }
                    }
                }
                foreach (var dir in di.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true);
                        if (Directory.Exists(dir.Name))
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(Resources.Deleted + dir.Name);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Resources.Folder_o);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(Resources.Couldn_t_remove + dir.Name + Resources.Colon_Symbol + ex.Message + Resources.Skipping);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(Resources.Folder_x);
                        }
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Swept_Windows_Error_Reports);
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Windows_Error_Report_directory_already_removed__Skipping);
                Console.WriteLine();
            }
            _windowsErrorReportsCleared = true;
        }
        #endregion

        #region Delivery Optimization Files (7)
        private void Option7()
        {
            // Delivery Optimization Files Removal
            _deliveryOptimizationFilesDirSizeBeforeDelete = _deliveryOptimizationFilesDirSizeInMegabytes;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Sweeping_Delivery_Optimization_Files);
            var di = new DirectoryInfo(_deliveryOptimizationFilesDirectory);

            if (di.GetFiles().Length != 0)
            {
                foreach (var file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        if (!File.Exists(file.Name))
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(Resources.Deleted + file.Name);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Resources.o);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed files.
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(Resources.Couldn_t_delete + file.Name + Resources.Colon_Symbol + ex.Message + Resources.Skipping);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Resources.x);
                        }
                    }
                }
                foreach (var dir in di.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true);
                        if (Directory.Exists(dir.Name))
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(Resources.Deleted + dir.Name);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Resources.Folder_o);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(Resources.Couldn_t_delete + dir.Name + Resources.Colon_Symbol + ex.Message + Resources.Skipping);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(Resources.Folder_x);
                        }
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(Resources.Removed_Delivery_Optimization_Files);
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.No_Delivery_Optimization_Files_needed_to_be_cleaned);
                Console.WriteLine();
            }
            _wereDeliveryOptimizationFilesRemoved = true;
        }
        #endregion

        #region Thumbnail Cache Removal (8)
        private void Option8()
        {
            // Thumbnail Cache Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Resources.Clearing_Thumbnail_Cache);
            AddWaitText();
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (_showOperationWindows)
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            }
            else
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }
            startInfo.FileName = "cmd.exe";
            if (_isVerboseMode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.Write(Resources.Shutting_down_Explorer_exe_process);
            }
            startInfo.Arguments = "/C taskkill /f /im explorer.exe & timeout 1 & del / f / s / q / a %LocalAppData%\\Microsoft\\Windows\\Explorer\\thumbcache_ *.db & timeout 1 & start %windir%\\explorer.exe";
            startInfo.UseShellExecute = true;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();

            while (!process.HasExited)
            {
                Thread.Sleep(200);
                AddWaitText();
                if (process.HasExited)
                {
                    break;
                }
            }
            if (_isVerboseMode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(Resources.Restarted_Explorer_exe_process);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(Resources.Cleared_Thumbnail_Cache);
            Console.WriteLine();
            _thumbnailCacheCleared = true;
        }
        #endregion

        #region User File History Removal (9)
        private void Option9()
        {
            // User File History Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Resources.Attempting_to_remove_all_file_history_snapshots_except_latest);
            AddWaitText();
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (_showOperationWindows)
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            }
            else
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }
            startInfo.FileName = "cmd.exe";
            startInfo.UseShellExecute = true;
            startInfo.Arguments = "/C FhManagew.exe -cleanup 0";
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();

            while (!process.HasExited)
            {
                Thread.Sleep(200);
                AddWaitText();
                if (process.HasExited)
                {
                    break;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(Resources.If_file_history_was_enabled__all_versions_except_latest_were_removed);
            Console.WriteLine();
            _deletedFileHistory = true;
        }
        #endregion

        #region Windows.old Directory Removal (10)
        private void Option10()
        {
            // Windows.old Directory Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Removing_old_versions_of_Windows);
            if (Directory.Exists("C:\\windows.old"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Found_Windows_old_directory__Cleaning);
                AddWaitText();
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                if (_showOperationWindows)
                {
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                }
                else
                {
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                }
                startInfo.FileName = "cmd.exe";
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = "C:\\Windows\\";
                startInfo.Arguments = "/C takeown /F C:\\Windows.old* /R /A /D Y & cacls C:\\Windows.old*.* /T /grant administrators:F & rmdir /S /Q C:\\Windows.old";
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();

                while (!process.HasExited)
                {
                    Thread.Sleep(200);
                    AddWaitText();
                    if (process.HasExited)
                    {
                        break;
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Swept_Windows_old_directory);
                Console.WriteLine();
                _windowsOldCleaned = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.No_Windows_old_directory_found__Skipping);
                Console.WriteLine();
                _windowsOldCleaned = false;
            }
        }
        #endregion

        #region Windows Defender Log Files Removal (11)
        private void Option11()
        {
            // Windows Defender Log Files Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Deleting_Windows_Defender_Log_Files);

            // Create an array that contain paths to search for log files.
            var directoryPath = new []
            {
                _programDataDirectory + "\\Microsoft\\Windows Defender\\Network Inspection System\\Support\\",
                _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Service\\"
            };

            // Create an array that contains the directory paths for Defender Logs.
            var defenderLogDirectoryPaths = new []
            {
                _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\ReportLatency\\Latency",
                _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Results\\Resource",
                _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Results\\Quick",
                _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\CacheManager",
                _programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\MetaStore",
                _programDataDirectory + "\\Microsoft\\Windows Defender\\Support"
            };
            // Remove the log-files.
            foreach (string directory in directoryPath)
            {
                var dir = new DirectoryInfo(directory);
                foreach (var f in dir.GetFiles())
                {
                    if (f.Name.Contains(".log"))
                    {
                        try
                        {
                            File.Delete(f.FullName);
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(Resources.Removed + f.Name);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Resources.o);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(ex.Message + Resources.Skipping);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(Resources.x + f.Name);
                            }
                        }
                    }
                }
            }
            // Remove the directories.
            foreach (string logFileDirectory in defenderLogDirectoryPaths)
            {
                if (Directory.Exists(logFileDirectory))
                {
                    try
                    {
                        Directory.Delete(logFileDirectory, true);
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Resources.Removed + logFileDirectory);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Resources.Folder_o);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex.Message + Resources.Skipping___);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(Resources.Folder_x);
                        }
                    }
                }
                else
                {
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Resources.Directory_already_removed + logFileDirectory);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(Resources.Removed_Windows_Defender_Logs);
            Console.WriteLine();
            _windowsDefenderLogsCleared = true;
        }
        #endregion

        #region Microsoft Office Cache Removal (12)
        private void Option12()
        {
            // Microsoft Office Cache Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Sweeping_MSO_cache);
            if (Directory.Exists(_msoCacheDir))
            {
                Directory.Delete(_msoCacheDir, true);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Swept_MSO_Cache);
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.No_MSOCache_directory_found__Skipping);
                Console.WriteLine();
            }

            _msoCacheCleared = true;
        }
        #endregion

        #region Microsoft Edge Cache Removal (13)
        private void Option13()
        {
            // Edge Cache Removal
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Sweeping_Edge_cache);
            if (_isVerboseMode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Ending_any_Edge_processes);
            }
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (_showOperationWindows)
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            }
            else
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }
            startInfo.FileName = "cmd.exe";
            startInfo.UseShellExecute = true;
            startInfo.Arguments = "/C TASKKILL /F /IM msedge.exe";
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();

            while (!process.HasExited)
            {
                Thread.Sleep(200);
                if (_isVerboseMode)
                {
                    AddWaitText();
                }
                if (process.HasExited)
                {
                    break;
                }
                if (_isVerboseMode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Resources.Stopped_Edge_processes);
                }
                foreach (string edgeDirectory in _edgeCacheDirectories)
                {
                    if (Directory.Exists(edgeDirectory))
                    {
                        try
                        {
                            Directory.Delete(edgeDirectory, true);
                            if (!Directory.Exists(edgeDirectory))
                            {
                                if (_isVerboseMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine(Resources.Removed + edgeDirectory + Resources.Period);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(Resources.Folder_o);
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(Resources.Skipping + edgeDirectory + Resources.Period + ex.Message);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(Resources.Folder_x);
                            }
                        }
                    }
                    else
                    {
                        if (_isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Resources.Directory_already_removed + edgeDirectory + Resources.Period);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(Resources.Folder_x);
                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Swept_Edge_cache);
            Console.WriteLine();
            _sweptEdgeCache = true;
        }
        #endregion

        #region Chrome Cache Removal (14)
        private void Option14()
        {
            // Chrome Cache Removal
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Sweeping_Chrome_cache);
            if (_showOperationWindows)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Ending_any_Chrome_processes);
            }
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (_isVerboseMode)
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            }
            else
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }
            startInfo.FileName = "cmd.exe";
            startInfo.UseShellExecute = true;
            startInfo.Arguments = "/C TASKKILL Chrome.exe";
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();

            while (!process.HasExited)
            {
                Thread.Sleep(200);
                if (_isVerboseMode)
                {
                    AddWaitText();
                }
                if (process.HasExited)
                {
                    break;
                }
            }
            if (_isVerboseMode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(Resources.Stopped_Chrome_processes);
            }
            foreach (string chromeDirectory in _chromeCacheDirectories)
            {
                if (Directory.Exists(chromeDirectory))
                {
                    try
                    {
                        Directory.Delete(chromeDirectory, true);
                        if (!Directory.Exists(chromeDirectory))
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(Resources.Removed + chromeDirectory + Resources.Period);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(Resources.Folder_x);
                            }
                        }
                    }
                    catch (SystemException ex)
                    {
                        if (ex.GetType() == typeof(IOException))
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(Resources.Skipping + chromeDirectory + Resources.Period + ex.Message);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(Resources.Folder_x);
                            }
                        }
                        else if (ex.GetType() == typeof(UnauthorizedAccessException))
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(Resources.Skipping + chromeDirectory + Resources.Period + ex.Message);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(Resources.Folder_x);
                            }
                        }
                        else
                        {
                            if (_isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(Resources.Skipping + chromeDirectory + Resources.Period + ex.Message);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(Resources.Folder_x);
                            }
                        }
                    }
                }
                else
                {
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(Resources.Directory_already_removed + chromeDirectory + Resources.Period);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Resources.Folder_x);
                    }
                }
            }
            _sweptChromeCache = true;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(Resources.Swept_Chrome_cache);
            Console.WriteLine();
        }
        #endregion

        #region Windows Installer Cache Removal (15)
        private void Option15()
        {
            // Windows Installer Cache Removal
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Removing_Windows_Installer_Cache);
            if (Directory.Exists(_windowsDirectory + "\\Installer\\$PatchCache$\\Managed"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Found_Windows_Installer_Cache__Cleaning);
                AddWaitText();
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                if (_showOperationWindows)
                {
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                }
                else
                {
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                }
                startInfo.FileName = "cmd.exe";
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = "C:\\Windows\\";
                startInfo.Arguments = "/C rmdir /q /s %WINDIR%\\Installer\\$PatchCache$\\Managed";
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();

                while (!process.HasExited)
                {
                    Thread.Sleep(200);
                    AddWaitText();
                    if (process.HasExited)
                    {
                        break;
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Swept_Windows_Installer_Cache);
            Console.WriteLine();
            _windowsInstallerCacheCleared = true;
        }
        #endregion

        #region Windows Update Logs Removal (16)
        private void Option16()
        {
            // Windows Update Logs Removal
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Resources.Sweeping_Windows_Update_Logs);
            try
            {
                if (Directory.Exists(_windowsUpdateLogDir))
                {
                    var di = new DirectoryInfo(_tempDirectory);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                            // If file no longer exists, append success to rich text box.
                            if (!File.Exists(file.Name))
                            {
                                if (_isVerboseMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine(Resources.Deleted + file.Name);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(Resources.o);
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(Resources.x);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() == typeof(IOException))
                            {
                                if (_isVerboseMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine();
                                    Console.WriteLine(ex.Message);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(Resources.x);
                                }
                            }
                            else if (ex.GetType() == typeof(UnauthorizedAccessException))
                            {
                                if (_isVerboseMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine();
                                    Console.WriteLine(ex.Message);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(Resources.x);
                                }
                            }
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine();
                    Console.WriteLine(Resources.Swept_Windows_Update_Logs);
                    Console.WriteLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine();
                    Console.Write(Resources.No_Windows_Update_Logs_directory_found__Skipping);
                    Console.WriteLine();
                }

                _windowsUpdateLogsCleared = true;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(IOException))
                {
                    if (_isVerboseMode)
                    {
                        Console.WriteLine();
                        Console.WriteLine(ex.Message, ConsoleColor.Red);
                    }
                }
                else if (ex.GetType() == typeof(UnauthorizedAccessException))
                {
                    if (_isVerboseMode)
                    {
                        Console.WriteLine();
                        Console.WriteLine(ex.Message, ConsoleColor.Red);
                    }
                }
            }
        }
        #endregion

        #region Calculate Space Saved
        private void CalculateSpaceSaved()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(Resources.Recovery_results);
            Console.WriteLine(Resources.LineSeparator);

            if (_tempFilesWereRemoved)
            {
                _tempFilesWereRemoved = false;
                // Get new Temporary Files size and output what was saved.
                _tempDirSize = Directory.GetFiles(_tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                _tempDirSizeInMegaBytes = _tempDirSize / 1024 / 1024;
                _newTempDirSize = _tempSizeBeforeDelete - _tempDirSizeInMegaBytes;
                _totalSpaceSaved += _newTempDirSize;
                if (_newTempDirSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources.Recovered + _newTempDirSize + Resources.MB_from_removing_Temporary_Files_);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources._1MB_recovered_from_Temporary_Files);
                }
            }

            if (_tempSetupFilesWereRemoved)
            {
                _tempSetupFilesWereRemoved = false;
                // Get new Temporary Setup Files size and output what was saved.
                _tempSetupDirSize = Directory.GetFiles(_tempSetupDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                _tempSetupSizeInMegabytes = _tempSetupDirSize / 1024 / 1024;
                _newTempSetupDirSize = _tempSetupFilesSizeBeforeDelete - _tempSetupSizeInMegabytes;
                _totalSpaceSaved += _newTempSetupDirSize;
                if (_newTempSetupDirSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources.Recovered + _newTempSetupDirSize + Resources.MB_from_removing_Temporary_Setup_Files);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources._1MB_recovered_from_Temporary_Setup_Files);
                }
            }

            if (_tempInternetFilesWereRemoved)
            {
                _tempInternetFilesWereRemoved = false;
                // Get new Temporary Internet Files size and output what was saved.
                try
                {
                    _tempInternetFilesDirSize = Directory.GetFiles(_tempInternetFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                }
                catch (Exception ex)
                {
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                    }
                }
                _tempInternetSizeInMegabytes = _tempInternetFilesDirSize / 1024 / 1024;
                _newTempInternetFilesDirSize = _tempInternetFilesBeforeDelete - _tempInternetSizeInMegabytes;
                _totalSpaceSaved += _newTempInternetFilesDirSize;
                if (_newTempInternetFilesDirSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Resources.Recovered + _newTempInternetFilesDirSize + Resources.MB_from_removing_Temporary_Internet_Files_);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources._1MB_recovered_from_Temporary_Internet_Files);
                }
            }

            if (_eventLogsCleared)
            {
                // Mark the sweeping of Event Logs as completed to allow re-sweeping.
                _eventLogsCleared = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Event_Logs_were_cleared);
            }

            if (_isRecycleBinEmpty)
            {
                _isRecycleBinEmpty = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Recycle_Bin_was_emptied);
            }

            if (_windowsErrorReportsCleared)
            {
                _windowsErrorReportsCleared = false;
                // Get new Windows Error Reports Directory Size and output what was saved.
                _windowsErrorReportsDirSize = Directory.GetFiles(_windowsErrorReportsDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                _windowsErrorReportsDirSizeInMegabytes = _windowsErrorReportsDirSize / 1024 / 1024;
                _newWindowsErrorReportsDirSize = _windowsErrorReportsDirSizeBeforeDelete - _windowsErrorReportsDirSizeInMegabytes;
                _totalSpaceSaved += _newWindowsErrorReportsDirSize;
                if (_newWindowsErrorReportsDirSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources.Recovered + _newWindowsErrorReportsDirSize + Resources.MB_from_removing_Windows_Error_Reports);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources._1MB_recovered_from_removing_Windows_Error_Reports);
                }
            }

            if (_wereDeliveryOptimizationFilesRemoved)
            {
                _wereDeliveryOptimizationFilesRemoved = false;
                // Get new Windows Delivery Optimization directory size and output what was saved.
                _deliveryOptimizationFilesDirSize = Directory.GetFiles(_deliveryOptimizationFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                _deliveryOptimizationFilesDirSizeInMegabytes = _deliveryOptimizationFilesDirSize / 1024 / 1024;
                _newDeliveryOptimizationSize = _deliveryOptimizationFilesDirSizeBeforeDelete - _deliveryOptimizationFilesDirSizeInMegabytes;
                _totalSpaceSaved += _newDeliveryOptimizationSize;
                if (_newDeliveryOptimizationSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources.Recovered + _newDeliveryOptimizationSize + Resources.MB_from_removing_Windows_Delivery_Optimization_Files_);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources._1MB_recovered_from_Windows_Delivery_Optimization_Files);
                }
            }

            if (_thumbnailCacheCleared)
            {
                _thumbnailCacheCleared = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Thumbnail_cache_was_cleared);
            }

            if (_deletedFileHistory)
            {
                _deletedFileHistory = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Removed_file_history_older_than_latest_snapshot);
            }

            if (_windowsOldCleaned)
            {
                _windowsOldCleaned = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Old_versions_of_Windows_were_removed);
            }

            if (_windowsDefenderLogsCleared)
            {
                _windowsDefenderLogsCleared = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Windows_Defender_Logs_were_removed);
            }
            if (_sweptChromeCache)
            {
                _sweptChromeCache = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Chrome_cache_was_cleared);
            }

            if (_sweptEdgeCache)
            {
                _sweptEdgeCache = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Edge_cache_was_cleared);
            }

            if (_msoCacheCleared)
            {
                _msoCacheCleared = false;
                // Get size of MSOCache.
                try
                {
                    if (!Directory.Exists(_msoCacheDir))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(Resources.Recovered + _msoCacheDirSize + Resources.MB_from_removing_MSO_Cache);
                        _totalSpaceSaved += _msoCacheDirSize;
                    }
                    else if (Directory.Exists(_msoCacheDir))
                    {
                        long oldMsoCacheSize = _msoCacheDirSize;
                        _msoCacheDirSize = 0;
                        try
                        {
                            _msoCacheDirSize = Directory.GetFiles(_msoCacheDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                            _newMsoCacheDirSize = oldMsoCacheSize - _msoCacheDirSize;
                            _totalSpaceSaved += _newMsoCacheDirSize;
                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() == typeof(IOException))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(Resources.Recovered + _newMsoCacheDirSize + Resources.MB_from_removing_MSO_Cache);
                            }
                            else if (ex.GetType() == typeof(UnauthorizedAccessException))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(Resources.Recovered + _newMsoCacheDirSize + Resources.MB_from_removing_MSO_Cache);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (_isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            if (_windowsInstallerCacheCleared)
            {
                _windowsInstallerCacheCleared = false;
                // Get size of Windows Installer Cache.
                if (Directory.Exists(_windowsDirectory + "\\Installer\\$PatchCache$"))
                {
                    long oldWindowsInstallerCacheDirSize = _windowsInstallerCacheDirSize;
                    if (Directory.Exists(_windowsInstallerCacheDir))
                    {
                        _windowsInstallerCacheDirSize = Directory.GetFiles(_windowsInstallerCacheDir + "\\Managed", "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                        _newWindowsInstallerCacheDirSize = oldWindowsInstallerCacheDirSize - _windowsInstallerCacheDirSize;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(Resources.Recovered + oldWindowsInstallerCacheDirSize + Resources.MB_from_removing_Windows_Installer_Cache);
                    }
                    _totalSpaceSaved += _newWindowsInstallerCacheDirSize;
                }
            }

            if (_windowsUpdateLogsCleared)
            {
                _windowsUpdateLogsCleared = false;
                // Get size of Windows Update Logs.
                if (!Directory.Exists(_windowsUpdateLogDir))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources.Recovered + _windowsUpdateLogDirSize + Resources.MB_from_removing_Windows_Update_Logs);
                    _totalSpaceSaved += _windowsUpdateLogDirSize;
                }
                else
                {
                    long oldWindowsUpdateLogDirSize = _windowsUpdateLogDirSize;
                    _windowsUpdateLogDirSize = 0;
                    _windowsUpdateLogDirSize = Directory.GetFiles(_windowsUpdateLogDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                    _newWindowsUpdateLogDirSize = oldWindowsUpdateLogDirSize - _windowsUpdateLogDirSize;
                    _totalSpaceSaved += _newWindowsUpdateLogDirSize;
                    if (_newWindowsUpdateLogDirSize > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(Resources.Recovered + _newWindowsUpdateLogDirSize + Resources.MB_from_removing_Windows_Update_Logs);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(Resources._1MB_recovered_from_Windows_Update_Logs);
                        Console.WriteLine(Resources.LineSeparator);
                    }
                }
            }

            // Output the total space saved from the entire operation and other important completed actions.
            if (_totalSpaceSaved > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(Resources.Total_space_recovered + _totalSpaceSaved + Resources.MB);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(Resources.Total_space_recovered___1MB);
            }
            _totalSpaceSaved = 0;
        }
        #endregion

        #region Add Wait Text
        private static void AddWaitText()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Resources.Period);
        }
        #endregion

        #region Show Application Info
        private static void ShowApplicationInfo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(Resources.CleanSweep);
            Console.WriteLine(Resources.Created_by_Thomas_Loupe);
            Console.WriteLine(Resources.Github__https___github_com_thomasloupe);
            Console.WriteLine(Resources.Twitter__https___twitter_com_acid_rain);
            Console.WriteLine(Resources.Website__https___thomasloupe_com);
            Console.WriteLine(Resources.CleanSweep_is__and_will_always_be_free__but_beer_is_not);
            Console.WriteLine(Resources.If_you_d_like_to_buy_me_a_beer_anyway__I_won_t_tell_you_not_to);
            Console.WriteLine(Resources.Visit_https___paypal_me_thomasloupe_if_you_d_like_to_donate);
            Console.WriteLine();
        }
        #endregion

        #region Check For Updates
        private static async void CheckForUpdates()
        {
            var client = new GitHubClient(new ProductHeaderValue("CleanSweep2_CLI"));
            var releases = await client.Repository.Release.GetAll("thomasloupe", "CleanSweep2");
            var latest = releases[0];
            if (CurrentVersion == latest.TagName)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.You_have_the_latest_version___0__, CurrentVersion);
                Console.WriteLine();
                Console.WriteLine();
            }
            else if (CurrentVersion != latest.TagName)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine(Resources.A_new_version_of_CleanSweep2_CLI_is_available);
                Console.WriteLine(Resources.Current_version);
                Console.WriteLine(Resources.Latest_version + latest.TagName);
                Console.WriteLine(Resources.Visit_https___github_com_thomasloupe_CleanSweep2_to_download_the_latest_version);
                Console.WriteLine();
            }
        }
        #endregion
    }
}
