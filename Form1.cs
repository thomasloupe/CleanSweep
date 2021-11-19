using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using octo = Octokit;
using System.Threading;
using System.Threading.Tasks;

namespace CleanSweep2
{
    public partial class Form1 : Form
    {
        #region Declarations
        private const string CurrentVersion = "v2.1.9";
        private octo.GitHubClient _octoClient;
        readonly string userName = Environment.UserName;
        readonly string systemDrive = Path.GetPathRoot(Environment.SystemDirectory);
        readonly string windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        readonly string programDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        readonly string localAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

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
        bool[] checkedArrayBool;
        long totalSpaceSaved;
        CheckBox[] checkedArray;

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

        public Form1()
        {
            InitializeComponent();
            SetWindowSizeAndLocation();
            SetVerbosity();
            SetOperationWindows();
            button1.Enabled = false;
        }

        private void SetWindowSizeAndLocation()
        {
            Location = Properties.Settings.Default.FormLocation;
            Height = Properties.Settings.Default.FormHeight;
            Width = Properties.Settings.Default.FormWidth;
            FormClosing += SaveSettingsEventHandler;
            StartPosition = FormStartPosition.Manual;
        }

        private void SetVerbosity()
        {
            // Set value of Verbosity equal to what's stored in the application's setting.
            if (Properties.Settings.Default.Verbosity)
            {
                verboseModeToolStripMenuItem.Checked = true;
                isVerboseMode = true;
            }
            else
            {
                verboseModeToolStripMenuItem.Checked = false;
                isVerboseMode = false;
            }
        }

        private void SetOperationWindows()
        {
            // Set value of Show Operation Windows equal to what's stored in the application's setting.
            if (Properties.Settings.Default.OperationWindows)
            {
                showOperationWindowsToolStripMenuItem.Checked = true;
                showOperationWindows = true;
            }
            else
            {
                showOperationWindowsToolStripMenuItem.Checked = false;
                showOperationWindows = false;
            }
        }

        private void SaveSettingsEventHandler(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.FormHeight = this.Height;
            Properties.Settings.Default.FormWidth = this.Width;
            Properties.Settings.Default.FormLocation = this.Location;
            if (Properties.Settings.Default.FirstRun)
            {
                Properties.Settings.Default.FirstRun = false;
            }
            Properties.Settings.Default.Save();
        }

        #region Calculate Space To Regain
        private void Form1_Load(object sender, EventArgs e)
        {
            checkedArray = new CheckBox[16] 
            { 
                checkBox1, 
                checkBox2, 
                checkBox3, 
                checkBox4, 
                checkBox5, 
                checkBox6, 
                checkBox7, 
                checkBox8, 
                checkBox9, 
                checkBox10, 
                checkBox11, 
                checkBox12, 
                checkBox13, 
                checkBox14, 
                checkBox15, 
                checkBox18
            };

            checkedArrayBool = new bool[16] 
            { 
                checkBox1.Checked, 
                checkBox2.Checked, 
                checkBox3.Checked, 
                checkBox4.Checked, 
                checkBox5.Checked, 
                checkBox6.Checked, 
                checkBox7.Checked, 
                checkBox8.Checked, 
                checkBox9.Checked, 
                checkBox10.Checked, 
                checkBox11.Checked, 
                checkBox12.Checked, 
                checkBox13.Checked, 
                checkBox14.Checked, 
                checkBox15.Checked, 
                checkBox18.Checked
            };

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
            tempDirSize = Directory.GetFiles(tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
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
                ScrollToOutputBottom();
            }
            tempInternetSizeInMegabytes = tempInternetFilesDirSize / 1024 / 1024;

            // Get size of Windows Error Reports
            windowsErrorReportsDirectory = programDataDirectory + "\\Microsoft\\Windows\\WER\\ReportArchive";
            if(Directory.Exists(windowsErrorReportsDirectory))
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
                            richTextBox1.AppendText(ex.Message + "\n", Color.Red);
                        }
                    }
                    else if (ex.GetType() == typeof(UnauthorizedAccessException)) 
                    {
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText(ex.Message + "\n", Color.Red);
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

            ScrollToOutputBottom();

            // Show potential reclamation, then each individual category.
            richTextBox1.AppendText("Potential space to reclaim: " + 
                (tempDirSizeInMegaBytes + tempSetupSizeInMegabytes + tempInternetSizeInMegabytes + windowsErrorReportsDirSizeInMegabytes + 
                deliveryOptimizationFilesDirSizeInMegabytes + totalChromeDirSize + totalEdgeDirSize + msoCacheDirSize + windowsInstallerCacheDirSize + 
                windowsUpdateLogDirSize) + "MB" + "\n" + "\n" + "Categorical breakdown:" + "\n" + 
                "----------------------------------------------------------------------------------------------------------------------------------------" + "\n");

            // List out total space reclamation per category.
            richTextBox1.AppendText("Temp Directory size: " + tempDirSizeInMegaBytes + "MB" + "\n");
            richTextBox1.AppendText("Temporary Setup Files directory size: " + tempSetupSizeInMegabytes + "MB" + "\n");
            richTextBox1.AppendText("Temporary Internet Files directory size: " + tempInternetSizeInMegabytes + "MB" + "\n");
            richTextBox1.AppendText("Windows Error Reports size: " + windowsErrorReportsDirSizeInMegabytes + "MB" + "\n");
            richTextBox1.AppendText("Windows Delivery Optimization File size: " + deliveryOptimizationFilesDirSizeInMegabytes + "MB" + "\n");
            richTextBox1.AppendText("Chrome Data Size: " + totalChromeDirSize + "MB" + "\n");
            richTextBox1.AppendText("Edge Data Size: " + totalEdgeDirSize + "MB" + "\n");
            richTextBox1.AppendText("MSO Cache size: " + msoCacheDirSize + "MB" + "\n");
            richTextBox1.AppendText("Windows Installer Cache size: " + windowsInstallerCacheDirSize + "MB" + "\n");
            richTextBox1.AppendText("Windows Update Logs size: " + windowsUpdateLogDirSize + "MB" + "\n" + "\n");

        }
        #endregion

