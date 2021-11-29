using Octokit;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace CleanSweep2_CLI
{
    internal class CleanSweep2_CLI
    {
        #region Declarations
        private const string CurrentVersion = "v2.2.0";
        readonly string userName = Environment.UserName;
        readonly string systemDrive = Path.GetPathRoot(Environment.SystemDirectory);
        readonly string windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        readonly string programDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        readonly string localAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string logPath;

        bool isVerboseMode;
        bool showOperationWindows;
        bool tempFilesWereRemoved = false;
        bool tempSetupFilesWereRemoved = false;
        bool tempInternetFilesWereRemoved = false;
        bool eventLogsCleared = false;
        bool isRecycleBinEmpty = false;
        bool windowsErrorReportsCleared = false;
        bool wereDeliveryOptimizationFilesRemoved = false;
        bool thumbnailCacheCleared = false;
        bool deletedFileHistory = false;
        bool windowsOldCleaned = false;
        bool windowsDefenderLogsCleared = false;
        bool sweptChromeCache = false;
        bool sweptEdgeCache = false;
        bool msoCacheCleared = false;
        bool windowsInstallerCacheCleared = false;
        bool windowsUpdateLogsCleared = false;
        long totalSpaceSaved;

        // Temporary Files
        string tempDirectory;
        long tempDirSize;
        long tempDirSizeInMegaBytes;
        long tempSizeBeforeDelete;
        long newTempDirSize;

        // Temporary Setup Files
        string tempSetupDirectory;
        long tempSetupDirSize;
        long tempSetupSizeInMegabytes;
        long tempSetupFilesSizeBeforeDelete;
        long newTempSetupDirSize;

        // Temporary Internet Files
        string tempInternetFilesDirectory;
        long tempInternetFilesDirSize;
        long tempInternetSizeInMegabytes;
        long tempInternetFilesBeforeDelete;
        long newTempInternetFilesDirSize;

        // Windows Error Reports
        string windowsErrorReportsDirectory;
        long windowsErrorReportsDirSize;
        long windowsErrorReportsDirSizeInMegabytes;
        long windowsErrorReportsDirSizeBeforeDelete;
        long newWindowsErrorReportsDirSize;

        // Delivery Optimization Files
        string deliveryOptimizationFilesDirectory;
        long deliveryOptimizationFilesDirSize;
        long deliveryOptimizationFilesDirSizeInMegabytes;
        long deliveryOptimizationFilesDirSizeBeforeDelete;
        long newDeliveryOptimizationSize;

        // Chrome Directories
        readonly string[] chromeCacheDirectories = new string[6];
        long totalChromeDirSize = 0;

        // Edge Directories
        readonly string[] edgeCacheDirectories = new string[12];
        long totalEdgeDirSize = 0;

        // MSO Cache Directories
        string msoCacheDir;
        long msoCacheDirSize = 0;
        long newMsoCacheDirSize;

        // Windows Installer Cache Directories
        string windowsInstallerCacheDir;
        long windowsInstallerCacheDirSize;
        long newWindowsInstallerCacheDirSize;

        // Windows Update Log Directories.
        string windowsUpdateLogDir;
        long windowsUpdateLogDirSize;
        long newWindowsUpdateLogDirSize;
        #endregion
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            var cs2cli = new CleanSweep2_CLI();
            ShowApplicationInfo();
            CheckForUpdates();
            cs2cli.GetSizeInformation();
            
            // TODO: Disable verbosity level 1/2 dependent on which was already selected in args.

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-1":
                        {
                            cs2cli.Option1();
                            break;
                        }
                    case "-2":
                        {
                            cs2cli.Option2();
                            break;
                        }
                    case "-3":
                        {
                            cs2cli.Option3();
                            break;
                        }
                    case "-4":
                        {
                            cs2cli.Option4();
                            break;
                        }
                    case "-5":
                        {
                            cs2cli.Option5();
                            break;
                        }
                    case "-6":
                        {
                            cs2cli.Option6();
                            break;
                        }
                    case "-7":
                        {
                            cs2cli.Option7();
                            break;
                        }
                    case "-8":
                        {
                            cs2cli.Option8();
                            break;
                        }
                    case "-9":
                        {
                            cs2cli.Option9();
                            break;
                        }
                    case "-10":
                        {
                            cs2cli.Option10();
                            break;
                        }
                    case "-11":
                        {
                            cs2cli.Option11();
                            break;
                        }
                    case "-12":
                        {
                            cs2cli.Option12();
                            break;
                        }
                    case "-13":
                        {
                            cs2cli.Option13();
                            break;
                        }
                    case "-14":
                        {
                            cs2cli.Option14();
                            break;
                        }
                    case "-15":
                        {
                            cs2cli.Option15();
                            break;
                        }
                    case "-16":
                        {
                            cs2cli.Option16();
                            break;
                        }
                    case "-log":
                        {
                            // Set the path for logging.
                            foreach (Match match in Regex.Matches(cs2cli.logPath, "\"([^\"]*)\""))
                            {
                                string tempLogpath = cs2cli.logPath;
                                tempLogpath.Replace(@"""", "");
                                cs2cli.logPath = tempLogpath;
                            }
                            break;
                        }
                    case "-v1":
                        {
                            // Set verbosity to low.
                            cs2cli.isVerboseMode = false;
                            break;
                        }
                    case "-v2":
                        {
                            // Set verbosity to high.
                            cs2cli.isVerboseMode = true;
                            break;
                        }
                    case "-visible":
                        {
                            // Show CMD window via method.
                            cs2cli.showOperationWindows = false;
                            break;
                        }
                    default:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("No arguments given. Please re-run CleanSweep2_CLI with arguments.");
                            break;
                        }
                }
            }
            cs2cli.CalculateSpaceSaved();
        }
        #region Get Size Information
        private void GetSizeInformation()
        {
            // Set Chrome cache directories.
            chromeCacheDirectories[0] = localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\Cache";
            chromeCacheDirectories[1] = localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\Media Cache";
            chromeCacheDirectories[2] = localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\GPUCache";
            chromeCacheDirectories[3] = localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\Storage\\ext";
            chromeCacheDirectories[4] = localAppDataDirectory + "\\Google\\Chrome\\User Data\\Default\\Service Worker";
            chromeCacheDirectories[5] = localAppDataDirectory + "\\Google\\Chrome\\User Data\\ShaderCache";

            // Set Edge cache directories.
            edgeCacheDirectories[0] = localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\Cache";
            edgeCacheDirectories[1] = localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\Media Cache";
            edgeCacheDirectories[2] = localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\GPUCache";
            edgeCacheDirectories[3] = localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\Storage\\ext";
            edgeCacheDirectories[4] = localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\Default\\Service Worker";
            edgeCacheDirectories[5] = localAppDataDirectory + "\\Microsoft\\Edge\\User Data\\ShaderCache";
            edgeCacheDirectories[6] = localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\Cache";
            edgeCacheDirectories[7] = localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\Media Cache";
            edgeCacheDirectories[8] = localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\GPUCache";
            edgeCacheDirectories[9] = localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\Storage\\ext";
            edgeCacheDirectories[10] = localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\Default\\Service Worker";
            edgeCacheDirectories[11] = localAppDataDirectory + "\\Microsoft\\Edge SxS\\User Data\\ShaderCache";

            // Set MSOCache Directory.
            msoCacheDir = systemDrive + "MSOCache";

            // Set Windows Installer Cache Directory.
            windowsInstallerCacheDir = windowsDirectory + "\\Installer\\$PatchCache$\\Managed";

            // Set Windows Log Directory.
            windowsUpdateLogDir = windowsDirectory + "\\Logs\\";

            // Get size of Temporary Files.
            tempDirectory = "C:\\Users\\" + userName + "\\AppData\\Local\\Temp\\";
            try
            {
                tempDirSize = Directory.GetFiles(tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
            tempDirSizeInMegaBytes = tempDirSize / 1024 / 1024;

            // Get size of Temporary Setup Files.
            tempSetupDirectory = windowsDirectory + "\\temp";
            tempSetupDirSize = Directory.GetFiles(tempSetupDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            tempSetupSizeInMegabytes = tempSetupDirSize / 1024 / 1024;

            // Get size of Temporary Internet Files.
            tempInternetFilesDirectory = "C:\\Users\\" + userName + "\\AppData\\Local\\Microsoft\\Windows\\INetCache\\";
            try
            {
                tempInternetFilesDirSize = Directory.GetFiles(tempInternetFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            }
            catch (Exception)
            {

            }
            tempInternetSizeInMegabytes = tempInternetFilesDirSize / 1024 / 1024;

            // Get size of Windows Error Reports
            windowsErrorReportsDirectory = programDataDirectory + "\\Microsoft\\Windows\\WER\\ReportArchive";
            if (Directory.Exists(windowsErrorReportsDirectory))
            {
                windowsErrorReportsDirSize = Directory.GetFiles(windowsErrorReportsDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                windowsErrorReportsDirSizeInMegabytes = windowsErrorReportsDirSize / 1024 / 1024;
            }
            else
            {
                windowsErrorReportsDirSizeInMegabytes = 0;
            }

            // Get size of Windows Delivery Optimization Files
            deliveryOptimizationFilesDirectory = windowsDirectory + "\\SoftwareDistribution\\";
            deliveryOptimizationFilesDirSize = Directory.GetFiles(deliveryOptimizationFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            deliveryOptimizationFilesDirSizeInMegabytes = deliveryOptimizationFilesDirSize / 1024 / 1024;

            // Get size of Chrome cache directories.
            foreach (string chromeDirectory in chromeCacheDirectories)
            {
                if (Directory.Exists(chromeDirectory))
                {
                    long thisChromeDirSize = 0;
                    thisChromeDirSize = Directory.GetFiles(chromeDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                    totalChromeDirSize += thisChromeDirSize;
                }
            }
            // Get size of Edge cache directories.
            foreach (string edgeDirectory in edgeCacheDirectories)
            {
                if (Directory.Exists(edgeDirectory))
                {
                    long thisEdgeDirSize = 0;
                    thisEdgeDirSize = Directory.GetFiles(edgeDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                    totalEdgeDirSize += thisEdgeDirSize;
                }
            }

            // Get size of MSOCache.
            if (Directory.Exists(msoCacheDir))
            {
                try
                {
                    msoCacheDirSize = Directory.GetFiles(msoCacheDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IOException))
                    {
                        if (isVerboseMode)
                        {
                            Console.Write(ex.Message + "\n", ConsoleColor.Red);
                        }
                    }
                    else if (ex.GetType() == typeof(UnauthorizedAccessException))
                    {
                        if (isVerboseMode)
                        {
                            Console.Write(ex.Message + "\n", ConsoleColor.Red);
                        }
                    }
                }
            }

            // Get size of Windows Installer Cache.
            if (Directory.Exists(windowsInstallerCacheDir))
            {
                windowsInstallerCacheDirSize = Directory.GetFiles(windowsInstallerCacheDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
            }

            // Get size of Windows Update Logs directory.
            if (Directory.Exists(windowsUpdateLogDir))
            {
                windowsUpdateLogDirSize = Directory.GetFiles(windowsUpdateLogDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
            }

            // Show potential reclamation, then each individual category.
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Potential space to reclaim: " +
                (tempDirSizeInMegaBytes + tempSetupSizeInMegabytes + tempInternetSizeInMegabytes + windowsErrorReportsDirSizeInMegabytes +
                deliveryOptimizationFilesDirSizeInMegabytes + totalChromeDirSize + totalEdgeDirSize + msoCacheDirSize + windowsInstallerCacheDirSize +
                windowsUpdateLogDirSize) + "MB" + "\n" + "\n" + "Categorical breakdown:" + "\n" +
                "--------------------------------------------------" + "\n");

            // List out total space reclamation per category.
            Console.Write("Temp Directory size: " + tempDirSizeInMegaBytes + "MB" + "\n");
            Console.Write("Temporary Setup Files directory size: " + tempSetupSizeInMegabytes + "MB" + "\n");
            Console.Write("Temporary Internet Files directory size: " + tempInternetSizeInMegabytes + "MB" + "\n");
            Console.Write("Windows Error Reports size: " + windowsErrorReportsDirSizeInMegabytes + "MB" + "\n");
            Console.Write("Windows Delivery Optimization File size: " + deliveryOptimizationFilesDirSizeInMegabytes + "MB" + "\n");
            Console.Write("Chrome Data Size: " + totalChromeDirSize + "MB" + "\n");
            Console.Write("Edge Data Size: " + totalEdgeDirSize + "MB" + "\n");
            Console.Write("MSO Cache size: " + msoCacheDirSize + "MB" + "\n");
            Console.Write("Windows Installer Cache size: " + windowsInstallerCacheDirSize + "MB" + "\n");
            Console.Write("Windows Update Logs size: " + windowsUpdateLogDirSize + "MB" + "\n" + "\n");
        }
        #endregion

        #region Temporary Files Removal (1)
        private void Option1()
        {
            // Temporary Files removal.
            tempSizeBeforeDelete = tempDirSizeInMegaBytes;
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping Temporary Files..." + "\n");
            var di = new DirectoryInfo(tempDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    // If file no longer exists, append success to rich text box.
                    if (!File.Exists(file.Name))
                    {
                        if (isVerboseMode)
                        {
                            Console.Write("Deleted: " + file.Name + "\n", ConsoleColor.Green);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("o");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed files.
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Couldn't delete " + file.Name + ": " + ex.Message + " Skipping..." + "\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("x");
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
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Deleted: " + dir.Name + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("o");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed directories.
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Couldn't delete " + dir.Name + ": " + ex.Message + " Skipping..." + "\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("x");
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + "Swept Temporary Files!" + "\n" + "\n");
            tempFilesWereRemoved = true;
        }
        #endregion

        #region Temporary Setup Files Removal (2)
        private void Option2()
        {
            // Temporary Setup Files removal
            tempSetupFilesSizeBeforeDelete = tempSetupSizeInMegabytes;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping Temporary Setup Files..." + "\n");
            var di = new DirectoryInfo(tempSetupDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    if (!File.Exists(file.Name))
                    {
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Deleted: " + file.Name + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("o");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed files.
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Couldn't delete " + file.Name + ": " + ex.Message + " Skipping..." + "\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("x");
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
                        if (isVerboseMode)
                        {
                            Console.Write("Deleted: " + dir.Name + "\n", ConsoleColor.Green);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("o");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed directories.
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Couldn't delete " + dir.Name + ": " + ex.Message + " Skipping..." + "\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("x");
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + "Swept Temporary Setup Files!" + "\n" + "\n");
            tempSetupFilesWereRemoved = true;
        }
        #endregion

        #region Temporary Internet Files Removal (3)
        private void Option3()
        {
            // Temporary Setup Files removal
            tempInternetFilesBeforeDelete = tempInternetSizeInMegabytes;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping Temporary Internet Files..." + "\n");
            var di = new DirectoryInfo(tempInternetFilesDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    if (!File.Exists(file.Name))
                    {
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Deleted: " + file.Name + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("o");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed files.
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Couldn't delete " + file.Name + ": " + ex.Message + "\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("x");
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
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Deleted: " + dir.Name + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("o");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Skip all failed directories.
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Couldn't delete " + dir.Name + ": " + ex.Message + " Skipping..." + "\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("x");

                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "Swept Temporary Internet Files!" + "\n" + "\n");
                    tempInternetFilesWereRemoved = true;
                }
            }
        }
        #endregion

        #region Event Viewer Logs Removal (4)
        private void Option4()
        {
            // Event Viewer Logs Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping Event Viewer Logs");
            AddWaitText();
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (showOperationWindows)
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
            Console.Write("\n" + "Swept Event Viewer Logs!" + "\n" + "\n");
            eventLogsCleared = true;
        }
        #endregion

        #region Empty Recycle Bin (5)
        private void Option5()
        {
            // Empty Recycle Bin.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Emptying Recycle Bin");
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (showOperationWindows)
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
            Console.Write("\n" + "Emptied Recycle Bin!" + "\n" + "\n");
            isRecycleBinEmpty = true;
        }
        #endregion

        #region Windows Error Reports Removal (6)
        private void Option6()
        {
            // Temporary Setup Files removal
            windowsErrorReportsDirSizeBeforeDelete = windowsErrorReportsDirSizeInMegabytes;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping Windows Error Reports" + "\n");
            if (Directory.Exists(windowsErrorReportsDirectory))
            {
                var di = new DirectoryInfo(windowsErrorReportsDirectory);
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        if (!File.Exists(file.Name))
                        {
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("Deleted: " + file.Name + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("o");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Skip all failed files.
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(file.Name + " appears to be in use or locked. Skipping..." + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("x");
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
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("Deleted: " + dir.Name + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("o");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("Couldn't remove " + dir.Name + ": " + ex.Message + ". Skipping..." + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("x");
                        }
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Swept Windows Error Reports!" + "\n" + "\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Windows Error Report directory already removed. Skipping" + "\n" + "\n");
            }
            windowsErrorReportsCleared = true;
        }
        #endregion

        #region Delivery Optimization Files (7)
        private void Option7()
        {
            // Delivery Optimization Files Removal
            deliveryOptimizationFilesDirSizeBeforeDelete = deliveryOptimizationFilesDirSizeInMegabytes;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping Delivery Optimization Files..." + "\n");
            var di = new DirectoryInfo(deliveryOptimizationFilesDirectory);

            if (di.GetFiles().Length != 0)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        if (!File.Exists(file.Name))
                        {
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("Deleted: " + file.Name + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("o");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed files.
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("Couldn't delete " + file.Name + ": " + ex.Message + ". Skipping..." + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("x");
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
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("Deleted: " + dir.Name + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("o");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (isVerboseMode)
                        {
                            Console.Write("Couldn't delete " + dir.Name + ": " + ex.Message + ". Skipping..." + "\n", ConsoleColor.Red);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("x");
                        }
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Removed Delivery Optimization Files!" + "\n" + "\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("No Delivery Optimization Files needed to be cleaned." + "\n" + "\n");
            }
            wereDeliveryOptimizationFilesRemoved = true;
        }
        #endregion

        #region Thumbnail Cache Removal (8)
        private void Option8()
        {
            // Thumbnail Cache Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Clearing Thumbnail Cache");
            AddWaitText();
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (showOperationWindows)
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            }
            else
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }
            startInfo.FileName = "cmd.exe";
            if (isVerboseMode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Shutting down Explorer.exe process...");
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
            if (isVerboseMode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Restarted Explorer.exe process.");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + "Cleared Thumbnail Cache!" + "\n" + "\n");
            thumbnailCacheCleared = true;
        }
        #endregion

        #region User File History Removal (9)
        private void Option9()
        {
            // User File History Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Attempting to remove all file history snapshots except latest");
            AddWaitText();
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (showOperationWindows)
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
            Console.Write("\n" + "If file history was enabled, all versions except latest were removed." + "\n" + "\n");
            deletedFileHistory = true;
        }
        #endregion

        #region Windows.old Directory Removal (10)
        private void Option10()
        {
            // Windows.old Directory Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Removing old versions of Windows");
            if (Directory.Exists("C:\\windows.old"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Found Windows.old directory. Cleaning");
                AddWaitText();
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                if (showOperationWindows)
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
                Console.Write("\n" + "Cleaned Windows.old directory!" + "\n" + "\n");
                windowsOldCleaned = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "No Windows.old directory found. Skipping..." + "\n" + "\n");
                windowsOldCleaned = false;
            }
        }
        #endregion

        #region Windows Defender Log Files Removal (11)
        private void Option11()
        {
            // Windows Defender Log Files Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Deleting Windows Defender Log Files..." + "\n");

            // Create an array that contain paths to search for log files.
            string[] directoryPath = new string[2]
            {
                    programDataDirectory + "\\Microsoft\\Windows Defender\\Network Inspection System\\Support\\",
                    programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Service\\"
            };

            // Create an array that contains the directory paths for Defender Logs.
            string[] defenderLogDirectoryPaths = new string[6]
            {
                    programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\ReportLatency\\Latency",
                    programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Results\\Resource",
                    programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\Results\\Quick",
                    programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\History\\CacheManager",
                    programDataDirectory + "\\Microsoft\\Windows Defender\\Scans\\MetaStore",
                    programDataDirectory + "\\Microsoft\\Windows Defender\\Support"
            };
            // Remove the logfiles.
            foreach (string directory in directoryPath)
            {
                var dir = new DirectoryInfo(directory);
                foreach (FileInfo f in dir.GetFiles())
                {
                    if (f.Name.Contains(".log"))
                    {
                        try
                        {
                            File.Delete(f.FullName);
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("Removed " + f.Name + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("o");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(ex.Message + " Skipping..." + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("x" + f.Name + "\n");
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
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Removed " + logFileDirectory + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("o");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ex.Message + "Skipping..." + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("x");
                        }
                    }
                }
                else
                {
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Directory already removed: " + logFileDirectory + "\n");
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + "Removed Windows Defender Logs!" + "\n" + "\n");
            windowsDefenderLogsCleared = true;
        }
        #endregion

        #region Microsoft Office Cache Removal (12)
        private void Option12()
        {
            // Microsoft Office Cache Removal.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping MSO cache");
            if (Directory.Exists(msoCacheDir))
            {
                Directory.Delete(msoCacheDir, true);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Swept MSO Cache!" + "\n" + "\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "No MSOCache directory found. Skipping..." + "\n" + "\n");
            }

            msoCacheCleared = true;
        }
        #endregion

        #region Microsoft Edge Cache Removal (13)
        private void Option13()
        {
            // Edge Cache Removal
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping Edge cache..." + "\n");
            if (isVerboseMode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Ending any Edge processes");
            }
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (showOperationWindows)
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
                if (isVerboseMode)
                {
                    AddWaitText();
                }
                if (process.HasExited)
                {
                    break;
                }
                if (isVerboseMode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "Stopped Edge processes." + "\n");
                }
                foreach (string edgeDirectory in edgeCacheDirectories)
                {
                    if (Directory.Exists(edgeDirectory))
                    {
                        try
                        {
                            Directory.Delete(edgeDirectory, true);
                            if (!Directory.Exists(edgeDirectory))
                            {
                                if (isVerboseMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write("Removed: " + edgeDirectory + "." + "\n");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write("x");
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Skipping: " + edgeDirectory + ". " + ex.Message + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("x");
                            }
                        }
                    }
                    else
                    {
                        if (isVerboseMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Directory already removed: " + edgeDirectory + "." + "\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("x");
                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + "Swept Edge cache!" + "\n" + "\n");
            sweptEdgeCache = true;
        }
        #endregion

        #region Chrome Cache Removal (14)
        private void Option14()
        {
            // Chrome Cache Removal
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping Chrome cache..." + "\n");
            if (showOperationWindows)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Ending any Chrome processes");
            }
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            if (isVerboseMode)
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
                if (isVerboseMode)
                {
                    AddWaitText();
                }
                if (process.HasExited)
                {
                    break;
                }
            }
            if (isVerboseMode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Stopped Chrome processes." + "\n");
            }
            foreach (string chromeDirectory in chromeCacheDirectories)
            {
                if (Directory.Exists(chromeDirectory))
                {
                    try
                    {
                        Directory.Delete(chromeDirectory, true);
                        if (!Directory.Exists(chromeDirectory))
                        {
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("Removed: " + chromeDirectory + "." + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("x");
                            }
                        }
                    }
                    catch (SystemException ex)
                    {
                        if (ex.GetType() == typeof(IOException))
                        {
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Skipping: " + chromeDirectory + ". " + ex.Message + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("x");
                            }
                        }
                        else if (ex.GetType() == typeof(UnauthorizedAccessException))
                        {
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Skipping: " + chromeDirectory + ". " + ex.Message + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("x");
                            }
                        }
                        else
                        {
                            if (isVerboseMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Skipping: " + chromeDirectory + ". " + ex.Message + "\n");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("x");
                            }
                        }
                    }
                }
                else
                {
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("Directory already removed: " + chromeDirectory + "." + "\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("x");
                    }
                }
            }
            sweptChromeCache = true;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + "Swept Chrome cache!" + "\n" + "\n");
        }
        #endregion

        #region Windows Installer Cache Removal (15)
        private void Option15()
        {
            // Windows Installer Cache Removal
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Removing Windows Installer Cache");
            if (Directory.Exists(windowsDirectory + "\\Installer\\$PatchCache$\\Managed"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Found Windows Installer Cache. Cleaning");
                AddWaitText();
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                if (showOperationWindows)
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Cleaned Windows Installer Cache!" + "\n" + "\n");
            }
            windowsInstallerCacheCleared = true;
        }
        #endregion

        #region Windows Update Logs Removal (16)
        private void Option16()
        {
            // Windows Update Logs Removal
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Sweeping Windows Update Logs" + "\n");
            try
            {
                if (Directory.Exists(windowsUpdateLogDir))
                {
                    var di = new DirectoryInfo(tempDirectory);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                            // If file no longer exists, append success to rich text box.
                            if (!File.Exists(file.Name))
                            {
                                if (isVerboseMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write("Deleted: " + file.Name + "\n");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write("o");
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("x");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() == typeof(IOException))
                            {
                                if (isVerboseMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("\n" + ex.Message);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("x");
                                }
                            }
                            else if (ex.GetType() == typeof(UnauthorizedAccessException))
                            {
                                if (isVerboseMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("\n" + ex.Message);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("x");
                                }
                            }
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "Swept Windows Update Logs!" + "\n" + "\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "No Windows Update Logs directory found. Skipping..." + "\n" + "\n");
                }

                windowsUpdateLogsCleared = true;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(IOException))
                {
                    if (isVerboseMode)
                    {
                        Console.Write("\n" + ex.Message, ConsoleColor.Red);
                    }
                }
                else if (ex.GetType() == typeof(UnauthorizedAccessException))
                {
                    if (isVerboseMode)
                    {
                        Console.Write("\n" + ex.Message, ConsoleColor.Red);
                    }
                }
            }
        }
        #endregion

        #region Calculate Space Saved
        private void CalculateSpaceSaved()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + "\n" + "Recovery results:");
            Console.Write("\n" + "--------------------------------------------------");

            if (tempFilesWereRemoved)
            {
                tempFilesWereRemoved = false;
                // Get new Temporary Files size and output what was saved.
                tempDirSize = Directory.GetFiles(tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                tempDirSizeInMegaBytes = tempDirSize / 1024 / 1024;
                newTempDirSize = tempSizeBeforeDelete - tempDirSizeInMegaBytes;
                totalSpaceSaved += newTempDirSize;
                if (newTempDirSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "Recovered " + newTempDirSize + "MB from removing Temporary Files.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "<1MB recovered from Temporary Files...");
                }
            }

            if (tempSetupFilesWereRemoved)
            {
                tempSetupFilesWereRemoved = false;
                // Get new Temporary Setup Files size and output what was saved.
                tempSetupDirSize = Directory.GetFiles(tempSetupDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                tempSetupSizeInMegabytes = tempSetupDirSize / 1024 / 1024;
                newTempSetupDirSize = tempSetupFilesSizeBeforeDelete - tempSetupSizeInMegabytes;
                totalSpaceSaved += newTempSetupDirSize;
                if (newTempSetupDirSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "Recovered " + newTempSetupDirSize + "MB from removing Temporary Setup Files.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "<1MB recovered from Temporary Setup Files...");
                }
            }

            if (tempInternetFilesWereRemoved)
            {
                tempInternetFilesWereRemoved = false;
                // Get new Temporary Internet Files size and output what was saved.
                try
                {
                    tempInternetFilesDirSize = Directory.GetFiles(tempInternetFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                }
                catch (Exception ex)
                {
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("\n" + ex.Message);
                    }
                }
                tempInternetSizeInMegabytes = tempInternetFilesDirSize / 1024 / 1024;
                newTempInternetFilesDirSize = tempInternetFilesBeforeDelete - tempInternetSizeInMegabytes;
                totalSpaceSaved += newTempInternetFilesDirSize;
                if (newTempInternetFilesDirSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "Recovered " + newTempInternetFilesDirSize + "MB from removing Temporary Internet Files.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "<1MB recovered from Temporary Internet Files...");
                }
            }

            if (eventLogsCleared)
            {
                // Mark the sweeping of Event Logs as completed to allow re-sweeping.
                eventLogsCleared = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Event Logs were cleared.");
            }

            if (isRecycleBinEmpty)
            {
                isRecycleBinEmpty = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Recycle Bin was emptied.");
            }

            if (windowsErrorReportsCleared)
            {
                windowsErrorReportsCleared = false;
                // Get new Windows Error Reports Directory Size and output what was saved.
                windowsErrorReportsDirSize = Directory.GetFiles(windowsErrorReportsDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                windowsErrorReportsDirSizeInMegabytes = windowsErrorReportsDirSize / 1024 / 1024;
                newWindowsErrorReportsDirSize = windowsErrorReportsDirSizeBeforeDelete - windowsErrorReportsDirSizeInMegabytes;
                totalSpaceSaved += newWindowsErrorReportsDirSize;
                if (newWindowsErrorReportsDirSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "Recovered " + newWindowsErrorReportsDirSize + "MB from removing Windows Error Reports.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "<1MB recovered from removing Windows Error Reports...");
                }
            }

            if (wereDeliveryOptimizationFilesRemoved)
            {
                wereDeliveryOptimizationFilesRemoved = false;
                // Get new Windows Delivery Optimization directory size and output what was saved.
                deliveryOptimizationFilesDirSize = Directory.GetFiles(deliveryOptimizationFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                deliveryOptimizationFilesDirSizeInMegabytes = deliveryOptimizationFilesDirSize / 1024 / 1024;
                newDeliveryOptimizationSize = deliveryOptimizationFilesDirSizeBeforeDelete - deliveryOptimizationFilesDirSizeInMegabytes;
                totalSpaceSaved += newDeliveryOptimizationSize;
                if (newDeliveryOptimizationSize > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "Recovered " + newDeliveryOptimizationSize + "MB from removing Windows Delivery Optimization Files.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "<1MB recovered from Windows Delivery Optimization Files...");
                }
            }

            if (thumbnailCacheCleared)
            {
                thumbnailCacheCleared = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Thumbnail cache was cleared.");
            }

            if (deletedFileHistory)
            {
                deletedFileHistory = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Removed file history older than latest snapshot.");
            }

            if (windowsOldCleaned)
            {
                windowsOldCleaned = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Old versions of Windows were removed.");
            }

            if (windowsDefenderLogsCleared)
            {
                windowsDefenderLogsCleared = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Windows Defender Logs were removed.");
            }
            if (sweptChromeCache)
            {
                sweptChromeCache = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Chrome cache was cleared.");
            }

            if (sweptEdgeCache)
            {
                sweptEdgeCache = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "Edge cache was cleared.");
            }

            if (msoCacheCleared)
            {
                msoCacheCleared = false;
                // Get size of MSOCache.
                try
                {
                    if (!Directory.Exists(msoCacheDir))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("\n" + "Recovered " + msoCacheDirSize + "MB from removing MSO Cache.");
                        totalSpaceSaved += msoCacheDirSize;
                    }
                    else if (Directory.Exists(msoCacheDir))
                    {
                        long oldMsoCacheSize = msoCacheDirSize;
                        msoCacheDirSize = 0;
                        try
                        {
                            msoCacheDirSize = Directory.GetFiles(msoCacheDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                            newMsoCacheDirSize = oldMsoCacheSize - msoCacheDirSize;
                            totalSpaceSaved += newMsoCacheDirSize;
                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() == typeof(IOException))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("\n" + "Recovered " + newMsoCacheDirSize + "MB from removing MSO Cache.");
                            }
                            else if (ex.GetType() == typeof(UnauthorizedAccessException))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("\n" + "Recovered " + newMsoCacheDirSize + "MB from removing MSO Cache.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (isVerboseMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("\n" + ex.Message);
                    }
                }
            }

            if (windowsInstallerCacheCleared)
            {
                windowsInstallerCacheCleared = false;
                // Get size of Windows Installer Cache.
                if (Directory.Exists(windowsDirectory + "\\Installer\\$PatchCache$"))
                {
                    long oldWindowsInstallerCacheDirSize = windowsInstallerCacheDirSize;
                    Console.WriteLine(oldWindowsInstallerCacheDirSize);
                    if (Directory.Exists(windowsInstallerCacheDir))
                    {
                        windowsInstallerCacheDirSize = Directory.GetFiles(windowsInstallerCacheDir + "\\Managed", "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                        newWindowsInstallerCacheDirSize = oldWindowsInstallerCacheDirSize - windowsInstallerCacheDirSize;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("\n" + "Recovered " + oldWindowsInstallerCacheDirSize + "MB from removing Windows Installer Cache.");
                    }
                    totalSpaceSaved += newWindowsInstallerCacheDirSize;
                }
            }

            if (windowsUpdateLogsCleared)
            {
                windowsUpdateLogsCleared = false;
                // Get size of Windows Update Logs.
                if (!Directory.Exists(windowsUpdateLogDir))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n" + "Recovered " + windowsUpdateLogDirSize + "MB from removing Windows Update Logs.");
                    totalSpaceSaved += windowsUpdateLogDirSize;
                }
                else
                {
                    long oldWindowsUpdateLogDirSize = windowsUpdateLogDirSize;
                    windowsUpdateLogDirSize = 0;
                    windowsUpdateLogDirSize = Directory.GetFiles(windowsUpdateLogDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                    newWindowsUpdateLogDirSize = oldWindowsUpdateLogDirSize - windowsUpdateLogDirSize;
                    totalSpaceSaved += newWindowsUpdateLogDirSize;
                    if (newWindowsUpdateLogDirSize > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("\n" + "Recovered " + newWindowsUpdateLogDirSize + "MB from removing Windows Update Logs.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("\n" + "<1MB recovered from Windows Update Logs...");
                    }
                }
            }

            // Output the total space saved from the entire operation and other important completed actions.
            if (totalSpaceSaved > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "\n" + "Total space recovered: " + totalSpaceSaved + "MB" + "\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n" + "\n" + "Total space recovered: <1MB" + "\n");
            }
            totalSpaceSaved = 0;
        }
        #endregion

        #region Add Wait Text
        private static void AddWaitText()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(".");
        }
        #endregion

        #region Show Application Info
        private static void ShowApplicationInfo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("CleanSweep " + CurrentVersion + ".\n" +
            "Created by Thomas Loupe." + "\n" +
            "Github: https://github.com/thomasloupe" + "\n" +
            "Twitter: https://twitter.com/acid_rain" + "\n" +
            "Website: https://thomasloupe.com" + "\n" + "\n");
            Console.Write("CleanSweep is, and will always be free, but beer is not!\n"
            + "If you'd like to buy me a beer anyway, I won't tell you not to!\n"
            + "Visit https://paypal.me/thomasloupe if you'd like to donate." + "\n" + "\n");
        }
        #endregion

        #region Check For Updates
        private static void CheckForUpdates()
        {
            var client = new GitHubClient(new ProductHeaderValue("CleanSweep2_CLI"));
            var releases = (client.Repository.Release.GetAll("thomasloupe", "CleanSweep2").Result);
            var latest = releases[0];
            if (CurrentVersion == latest.TagName)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("You have the latest version, {0}!" + "\n" + "\n", CurrentVersion);
            }
            else if (CurrentVersion != latest.TagName)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("A new version of CleanSweep2_CLI is available!" + "\n"
                + "Current version: " + CurrentVersion + "\n"
                + "Latest version: " + latest.TagName + "\n"
                + "Visit https://github.com/thomasloupe/CleanSweep2 to download the latest version." + "\n" + "\n");
            }
        }
        #endregion
    }
}