        #region Sweep Button
        // Sweep button function.
        private async void Button1_Click_1(object sender, EventArgs e)
        {
            // Lock cleaning until the entire operation is done.
            LockCleaning(true);

            #region Temporary Files Removal
            // Temporary Files removal.
            if (checkBox1.Checked)
            {
                tempSizeBeforeDelete = tempDirSizeInMegaBytes;

                richTextBox1.AppendText("Sweeping Temporary Files..." + "\n", Color.Green);
                DirectoryInfo di = new DirectoryInfo(tempDirectory);

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
                                richTextBox1.AppendText("Deleted: " + file.Name + "\n", Color.Green);
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                        }
                        ScrollToOutputBottom();
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed files.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("Couldn't delete " + file.Name + ": " + ex.Message + " Skipping..." + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
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
                                richTextBox1.AppendText("Deleted: " + dir.Name + "\n", Color.Green);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("Couldn't delete " + dir.Name + ": " + ex.Message + " Skipping..." + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                    }
                }
                richTextBox1.AppendText("\n" + "Swept Temporary Files!" + "\n" + "\n", Color.Green);
                tempFilesWereRemoved = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Temporary Setup Files Removal
            // Temporary Setup Files removal
            if (checkBox2.Checked)
            {
                tempSetupFilesSizeBeforeDelete = tempSetupSizeInMegabytes;

                richTextBox1.AppendText("Sweeping Temporary Setup Files..." + "\n", Color.Green);
                DirectoryInfo di = new DirectoryInfo(tempSetupDirectory);

                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        if (!File.Exists(file.Name))
                        {
                            if (isVerboseMode)
                            {
                                richTextBox1.AppendText("Deleted: " + file.Name + "\n", Color.Green);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed files.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("Couldn't delete " + file.Name + ": " + ex.Message + " Skipping..." + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
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
                                richTextBox1.AppendText("Deleted: " + dir.Name + "\n", Color.Green);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("Couldn't delete " + dir.Name + ": " + ex.Message + " Skipping..." + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                    }
                }
                richTextBox1.AppendText("\n" + "Swept Temporary Setup Files!" + "\n" + "\n", Color.Green);
                tempSetupFilesWereRemoved = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Temporary Internet Files Removal
            // Temporary Setup Files removal
            if (checkBox3.Checked)
            {
                tempInternetFilesBeforeDelete = tempInternetSizeInMegabytes;

                richTextBox1.AppendText("Sweeping Temporary Internet Files..." + "\n", Color.Green);
                DirectoryInfo di = new DirectoryInfo(tempInternetFilesDirectory);

                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        if (!File.Exists(file.Name))
                        {
                            if (isVerboseMode)
                            {
                                richTextBox1.AppendText("Deleted: " + file.Name + "\n", Color.Green);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed files.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("Couldn't delete " + file.Name + ": " + ex.Message + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
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
                                richTextBox1.AppendText("Deleted: " + dir.Name + "\n", Color.Green);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("Couldn't delete " + dir.Name + ": " + ex.Message + " Skipping..." + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                    }
                }
                richTextBox1.AppendText("\n" + "Swept Temporary Internet Files!" + "\n" + "\n", Color.Green);
                tempInternetFilesWereRemoved = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Event Viewer Logs Removal
            // Event Viewer Logs Removal.
            if (checkBox4.Checked)
            {
                richTextBox1.AppendText("Sweeping Event Viewer Logs", Color.Green);
                ScrollToOutputBottom();
                AddWaitText();

                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                });
                
                richTextBox1.AppendText("\n" + "Swept Event Viewer Logs!" + "\n" + "\n", Color.Green);
                eventLogsCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Empty Recycle Bin
            // Empty Recycle Bin.
            if (checkBox5.Checked)
            {
                richTextBox1.AppendText("Emptying Recycle Bin", Color.Green);
                ScrollToOutputBottom();

                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                });
                
                richTextBox1.AppendText("\n" + "Emptied Recycle Bin!" + "\n" + "\n", Color.Green);
                isRecycleBinEmpty = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Windows Error Reports Removal
            // Temporary Setup Files removal
            if (checkBox6.Checked)
            {
                windowsErrorReportsDirSizeBeforeDelete = windowsErrorReportsDirSizeInMegabytes;

                richTextBox1.AppendText("Sweeping Windows Error Reports" + "\n", Color.Green);
                if (Directory.Exists(windowsErrorReportsDirectory))
                {
                    DirectoryInfo di = new DirectoryInfo(windowsErrorReportsDirectory);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                            if (!File.Exists(file.Name))
                            {
                                if (isVerboseMode)
                                {
                                    richTextBox1.AppendText("Deleted: " + file.Name + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }
                                else
                                {
                                    richTextBox1.AppendText("o", Color.Green);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // Skip all failed files.
                            if (isVerboseMode)
                            {
                                richTextBox1.AppendText(file.Name + " appears to be in use or locked. Skipping..." + "\n", Color.Red);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("x", Color.Red);
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
                                    richTextBox1.AppendText("Deleted: " + dir.Name + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }
                                else
                                {
                                    richTextBox1.AppendText("o", Color.Green);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Skip all failed directories.
                            if (isVerboseMode)
                            {
                                richTextBox1.AppendText("Couldn't remove " + dir.Name + ": " + ex.Message + ". Skipping..." + "\n", Color.Red);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("x", Color.Red);
                            }
                        }
                    }
                    richTextBox1.AppendText("Swept Windows Error Reports!" + "\n" + "\n", Color.Green);
                }
                else
                {
                    richTextBox1.AppendText("Windows Error Report directory already removed. Skipping" + "\n" + "\n", Color.Green);
                }
                windowsErrorReportsCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Delivery Optimization Files
            // Delivery Optimization Files Removal
            if (checkBox7.Checked)
            {
                deliveryOptimizationFilesDirSizeBeforeDelete = deliveryOptimizationFilesDirSizeInMegabytes;

                richTextBox1.AppendText("Sweeping Delivery Optimization Files..." + "\n", Color.Green);
                DirectoryInfo di = new DirectoryInfo(deliveryOptimizationFilesDirectory);

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
                                    richTextBox1.AppendText("Deleted: " + file.Name + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }
                                else
                                {
                                    richTextBox1.AppendText("o", Color.Green);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Skip all failed files.
                            if (isVerboseMode)
                            {
                                richTextBox1.AppendText("Couldn't delete " + file.Name + ": " + ex.Message + ". Skipping..." + "\n", Color.Red);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("x", Color.Red);
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
                                    richTextBox1.AppendText("Deleted: " + dir.Name + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }
                                else
                                {
                                    richTextBox1.AppendText("o", Color.Green);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Skip all failed directories.
                            if (isVerboseMode)
                            {
                                richTextBox1.AppendText("Couldn't delete " + dir.Name + ": " + ex.Message + ". Skipping..." + "\n", Color.Red);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("x", Color.Red);
                            }
                        }
                    }
                    richTextBox1.AppendText("\n" + "Removed Delivery Optimization Files!" + "\n" + "\n", Color.Green);
                }
                else
                {
                    richTextBox1.AppendText("No Delivery Optimization Files needed to be cleaned." + "\n" + "\n", Color.Green);
                }
                wereDeliveryOptimizationFilesRemoved = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Thumbnail Cache Removal
            // Thumbnail Cache Removal.
            if (checkBox8.Checked)
            {
                richTextBox1.AppendText("Clearing Thumbnail Cache", Color.Green);
                ScrollToOutputBottom();
                AddWaitText();

                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    if (showOperationWindows)
                    {
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    }
                    else
                    {
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    }
                    startInfo.FileName = "cmd.exe";
                    Invoke(new Action(() =>
                    {
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("\n" + "Shutting down Explorer.exe process...", Color.Green);
                        }
                    }));
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                });
                Invoke(new Action(() =>
                {
                    if (isVerboseMode)
                    {
                        richTextBox1.AppendText("\n" + "Restarted Explorer.exe process.", Color.Green);
                    }
                }));
                richTextBox1.AppendText("\n" + "Cleared Thumbnail Cache!" + "\n" + "\n", Color.Green);
                thumbnailCacheCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region User File History Removal
            // User File History Removal.
            if (checkBox9.Checked)
            {
                richTextBox1.AppendText("Attempting to remove all file history snapshots except latest", Color.Green);
                ScrollToOutputBottom();
                AddWaitText();

                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                });
                richTextBox1.AppendText("\n" + "If file history was enabled, all versions except latest were removed." + "\n" + "\n", Color.Green);
                deletedFileHistory = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Windows.old Directory Removal
            // Windows.old Directory Removal.
            if (checkBox10.Checked)
            {
                richTextBox1.AppendText("Removing old versions of Windows", Color.Green);
                ScrollToOutputBottom();
                if (Directory.Exists("C:\\windows.old"))
                {
                    richTextBox1.AppendText("\n" + "Found Windows.old directory. Cleaning", Color.Green);
                    AddWaitText();
                    await Task.Run(() =>
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                                Invoke(new Action(() =>
                                {
                                    ScrollToOutputBottom();
                                }));
                                break;
                            }
                        }
                    });
                    richTextBox1.AppendText("\n" + "Cleaned Windows.old directory!" + "\n" + "\n", Color.Green);
                    windowsOldCleaned = true;
                }
                else
                {
                    richTextBox1.AppendText("\n" + "No Windows.old directory found. Skipping..." + "\n" + "\n", Color.Green);
                    windowsOldCleaned = false;
                }
                ScrollToOutputBottom();
            }
            #endregion
            #region Windows Defender Log Files Removal
            // Windows Defender Log Files Removal.
            if (checkBox11.Checked)
            {
                richTextBox1.AppendText("Deleting Windows Defender Log Files..." + "\n", Color.Green);
                ScrollToOutputBottom();

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
                await Task.Run(() =>
                {
                    foreach (string directory in directoryPath)
                    {
                        DirectoryInfo dir = new DirectoryInfo(directory);
                        foreach (FileInfo f in dir.GetFiles())
                        {
                            if (f.Name.Contains(".log"))
                                try
                                {
                                    File.Delete(f.FullName);
                                    if (isVerboseMode)
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("Removed " + f.Name + "\n", Color.Green);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("o", Color.Green);
                                        }));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (isVerboseMode)
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText(ex.Message + " Skipping..." + "\n", Color.Red);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("x" + f.Name + "\n", Color.Red);
                                        }));
                                    }
                                }
                        }
                    }
                });

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
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("Removed " + logFileDirectory + "\n", Color.Green);
                                }));
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("o", Color.Green);
                                }));
                            }
                        }
                        catch (Exception ex)
                        {
                            if (isVerboseMode)
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText(ex.Message + "Skipping..." + "\n", Color.Red);

                                }));
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("x", Color.Red);
                                }));
                            }
                        }
                    }
                    else
                    {
                        if (isVerboseMode)
                        {
                            Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText("Directory already removed: " + logFileDirectory + "\n", Color.Red);
                            }));
                        }
                    }
                }
                richTextBox1.AppendText("\n" + "Removed Windows Defender Logs!" + "\n" + "\n", Color.Green);
                windowsDefenderLogsCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Microsoft Office Cache Removal
            // Microsoft Office Cache Removal.
            if (checkBox12.Checked)
            {
                richTextBox1.AppendText("Sweeping MSO cache", Color.Green);
                ScrollToOutputBottom();
                if (Directory.Exists(msoCacheDir))
                {
                    Directory.Delete(msoCacheDir, true);
                    richTextBox1.AppendText("\n" + "Swept MSO Cache!" + "\n" + "\n", Color.Green);
                }
                else
                {
                    richTextBox1.AppendText("\n" + "No MSOCache directory found. Skipping..." + "\n" + "\n", Color.Green);
                }

                msoCacheCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Microsoft Edge Cache Removal
            // Edge Cache Removal
            if (checkBox13.Checked)
            {
                richTextBox1.AppendText("Sweeping Edge cache..." + "\n", Color.Green);
                ScrollToOutputBottom();
                if (isVerboseMode)
                {
                    richTextBox1.AppendText("Ending any Edge processes", Color.Green);
                    ScrollToOutputBottom();
                }
                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                    Invoke(new Action(() =>
                    {
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("\n" + "Stopped Edge processes." + "\n", Color.Green);
                            ScrollToOutputBottom();
                        }
                    }));
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
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("Removed: " + edgeDirectory + "." + "\n", Color.Green);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("x", Color.Green);
                                        }));
                                    }
                                }
                            }
                            catch (IOException ex)
                            {
                                if (isVerboseMode)
                                {
                                    Invoke(new Action(() =>
                                    {
                                        richTextBox1.AppendText("Skipping: " + edgeDirectory + ". " + ex.Message + "\n", Color.Green);
                                        ScrollToOutputBottom();
                                    }));
                                }
                                else
                                {
                                    Invoke(new Action(() =>
                                    {
                                        richTextBox1.AppendText("x", Color.Red);
                                    }));
                                }
                            }
                        }
                        else
                        {
                            if (isVerboseMode) 
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("Directory already removed: " + edgeDirectory + "." + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }));
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("x", Color.Red);
                                    ScrollToOutputBottom();
                                }));
                            }
                        }
                    }
                });

                richTextBox1.AppendText("\n" + "Swept Edge cache!" + "\n" + "\n", Color.Green);
                sweptEdgeCache = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Chrome Cache Removal
            // Chrome Cache Removal
            if (checkBox14.Checked)
            {
                richTextBox1.AppendText("Sweeping Chrome cache..." + "\n", Color.Green);
                ScrollToOutputBottom();
                if (showOperationWindows)
                {
                    richTextBox1.AppendText("Ending any Chrome processes", Color.Green);
                    ScrollToOutputBottom();
                }
                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                    Invoke(new Action(() =>
                    {
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("\n" + "Stopped Chrome processes." + "\n", Color.Green);
                            ScrollToOutputBottom();
                        }
                    }));
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
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("Removed: " + chromeDirectory + "." + "\n", Color.Green);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("x", Color.Green);
                                        }));
                                    }
                                }
                            }
                            catch (SystemException ex)
                            {
                                if (ex.GetType() == typeof(IOException))
                                {
                                    if (isVerboseMode)
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("Skipping: " + chromeDirectory + ". " + ex.Message + "\n", Color.Red);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("x", Color.Red);
                                        }));
                                    }
                                }
                                else if (ex.GetType() == typeof(UnauthorizedAccessException))
                                {
                                    if (isVerboseMode)
                                    {
                                        richTextBox1.AppendText("Skipping: " + chromeDirectory + ". " + ex.Message + "\n", Color.Red);
                                        ScrollToOutputBottom();
                                    }
                                    else
                                    {
                                        richTextBox1.AppendText("x", Color.Red);
                                    }
                                }
                                else
                                {
                                    if (isVerboseMode)
                                    {
                                        richTextBox1.AppendText("Skipping: " + chromeDirectory + ". " + ex.Message + "\n", Color.Red);
                                    }
                                    else
                                    {
                                        richTextBox1.AppendText("x", Color.Red);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (isVerboseMode)
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("Directory already removed: " + chromeDirectory + "." + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }));
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("x", Color.Red);
                                }));
                            }
                        }
                    }
                });

                sweptChromeCache = true;
                richTextBox1.AppendText("\n" + "Swept Chrome cache!" + "\n" + "\n", Color.Green);
                ScrollToOutputBottom();
            }
            #endregion
            #region Windows Installer Cache Removal
            // Windows Installer Cache Removal
            if (checkBox15.Checked)
            {
                richTextBox1.AppendText("Removing Windows Installer Cache", Color.Green);
                ScrollToOutputBottom();
                if (Directory.Exists(windowsDirectory + "\\Installer\\$PatchCache$\\Managed"))
                {
                    richTextBox1.AppendText("\n" + "Found Windows Installer Cache. Cleaning", Color.Green);
                    AddWaitText();
                    await Task.Run(() =>
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                                Invoke(new Action(() =>
                                {
                                    ScrollToOutputBottom();
                                }));
                                break;
                            }
                        }
                    });
                    richTextBox1.AppendText("\n" + "Cleaned Windows Installer Cache!" + "\n" + "\n", Color.Green);
                }
                ScrollToOutputBottom();
                windowsInstallerCacheCleared = true;
            }
            #endregion
            #region Windows Update Logs Removal
            // Windows Update Logs Removal
            if (checkBox18.Checked)
            {
                richTextBox1.AppendText("Sweeping Windows Update Logs" + "\n", Color.Green);
                ScrollToOutputBottom();
                try
                {
                    if (Directory.Exists(windowsUpdateLogDir))
                    {
                        DirectoryInfo di = new DirectoryInfo(tempDirectory);

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
                                        richTextBox1.AppendText("Deleted: " + file.Name + "\n", Color.Green);
                                    }
                                    else
                                    {
                                        richTextBox1.AppendText("o", Color.Green);
                                    }
                                }
                                else
                                {
                                    richTextBox1.AppendText("x", Color.Red);
                                }
                                ScrollToOutputBottom();
                            }
                            catch (Exception ex)
                            {
                                if (ex.GetType() == typeof(IOException))
                                {
                                    if (isVerboseMode)
                                    {
                                        richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                                    }
                                    else
                                    {
                                        richTextBox1.AppendText("x", Color.Red);
                                    }
                                }
                                else if (ex.GetType() == typeof(UnauthorizedAccessException))
                                {
                                    if (isVerboseMode)
                                    {
                                        richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                                    }
                                    else
                                    {
                                        richTextBox1.AppendText("x", Color.Red);
                                    }
                                }
                            }
                        }
                        richTextBox1.AppendText("\n" + "Swept Windows Update Logs!" + "\n" + "\n", Color.Green);
                    }
                    else
                    {
                        richTextBox1.AppendText("\n" + "No Windows Update Logs directory found. Skipping..." + "\n" + "\n", Color.Green);
                    }

                    ScrollToOutputBottom();
                    windowsUpdateLogsCleared = true;
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IOException))
                    {
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                        }
                    }
                    else if (ex.GetType() == typeof(UnauthorizedAccessException))
                    {
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                        }
                    }
                }
            }
            #endregion


            #region Calculate Space Saved
            richTextBox1.AppendText("\n" + "\n" + "Recovery results:", Color.Green);
            richTextBox1.AppendText("\n" + "----------------------------------------------------------------------------------------------------------------------------------------", Color.Green);

            if (tempFilesWereRemoved)
            {
                tempFilesWereRemoved =  false;
                // Get new Temporary Files size and output what was saved.
                tempDirSize = Directory.GetFiles(tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                tempDirSizeInMegaBytes = tempDirSize / 1024 / 1024;
                newTempDirSize = tempSizeBeforeDelete - tempDirSizeInMegaBytes;
                totalSpaceSaved += newTempDirSize;
                if (newTempDirSize > 0)
                {
                    richTextBox1.AppendText("\n" + "Recovered " + newTempDirSize + "MB from removing Temporary Files.", Color.Green);
                }
                else
                {
                    richTextBox1.AppendText("\n" + "<1MB recovered from Temporary Files...", Color.Green);
                }
                ScrollToOutputBottom();
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
                    richTextBox1.AppendText("\n" + "Recovered " + newTempSetupDirSize + "MB from removing Temporary Setup Files.", Color.Green);
                }
                else
                {
                    richTextBox1.AppendText("\n" + "<1MB recovered from Temporary Setup Files...", Color.Green);
                }
                ScrollToOutputBottom();
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
                        richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                    }
                }
                tempInternetSizeInMegabytes = tempInternetFilesDirSize / 1024 / 1024;
                newTempInternetFilesDirSize = tempInternetFilesBeforeDelete - tempInternetSizeInMegabytes;
                totalSpaceSaved += newTempInternetFilesDirSize;
                if (newTempInternetFilesDirSize > 0)
                {
                    richTextBox1.AppendText("\n" + "Recovered " + newTempInternetFilesDirSize + "MB from removing Temporary Internet Files.", Color.Green);
                }
                else
                {
                    richTextBox1.AppendText("\n" + "<1MB recovered from Temporary Internet Files...", Color.Green);
                }
                ScrollToOutputBottom();
            }

            if (eventLogsCleared)
            {
                // Mark the sweeping of Event Logs as completed to allow re-sweeping.
                eventLogsCleared = false;
                richTextBox1.AppendText("\n" + "Event Logs were cleared.", Color.Green);
                ScrollToOutputBottom();
            }

            if (isRecycleBinEmpty)
            {
                isRecycleBinEmpty = false;
                richTextBox1.AppendText("\n" + "Recycle Bin was emptied.", Color.Green);
                ScrollToOutputBottom();
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
                    richTextBox1.AppendText("\n" + "Recovered " + newWindowsErrorReportsDirSize + "MB from removing Windows Error Reports.", Color.Green);
                }
                else
                {
                    richTextBox1.AppendText("\n" + "<1MB recovered from removing Windows Error Reports...", Color.Green);
                }
                ScrollToOutputBottom();
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
                    richTextBox1.AppendText("\n" + "Recovered " + newDeliveryOptimizationSize + "MB from removing Windows Delivery Optimization Files.", Color.Green);
                }
                else
                {
                    richTextBox1.AppendText("\n" + "<1MB recovered from Windows Delivery Optimization Files...", Color.Green);
                }
                ScrollToOutputBottom();
            }

            if (thumbnailCacheCleared)
            {
                thumbnailCacheCleared = false;
                richTextBox1.AppendText("\n" + "Thumbnail cache was cleared.", Color.Green);
                ScrollToOutputBottom();
            }

            if (deletedFileHistory)
            {
                deletedFileHistory = false;
                richTextBox1.AppendText("\n" + "Removed file history older than latest snapshot.", Color.Green);
                ScrollToOutputBottom();
            }

            if (windowsOldCleaned)
            {
                windowsOldCleaned = false;
                richTextBox1.AppendText("\n" + "Old versions of Windows were removed.", Color.Green);
                ScrollToOutputBottom();
            }

            if (windowsDefenderLogsCleared)
            {
                windowsDefenderLogsCleared = false;
                richTextBox1.AppendText("\n" + "Windows Defender Logs were removed.", Color.Green);
                ScrollToOutputBottom();
            }
            if (sweptChromeCache)
            {
                sweptChromeCache = false;
                richTextBox1.AppendText("\n" + "Chrome cache was cleared.", Color.Green);
                ScrollToOutputBottom();
            }

            if (sweptEdgeCache)
            {
                sweptEdgeCache = false;
                richTextBox1.AppendText("\n" + "Edge cache was cleared.", Color.Green);
                ScrollToOutputBottom();
            }

            if (msoCacheCleared)
            {
                msoCacheCleared = false;
                // Get size of MSOCache.
                try
                {
                    if (!Directory.Exists(msoCacheDir))
                    {
                        richTextBox1.AppendText("\n" + "Recovered " + msoCacheDirSize + "MB from removing MSO Cache.", Color.Green);
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
                                richTextBox1.AppendText("\n" + "Recovered " + newMsoCacheDirSize + "MB from removing MSO Cache.", Color.Green);
                            }
                            else if (ex.GetType() == typeof(UnauthorizedAccessException))
                            {
                                richTextBox1.AppendText("\n" + "Recovered " + newMsoCacheDirSize + "MB from removing MSO Cache.", Color.Green);
                            }
                        }
                    }
                    ScrollToOutputBottom();
                }
                catch (Exception ex)
                {
                    if (isVerboseMode)
                    {
                        richTextBox1.AppendText("\n" + ex.Message, Color.Red);
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
                        richTextBox1.AppendText("\n" + "Recovered " + oldWindowsInstallerCacheDirSize + "MB from removing Windows Installer Cache.", Color.Green);
                    }
                    totalSpaceSaved += newWindowsInstallerCacheDirSize;
                }
                ScrollToOutputBottom();
            }

            if (windowsUpdateLogsCleared)
            {
                windowsUpdateLogsCleared = false;
                // Get size of Windows Update Logs.
                if (!Directory.Exists(windowsUpdateLogDir))
                {
                    richTextBox1.AppendText("\n" + "Recovered " + windowsUpdateLogDirSize + "MB from removing Windows Update Logs.", Color.Green);
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
                        richTextBox1.AppendText("\n" + "Recovered " + newWindowsUpdateLogDirSize + "MB from removing Windows Update Logs.", Color.Green);
                    }
                    else
                    {
                        richTextBox1.AppendText("\n" + "<1MB recovered from Windows Update Logs...", Color.Green);
                    }
                }
                ScrollToOutputBottom();
            }
            
            // Output the total space saved from the entire operation and other important completed actions.
            if (totalSpaceSaved > 0)
            {
                richTextBox1.AppendText("\n" + "\n" + "Total space recovered: " + totalSpaceSaved + "MB" + "\n", Color.Green);
            }
            else
            {
                richTextBox1.AppendText("\n" + "\n" + "Total space recovered: <1MB" + "\n", Color.Green);
            }
            ScrollToOutputBottom();
            totalSpaceSaved = 0;
            LockCleaning(false);
            CanCleanStatus();
            RemoveAllChecks();
            #endregion
        }
        #endregion

        private void ScrollToOutputBottom()
        {
            // Scroll to the bottom of the output window.
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void AddWaitText()
        {
            Invoke(new Action(() =>
            {
                richTextBox1.AppendText(".", Color.Green);
            }));
        }

        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        // Checkbox Checked Functions
        #region Checkboxes
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox1.Checked, 0);
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox2.Checked, 1);
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox3.Checked, 2);
        }

        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox4.Checked, 3);
        }

        private void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox5.Checked, 4);
        }

        private void CheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox6.Checked, 5);
        }

        private void CheckBox7_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox7.Checked, 6);
        }

        private void CheckBox8_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox8.Checked, 7);
        }
        private void CheckBox9_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox9.Checked, 8);
        }

        private void CheckBox10_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox10.Checked, 9);
        }

        private void CheckBox11_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox11.Checked, 10);
        }

        private void CheckBox12_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox12.Checked, 11);
        }

        private void CheckBox13_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox13.Checked, 12);
        }

        private void CheckBox14_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox14.Checked, 13);
        }

        private void CheckBox15_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox15.Checked, 14);
        }

        private void CheckBox18_CheckedChanged_1(object sender, EventArgs e)
        {
            ToggleCleanableStatus(checkBox18.Checked, 15);
        }
        #endregion

        // Check each checkbox's status and update the cleanable status immediately.
        private void ToggleCleanableStatus(bool boxCheckedOperationPosition, int indexPosition)
        {
            if (boxCheckedOperationPosition)
            {
                checkedArrayBool[indexPosition] = true;
            }
            else
            {
                checkedArrayBool[indexPosition] = false;
            }
            CanCleanStatus();
        }

        // If any box is checked, allow cleaning. Otherwise, disable cleaning.
        private void CanCleanStatus()
        {
            if (checkedArrayBool.Contains(true))
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void LockCleaning(bool isEnabled)
        {
            // Lock or unlock all checkboxes depending on whether cleaning operation is in progress.
            if (isEnabled)
            {
                foreach (var box in checkedArray)
                {
                    box.Enabled = false;
                }

                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                foreach (var box in checkedArray)
                {
                    box.Enabled = true;
                }

                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void RemoveAllChecks()
        {
            foreach (CheckBox box in checkedArray)
            {
                box.Checked = false;
            }
        }

        private void PlaceAllChecks()
        {
            foreach (CheckBox box in checkedArray)
            {
                box.Checked = true;
            }
        }

        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_octoClient == null)
            {
                _octoClient = new octo.GitHubClient(new octo.ProductHeaderValue("CleanSweep2"));
                CheckForUpdates();
            }
            else if (_octoClient != null)
            {
                CheckForUpdates();
            }
            else
            {
                MessageBox.Show("Couldn't connect to get updates. Your connection might be down, you're checking too often, or Github is down. Try checking again later?",
                    "Couldn't Connect!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CheckForUpdates()
        {
            var releases = await _octoClient.Repository.Release.GetAll("thomasloupe", "CleanSweep2").ConfigureAwait(false);
            var latest = releases[0];
            if (CurrentVersion == latest.TagName)
            {
                MessageBox.Show("You have the latest version, " + CurrentVersion + "!", CurrentVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (CurrentVersion != latest.TagName)
            {
                var result = MessageBox.Show("A new version of CleanSweep is available!\n"
                    + "Current version: " + CurrentVersion + "\n"
                    + "Latest version: " + latest.TagName + "\n"
                    + "Would you like to visit the download page?", "Update Available!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/thomasloupe/CleanSweep2/releases/tag/" +
                                                     latest.TagName);
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
            Application.Exit();
        }

        private void DonateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("CleanSweep will always be free!\n"
            + "If you'd like to buy me a beer anyway, I won't tell you no!\n"
            + "Would you like to open the donation page now?", "CleanSweep is free, but beer is not!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://paypal.me/thomasloupe");
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CleanSweep " + CurrentVersion + ".\n" +
            "Created by Thomas Loupe." + "\n" +
            "Github: https://github.com/thomasloupe" + "\n" +
            "Twitter: https://twitter.com/acid_rain" + "\n" +
            "Website: https://thomasloupe.com", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void VerboseModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (verboseModeToolStripMenuItem.Checked)
            {
                Properties.Settings.Default.Verbosity = true;
                isVerboseMode = true;
            }
            else
            {
                Properties.Settings.Default.Verbosity = false;
                isVerboseMode = false;
            }
            Properties.Settings.Default.Save();
        }

        private void ShowOperationWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showOperationWindowsToolStripMenuItem.Checked)
            {
                Properties.Settings.Default.OperationWindows = true;
                showOperationWindows = true;
            }
            else
            {
                Properties.Settings.Default.OperationWindows = false;
                showOperationWindows = false;
            }
            Properties.Settings.Default.Save();
        }

        private void ToolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripButton1_Click_1(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength == 0)
            {
                richTextBox1.SelectAll();
                richTextBox1.Copy();
            }
        }

        private void ToolStripButton2_Click_2(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            PlaceAllChecks();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            RemoveAllChecks();
        }
    }
}
public static class RichTextBoxExtensions
{
    public static void AppendText(this RichTextBox box, string text, Color color)
    {
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;

        box.SelectionColor = color;
        box.AppendText(text);
        box.SelectionColor = box.ForeColor;
    }
}
