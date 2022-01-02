using CleanSweep2.Properties;
using Octo = Octokit;
using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CleanSweep2
{
    public partial class Form1 : Form
    {
        #region Declarations
        private const string CurrentVersion = "v2.3.1";
        private Octo.GitHubClient _octoClient;
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
        private bool[] _checkedArrayBool;
        private ToolStripMenuItem[] _languageBoxes;
        private long _totalSpaceSaved;
        private CheckBox[] _checkedArray;

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
        public Form1()
        {
            InitializeComponent();
            SetWindowSizeAndLocation();
            SetVerbosity();
            SetOperationWindows();
            var language = ConfigurationManager.AppSettings["language"];
            Console.WriteLine(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
        }

        private void SetWindowSizeAndLocation()
        {
            Location = Settings.Default.FormLocation;
            Height = Settings.Default.FormHeight;
            Width = Settings.Default.FormWidth;
            FormClosing += SaveSettingsEventHandler;
            StartPosition = FormStartPosition.Manual;
        }

        private void SetVerbosity()
        {
            // Set value of Verbosity equal to what's stored in the application's setting.
            if (Settings.Default.Verbosity)
            {
                verboseModeToolStripMenuItem.Checked = true;
                _isVerboseMode = true;
            }
            else
            {
                verboseModeToolStripMenuItem.Checked = false;
                _isVerboseMode = false;
            }
        }

        private void SetOperationWindows()
        {
            // Set value of Show Operation Windows equal to what's stored in the application's setting.
            if (Settings.Default.OperationWindows)
            {
                showOperationWindowsToolStripMenuItem.Checked = true;
                _showOperationWindows = true;
            }
            else
            {
                showOperationWindowsToolStripMenuItem.Checked = false;
                _showOperationWindows = false;
            }
        }

        private void RestoreSavedChecks() 
        {
            checkBox1.Checked = Settings.Default.Box1Checked;
            checkBox2.Checked = Settings.Default.Box2Checked;
            checkBox3.Checked = Settings.Default.Box3Checked;
            checkBox4.Checked = Settings.Default.Box4Checked;
            checkBox5.Checked = Settings.Default.Box5Checked;
            checkBox6.Checked = Settings.Default.Box6Checked;
            checkBox7.Checked = Settings.Default.Box7Checked;
            checkBox8.Checked = Settings.Default.Box8Checked;
            checkBox9.Checked = Settings.Default.Box9Checked;
            checkBox10.Checked = Settings.Default.Box10Checked;
            checkBox11.Checked = Settings.Default.Box11Checked;
            checkBox12.Checked = Settings.Default.Box12Checked;
            checkBox13.Checked = Settings.Default.Box13Checked;
            checkBox14.Checked = Settings.Default.Box14Checked;
            checkBox15.Checked = Settings.Default.Box15Checked;
            checkBox18.Checked = Settings.Default.Box18Checked;
        }

        private void SaveSettingsEventHandler(object sender, FormClosingEventArgs e)
        {
            Settings.Default.FormHeight = this.Height;
            Settings.Default.FormWidth = this.Width;
            Settings.Default.FormLocation = this.Location;
            if (Settings.Default.FirstRun)
            {
                Settings.Default.FirstRun = false;
            }
            Settings.Default.Save();
        }

        #region Calculate Space To Regain
        private void Form1_Load(object sender, EventArgs e)
        {
            _checkedArray = new [] 
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

            _checkedArrayBool = new [] 
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

            _languageBoxes = new []
            {
                englishToolStripMenuItem,
                frenchToolStripMenuItem,
                germanToolStripMenuItem,
                italianToolStripMenuItem,
                spanishToolStripMenuItem,
                russianToolStripMenuItem
            };

            RestoreSavedChecks();

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
            catch (Exception)
            {
                ScrollToOutputBottom();
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
            catch (Exception)
            {
                ScrollToOutputBottom();
            }
            _tempInternetSizeInMegabytes = _tempInternetFilesDirSize / 1024 / 1024;

            // Get size of Windows Error Reports
            _windowsErrorReportsDirectory = _programDataDirectory + "\\Microsoft\\Windows\\WER\\ReportArchive";
            if(Directory.Exists(_windowsErrorReportsDirectory))
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
                            richTextBox1.AppendText(ex.Message + "\n", Color.Red);
                        }
                    }
                    else if (ex.GetType() == typeof(UnauthorizedAccessException)) 
                    {
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(ex.Message + "\n", Color.Red);
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

            ScrollToOutputBottom();

            // Show potential reclamation, then each individual category.
            richTextBox1.AppendText(Resources.Potential_space_to_reclaim + 
                (_tempDirSizeInMegaBytes + _tempSetupSizeInMegabytes + _tempInternetSizeInMegabytes + _windowsErrorReportsDirSizeInMegabytes +
                 _deliveryOptimizationFilesDirSizeInMegabytes + _totalChromeDirSize + _totalEdgeDirSize + _msoCacheDirSize + _windowsInstallerCacheDirSize +
                 _windowsUpdateLogDirSize) + Resources.MB + "\n" + "\n" + Resources.Categorical_breakdown + "\n" + 
                "----------------------------------------------------------------------------------------------------------------------------------------" + "\n");

            // List out total space reclamation per category.
            richTextBox1.AppendText(Resources.Temp_Directory_size + _tempDirSizeInMegaBytes + Resources.MB + "\n");
            richTextBox1.AppendText(Resources.Temporary_Setup_Files_directory_size + _tempSetupSizeInMegabytes + Resources.MB + "\n");
            richTextBox1.AppendText(Resources.Temporary_Internet_Files_directory_size + _tempInternetSizeInMegabytes + Resources.MB + "\n");
            richTextBox1.AppendText(Resources.Windows_Error_Reports_size + _windowsErrorReportsDirSizeInMegabytes + Resources.MB + "\n");
            richTextBox1.AppendText(Resources.Windows_Delivery_Optimization_File_size + _deliveryOptimizationFilesDirSizeInMegabytes + Resources.MB + "\n");
            richTextBox1.AppendText(Resources.Chrome_Data_Size + _totalChromeDirSize + Resources.MB + "\n");
            richTextBox1.AppendText(Resources.Edge_Data_Size + _totalEdgeDirSize + Resources.MB + "\n");
            richTextBox1.AppendText(Resources.MSO_Cache_size + _msoCacheDirSize + Resources.MB + "\n");
            richTextBox1.AppendText(Resources.Windows_Installer_Cache_size + _windowsInstallerCacheDirSize + Resources.MB + "\n");
            richTextBox1.AppendText(Resources.Windows_Update_Logs_size + _windowsUpdateLogDirSize + Resources.MB + "\n" + "\n");

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
                _tempSizeBeforeDelete = _tempDirSizeInMegaBytes;

                richTextBox1.AppendText(Resources.Sweeping_Temporary_Files, Color.Green);
                var di = new DirectoryInfo(_tempDirectory);

                foreach (var file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        // If file no longer exists, append success to rich text box.
                        if (!File.Exists(file.Name))
                        {
                            if (_isVerboseMode)
                            {
                                richTextBox1.AppendText(Resources.Deleted + file.Name + "\n", Color.Green);
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
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(Resources.Couldn_t_delete + file.Name + ": " + ex.Message + Resources.Skipping + "\n", Color.Red);
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
                            if (_isVerboseMode)
                            {
                                richTextBox1.AppendText(Resources.Deleted + dir.Name + "\n", Color.Green);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("[o]", Color.Green);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(Resources.Couldn_t_delete + dir.Name + ": " + ex.Message + Resources.Skipping + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("[x]", Color.Red);
                        }
                    }
                }
                richTextBox1.AppendText(Resources.Swept_Temporary_Files, Color.Green);
                _tempFilesWereRemoved = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Temporary Setup Files Removal
            // Temporary Setup Files removal
            if (checkBox2.Checked)
            {
                _tempSetupFilesSizeBeforeDelete = _tempSetupSizeInMegabytes;

                richTextBox1.AppendText(Resources.Sweeping_Temporary_Setup_Files, Color.Green);
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
                                richTextBox1.AppendText(Resources.Deleted + file.Name + "\n", Color.Green);
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
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(Resources.Couldn_t_delete + file.Name + ": " + ex.Message + Resources.Skipping + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
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
                                richTextBox1.AppendText(Resources.Deleted + dir.Name + "\n", Color.Green);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("[o]", Color.Green);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(Resources.Couldn_t_delete + dir.Name + ": " + ex.Message + Resources.Skipping + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("[x]", Color.Red);
                        }
                    }
                }
                richTextBox1.AppendText(Resources.Swept_Temporary_Setup_Files, Color.Green);
                _tempSetupFilesWereRemoved = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Temporary Internet Files Removal
            // Temporary Setup Files removal
            if (checkBox3.Checked)
            {
                _tempInternetFilesBeforeDelete = _tempInternetSizeInMegabytes;

                richTextBox1.AppendText(Resources.Sweeping_Temporary_Internet_Files, Color.Green);
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
                                richTextBox1.AppendText(Resources.Deleted + file.Name + "\n", Color.Green);
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
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(Resources.Couldn_t_delete + file.Name + ": " + ex.Message + "\n", Color.Red);
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
                            if (_isVerboseMode)
                            {
                                richTextBox1.AppendText(Resources.Deleted + dir.Name + "\n", Color.Green);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("[o]", Color.Green);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip all failed directories.
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(Resources.Couldn_t_delete + dir.Name + ": " + ex.Message + Resources.Skipping + "\n", Color.Red);
                            ScrollToOutputBottom();
                        }
                        else
                        {
                            richTextBox1.AppendText("[x]", Color.Red);
                        }
                    }
                }
                richTextBox1.AppendText(Resources.Swept_Temporary_Internet_Files, Color.Green);
                _tempInternetFilesWereRemoved = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Event Viewer Logs Removal
            // Event Viewer Logs Removal.
            if (checkBox4.Checked)
            {
                richTextBox1.AppendText(Resources.Sweeping_Event_Viewer_Logs, Color.Green);
                ScrollToOutputBottom();
                AddWaitText();

                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                });
                
                richTextBox1.AppendText(Resources.Swept_Event_Viewer_Logs, Color.Green);
                _eventLogsCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Empty Recycle Bin
            // Empty Recycle Bin.
            if (checkBox5.Checked)
            {
                richTextBox1.AppendText(Resources.Emptying_Recycle_Bin, Color.Green);
                ScrollToOutputBottom();

                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                });
                
                richTextBox1.AppendText(Resources.Emptied_Recycle_Bin, Color.Green);
                _isRecycleBinEmpty = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Windows Error Reports Removal
            // Temporary Setup Files removal
            if (checkBox6.Checked)
            {
                _windowsErrorReportsDirSizeBeforeDelete = _windowsErrorReportsDirSizeInMegabytes;

                richTextBox1.AppendText(Resources.Sweeping_Windows_Error_Reports, Color.Green);
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
                                    richTextBox1.AppendText(Resources.Deleted + file.Name + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }
                                else
                                {
                                    richTextBox1.AppendText("[o]", Color.Green);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // Skip all failed files.
                            if (_isVerboseMode)
                            {
                                richTextBox1.AppendText(file.Name + Resources.appears_to_be_in_use_or_locked__Skipping + "\n", Color.Red);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("[x]", Color.Red);
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
                                    richTextBox1.AppendText(Resources.Deleted + dir.Name + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }
                                else
                                {
                                    richTextBox1.AppendText("[o]", Color.Green);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Skip all failed directories.
                            if (_isVerboseMode)
                            {
                                richTextBox1.AppendText(Resources.Couldn_t_remove + dir.Name + ": " + ex.Message + Resources.Skipping___ + "\n", Color.Red);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("[x]", Color.Red);
                            }
                        }
                    }
                    richTextBox1.AppendText(Resources.Swept_Windows_Error_Reports, Color.Green);
                }
                else
                {
                    richTextBox1.AppendText(Resources.Windows_Error_Report_Directory_Already_Removed_Skipping, Color.Green);
                }
                _windowsErrorReportsCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Delivery Optimization Files
            // Delivery Optimization Files Removal
            if (checkBox7.Checked)
            {
                _deliveryOptimizationFilesDirSizeBeforeDelete = _deliveryOptimizationFilesDirSizeInMegabytes;

                richTextBox1.AppendText(Resources.Sweeping_Delivery_Information_Files, Color.Green);
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
                                    richTextBox1.AppendText(Resources.Deleted + file.Name + "\n", Color.Green);
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
                            if (_isVerboseMode)
                            {
                                richTextBox1.AppendText(Resources.Couldn_t_delete + file.Name + ": " + ex.Message + Resources.Skipping___ + "\n", Color.Red);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("x", Color.Red);
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
                                    richTextBox1.AppendText(Resources.Deleted + dir.Name + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }
                                else
                                {
                                    richTextBox1.AppendText("[o]", Color.Green);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Skip all failed directories.
                            if (_isVerboseMode)
                            {
                                richTextBox1.AppendText(Resources.Couldn_t_delete + dir.Name + ": " + ex.Message + Resources.Skipping___ + "\n", Color.Red);
                                ScrollToOutputBottom();
                            }
                            else
                            {
                                richTextBox1.AppendText("[x]", Color.Red);
                            }
                        }
                    }
                    richTextBox1.AppendText(Resources.Swept_Delivery_Optimization_Files, Color.Green);
                }
                else
                {
                    richTextBox1.AppendText(Resources.No_Delivery_Optimization_Files_Needed_To_Be_Cleaned, Color.Green);
                }
                _wereDeliveryOptimizationFilesRemoved = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Thumbnail Cache Removal
            // Thumbnail Cache Removal.
            if (checkBox8.Checked)
            {
                richTextBox1.AppendText(Resources.Clearing_Thumbnail_Cache, Color.Green);
                ScrollToOutputBottom();
                AddWaitText();

                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    if (_showOperationWindows)
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
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(Resources.Shutting_Down_Explorer_exe_Process, Color.Green);
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
                    if (_isVerboseMode)
                    {
                        richTextBox1.AppendText(Resources.Restarted_Explorer_exe_Process, Color.Green);
                    }
                }));
                richTextBox1.AppendText(Resources.Swept_Thumbnail_Cache, Color.Green);
                _thumbnailCacheCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region User File History Removal
            // User File History Removal.
            if (checkBox9.Checked)
            {
                richTextBox1.AppendText(Resources.Attempting_to_remove_all_file_history_snapshots_except_latest, Color.Green);
                ScrollToOutputBottom();
                AddWaitText();

                await Task.Run(() =>
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                });
                richTextBox1.AppendText(Resources.If_File_History_Was_Enabled_All_Versions_Except_The_Latest_Were_Removed, Color.Green);
                _deletedFileHistory = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Windows.old Directory Removal
            // Windows.old Directory Removal.
            if (checkBox10.Checked)
            {
                richTextBox1.AppendText(Resources.Removing_old_versions_of_Windows, Color.Green);
                ScrollToOutputBottom();
                if (Directory.Exists("C:\\windows.old"))
                {
                    richTextBox1.AppendText(Resources.Found_Windows_Old_Directory_Cleaning, Color.Green);
                    AddWaitText();
                    await Task.Run(() =>
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                                Invoke(new Action(() =>
                                {
                                    ScrollToOutputBottom();
                                }));
                                break;
                            }
                        }
                    });
                    richTextBox1.AppendText(Resources.Swept_Windows_Old_Directory, Color.Green);
                    _windowsOldCleaned = true;
                }
                else
                {
                    richTextBox1.AppendText(Resources.No_Windows_Old_Directory_Found, Color.Green);
                    _windowsOldCleaned = false;
                }
                ScrollToOutputBottom();
            }
            #endregion
            #region Windows Defender Log Files Removal
            // Windows Defender Log Files Removal.
            if (checkBox11.Checked)
            {
                richTextBox1.AppendText(Resources.Sweeping_Windows_Defender_Log_Files, Color.Green);
                ScrollToOutputBottom();

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
                await Task.Run(() =>
                {
                    foreach (var directory in directoryPath)
                    {
                        var dir = new DirectoryInfo(directory);
                        foreach (var f in dir.GetFiles())
                        {
                            if (f.Name.Contains(".log"))
                                try
                                {
                                    File.Delete(f.FullName);
                                    if (_isVerboseMode)
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText(Resources.Removed_ + f.Name + "\n", Color.Green);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("[o]", Color.Green);
                                        }));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (_isVerboseMode)
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText(ex.Message + Resources.Skipping + "\n", Color.Red);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("[x]" + f.Name + "\n", Color.Red);
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
                            if (_isVerboseMode)
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText(Resources.Removed_ + logFileDirectory + "\n", Color.Green);
                                }));
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("[o]", Color.Green);
                                }));
                            }
                        }
                        catch (Exception ex)
                        {
                            if (_isVerboseMode)
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText(ex.Message + Resources.Skipping_ + "\n", Color.Red);

                                }));
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("[x]", Color.Red);
                                }));
                            }
                        }
                    }
                    else
                    {
                        if (_isVerboseMode)
                        {
                            Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(Resources.Directory_already_removed + logFileDirectory + "\n", Color.Red);
                            }));
                        }
                    }
                }
                richTextBox1.AppendText("\n" + "Removed Windows Defender Logs!" + "\n" + "\n", Color.Green);
                _windowsDefenderLogsCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Microsoft Office Cache Removal
            // Microsoft Office Cache Removal.
            if (checkBox12.Checked)
            {
                richTextBox1.AppendText(Resources.Sweeping_MSO_cache, Color.Green);
                ScrollToOutputBottom();
                if (Directory.Exists(_msoCacheDir))
                {
                    Directory.Delete(_msoCacheDir, true);
                    richTextBox1.AppendText(Resources.Swept_MSO_Cache, Color.Green);
                }
                else
                {
                    richTextBox1.AppendText(Resources.No_MSO_Cache_Directory_Found_Skipping, Color.Green);
                }

                _msoCacheCleared = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Microsoft Edge Cache Removal
            // Edge Cache Removal
            if (checkBox13.Checked)
            {
                richTextBox1.AppendText(Resources.Sweeping_Edge_Cache, Color.Green);
                ScrollToOutputBottom();
                if (_isVerboseMode)
                {
                    richTextBox1.AppendText(Resources.Ending_any_Edge_processes, Color.Green);
                    ScrollToOutputBottom();
                }
                await Task.Run(() =>
                {
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                    Invoke(new Action(() =>
                    {
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(Resources.Stopped_Edge_Processes, Color.Green);
                            ScrollToOutputBottom();
                        }
                    }));
                    foreach (var edgeDirectory in _edgeCacheDirectories)
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
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText(Resources.Removed__ + edgeDirectory + "." + "\n", Color.Green);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("[x]", Color.Green);
                                        }));
                                    }
                                }
                            }
                            catch (IOException ex)
                            {
                                if (_isVerboseMode)
                                {
                                    Invoke(new Action(() =>
                                    {
                                        richTextBox1.AppendText(Resources.Skipping__ + edgeDirectory + ". " + ex.Message + "\n", Color.Green);
                                        ScrollToOutputBottom();
                                    }));
                                }
                                else
                                {
                                    Invoke(new Action(() =>
                                    {
                                        richTextBox1.AppendText("[x]", Color.Red);
                                    }));
                                }
                            }
                        }
                        else
                        {
                            if (_isVerboseMode) 
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText(Resources.Directory_already_removed + edgeDirectory + "." + "\n", Color.Green);
                                    ScrollToOutputBottom();
                                }));
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText("[x]", Color.Red);
                                    ScrollToOutputBottom();
                                }));
                            }
                        }
                    }
                });

                richTextBox1.AppendText(Resources.Swept_Edge_Cache, Color.Green);
                _sweptEdgeCache = true;
                ScrollToOutputBottom();
            }
            #endregion
            #region Chrome Cache Removal
            // Chrome Cache Removal
            if (checkBox14.Checked)
            {
                richTextBox1.AppendText(Resources.Sweeping_Chrome_Cache, Color.Green);
                ScrollToOutputBottom();
                if (_showOperationWindows)
                {
                    richTextBox1.AppendText(Resources.Ending_any_Chrome_processes, Color.Green);
                    ScrollToOutputBottom();
                }
                await Task.Run(() =>
                {
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
                            Invoke(new Action(() =>
                            {
                                ScrollToOutputBottom();
                            }));
                            break;
                        }
                    }
                    Invoke(new Action(() =>
                    {
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText(Resources.Stopped_Chrome_Processes, Color.Green);
                            ScrollToOutputBottom();
                        }
                    }));
                    foreach (var chromeDirectory in _chromeCacheDirectories)
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
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText(Resources.Removed__ + chromeDirectory + "." + "\n", Color.Green);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("[x]", Color.Green);
                                        }));
                                    }
                                }
                            }
                            catch (SystemException ex)
                            {
                                if (ex.GetType() == typeof(IOException))
                                {
                                    if (_isVerboseMode)
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText(Resources.Skipping__ + chromeDirectory + ". " + ex.Message + "\n", Color.Red);
                                            ScrollToOutputBottom();
                                        }));
                                    }
                                    else
                                    {
                                        Invoke(new Action(() =>
                                        {
                                            richTextBox1.AppendText("[x]", Color.Red);
                                        }));
                                    }
                                }
                                else if (ex.GetType() == typeof(UnauthorizedAccessException))
                                {
                                    if (_isVerboseMode)
                                    {
                                        richTextBox1.AppendText(Resources.Skipping__ + chromeDirectory + ". " + ex.Message + "\n", Color.Red);
                                        ScrollToOutputBottom();
                                    }
                                    else
                                    {
                                        richTextBox1.AppendText("x", Color.Red);
                                    }
                                }
                                else
                                {
                                    if (_isVerboseMode)
                                    {
                                        richTextBox1.AppendText(Resources.Skipping__ + chromeDirectory + ". " + ex.Message + "\n", Color.Red);
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
                            if (_isVerboseMode)
                            {
                                Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText(Resources.Directory_already_removed + chromeDirectory + "." + "\n", Color.Green);
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

                _sweptChromeCache = true;
                richTextBox1.AppendText(Resources.Swept_Chrome_Cache, Color.Green);
                ScrollToOutputBottom();
            }
            #endregion
            #region Windows Installer Cache Removal
            // Windows Installer Cache Removal
            if (checkBox15.Checked)
            {
                richTextBox1.AppendText(Resources.Removing_Windows_Installer_Cache, Color.Green);
                ScrollToOutputBottom();
                if (Directory.Exists(_windowsDirectory + "\\Installer\\$PatchCache$\\Managed"))
                {
                    richTextBox1.AppendText(Resources.Found_Windows_Installer_Cache_Cleaning, Color.Green);
                    AddWaitText();
                    await Task.Run(() =>
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
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
                                Invoke(new Action(() =>
                                {
                                    ScrollToOutputBottom();
                                }));
                                break;
                            }
                        }
                    });
                    richTextBox1.AppendText(Resources.Swept_Windows_Installer_Cache, Color.Green);
                }
                ScrollToOutputBottom();
                _windowsInstallerCacheCleared = true;
            }
            #endregion
            #region Windows Update Logs Removal
            // Windows Update Logs Removal
            if (checkBox18.Checked)
            {
                richTextBox1.AppendText(Resources.Sweeping_Windows_Update_Logs, Color.Green);
                ScrollToOutputBottom();
                try
                {
                    if (Directory.Exists(_windowsUpdateLogDir))
                    {
                        var di = new DirectoryInfo(_tempDirectory);

                        foreach (var file in di.GetFiles())
                        {
                            try
                            {
                                file.Delete();
                                // If file no longer exists, append success to rich text box.
                                if (!File.Exists(file.Name))
                                {
                                    if (_isVerboseMode)
                                    {
                                        richTextBox1.AppendText(Resources.Deleted + file.Name + "\n", Color.Green);
                                    }
                                    else
                                    {
                                        richTextBox1.AppendText("[o]", Color.Green);
                                    }
                                }
                                else
                                {
                                    richTextBox1.AppendText("[x]", Color.Red);
                                }
                                ScrollToOutputBottom();
                            }
                            catch (Exception ex)
                            {
                                if (ex.GetType() == typeof(IOException))
                                {
                                    if (_isVerboseMode)
                                    {
                                        richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                                    }
                                    else
                                    {
                                        richTextBox1.AppendText("[x]", Color.Red);
                                    }
                                }
                                else if (ex.GetType() == typeof(UnauthorizedAccessException))
                                {
                                    if (_isVerboseMode)
                                    {
                                        richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                                    }
                                    else
                                    {
                                        richTextBox1.AppendText("[x]", Color.Red);
                                    }
                                }
                            }
                        }
                        richTextBox1.AppendText(Resources.Swept_Windows_Update_Logs, Color.Green);
                    }
                    else
                    {
                        richTextBox1.AppendText(Resources.No_Windows_Update_Logs_Directory_Found_Skipping, Color.Green);
                    }

                    ScrollToOutputBottom();
                    _windowsUpdateLogsCleared = true;
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IOException))
                    {
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                        }
                    }
                    else if (ex.GetType() == typeof(UnauthorizedAccessException))
                    {
                        if (_isVerboseMode)
                        {
                            richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                        }
                    }
                }
            }
            #endregion


            #region Calculate Space Saved
            richTextBox1.AppendText(Resources.Recovery_Results, Color.Green);
            richTextBox1.AppendText("\n" + "----------------------------------------------------------------------------------------------------------------------------------------", Color.Green);

            if (_tempFilesWereRemoved)
            {
                _tempFilesWereRemoved =  false;
                // Get new Temporary Files size and output what was saved.
                _tempDirSize = Directory.GetFiles(_tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                _tempDirSizeInMegaBytes = _tempDirSize / 1024 / 1024;
                _newTempDirSize = _tempSizeBeforeDelete - _tempDirSizeInMegaBytes;
                _totalSpaceSaved += _newTempDirSize;
                if (_newTempDirSize > 0)
                {
                    richTextBox1.AppendText(Resources.Recovered + _newTempDirSize + Resources.MB_from_removing_Temporary_Files, Color.Green);
                }
                else
                {
                    richTextBox1.AppendText(Resources._1MB_Recovered_From_Temporary_Files, Color.Green);
                }
                ScrollToOutputBottom();
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
                    richTextBox1.AppendText(Resources.Recovered + _newTempSetupDirSize + Resources.MB_from_removing_Temporary_Setup_Files, Color.Green);
                }
                else
                {
                    richTextBox1.AppendText(Resources._1MB_Recovered_From_Temporary_Setup_Files, Color.Green);
                }
                ScrollToOutputBottom();
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
                        richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                    }
                }
                _tempInternetSizeInMegabytes = _tempInternetFilesDirSize / 1024 / 1024;
                _newTempInternetFilesDirSize = _tempInternetFilesBeforeDelete - _tempInternetSizeInMegabytes;
                _totalSpaceSaved += _newTempInternetFilesDirSize;
                if (_newTempInternetFilesDirSize > 0)
                {
                    richTextBox1.AppendText(Resources.Recovered + _newTempInternetFilesDirSize + Resources.MB_from_removing_Temporary_Internet_Files, Color.Green);
                }
                else
                {
                    richTextBox1.AppendText(Resources._1MB_Recovered_From_Temporary_Internet_Files, Color.Green);
                }
                ScrollToOutputBottom();
            }

            if (_eventLogsCleared)
            {
                // Mark the sweeping of Event Logs as completed to allow re-sweeping.
                _eventLogsCleared = false;
                richTextBox1.AppendText(Resources.Event_Logs_Were_Cleared, Color.Green);
                ScrollToOutputBottom();
            }

            if (_isRecycleBinEmpty)
            {
                _isRecycleBinEmpty = false;
                richTextBox1.AppendText(Resources.Recycle_Bin_Was_Emptied, Color.Green);
                ScrollToOutputBottom();
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
                    richTextBox1.AppendText(Resources.Recovered + _newWindowsErrorReportsDirSize + Resources.MB_from_removing_Windows_Error_Reports_, Color.Green);
                }
                else
                {
                    richTextBox1.AppendText(Resources._1MB_From_Removing_Windows_Error_Reports_, Color.Green);
                }
                ScrollToOutputBottom();
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
                    richTextBox1.AppendText(Resources.Recovered + _newDeliveryOptimizationSize + Resources.MB_from_removing_Windows_Delivery_Optimization_Files, Color.Green);
                }
                else
                {
                    richTextBox1.AppendText(Resources._1MB_Recovered_From_Windows_Delivery_Optimization_Files, Color.Green);
                }
                ScrollToOutputBottom();
            }

            if (_thumbnailCacheCleared)
            {
                _thumbnailCacheCleared = false;
                richTextBox1.AppendText(Resources.Thumbnail_Cache_Was_Cleared, Color.Green);
                ScrollToOutputBottom();
            }

            if (_deletedFileHistory)
            {
                _deletedFileHistory = false;
                richTextBox1.AppendText(Resources.Removed_File_History_Older_Than_Latest_Snapshot, Color.Green);
                ScrollToOutputBottom();
            }

            if (_windowsOldCleaned)
            {
                _windowsOldCleaned = false;
                richTextBox1.AppendText(Resources.Old_Versions_Of_Windows_Were_Removed, Color.Green);
                ScrollToOutputBottom();
            }

            if (_windowsDefenderLogsCleared)
            {
                _windowsDefenderLogsCleared = false;
                richTextBox1.AppendText(Resources.Windows_Defender_Logs_Were_Removed, Color.Green);
                ScrollToOutputBottom();
            }
            if (_sweptChromeCache)
            {
                _sweptChromeCache = false;
                richTextBox1.AppendText(Resources.Chome_Cache_Was_Cleared, Color.Green);
                ScrollToOutputBottom();
            }

            if (_sweptEdgeCache)
            {
                _sweptEdgeCache = false;
                richTextBox1.AppendText(Resources.Edge_Cache_Was_Cleared, Color.Green);
                ScrollToOutputBottom();
            }

            if (_msoCacheCleared)
            {
                _msoCacheCleared = false;
                // Get size of MSOCache.
                try
                {
                    if (!Directory.Exists(_msoCacheDir))
                    {
                        richTextBox1.AppendText(Resources.Recovered + _msoCacheDirSize + Resources.MB_from_removing_MSO_Cache, Color.Green);
                        _totalSpaceSaved += _msoCacheDirSize; 
                    }
                    else if (Directory.Exists(_msoCacheDir))
                    {
                        var oldMsoCacheSize = _msoCacheDirSize;
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
                                richTextBox1.AppendText(Resources.Recovered + _newMsoCacheDirSize + Resources.MB_from_removing_MSO_Cache, Color.Green);
                            }
                            else if (ex.GetType() == typeof(UnauthorizedAccessException))
                            {
                                richTextBox1.AppendText(Resources.Recovered + _newMsoCacheDirSize + Resources.MB_from_removing_MSO_Cache, Color.Green);
                            }
                        }
                    }
                    ScrollToOutputBottom();
                }
                catch (Exception ex)
                {
                    if (_isVerboseMode)
                    {
                        richTextBox1.AppendText("\n" + ex.Message, Color.Red);
                    }
                }
            }

            if (_windowsInstallerCacheCleared)
            {
                _windowsInstallerCacheCleared = false;
                // Get size of Windows Installer Cache.
                if (Directory.Exists(_windowsDirectory + "\\Installer\\$PatchCache$"))
                {
                    var oldWindowsInstallerCacheDirSize = _windowsInstallerCacheDirSize;
                    if (Directory.Exists(_windowsInstallerCacheDir))
                    {
                        _windowsInstallerCacheDirSize = Directory.GetFiles(_windowsInstallerCacheDir + "\\Managed", "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                        _newWindowsInstallerCacheDirSize = oldWindowsInstallerCacheDirSize - _windowsInstallerCacheDirSize;
                    }
                    else
                    {
                        richTextBox1.AppendText(Resources.Recovered + oldWindowsInstallerCacheDirSize + Resources.MB_from_removing_Windows_Installer_Cache, Color.Green);
                    }
                    _totalSpaceSaved += _newWindowsInstallerCacheDirSize;
                }
                ScrollToOutputBottom();
            }

            if (_windowsUpdateLogsCleared)
            {
                _windowsUpdateLogsCleared = false;
                // Get size of Windows Update Logs.
                if (!Directory.Exists(_windowsUpdateLogDir))
                {
                    richTextBox1.AppendText(Resources.Recovered + _windowsUpdateLogDirSize + Resources.MB_from_removing_Windows_Update_Logs, Color.Green);
                    _totalSpaceSaved += _windowsUpdateLogDirSize;
                }
                else
                {
                    var oldWindowsUpdateLogDirSize = _windowsUpdateLogDirSize;
                    _windowsUpdateLogDirSize = 0;
                    _windowsUpdateLogDirSize = Directory.GetFiles(_windowsUpdateLogDir, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)) / 1024 / 1024;
                    _newWindowsUpdateLogDirSize = oldWindowsUpdateLogDirSize - _windowsUpdateLogDirSize;
                    _totalSpaceSaved += _newWindowsUpdateLogDirSize;
                    if (_newWindowsUpdateLogDirSize > 0)
                    {
                        richTextBox1.AppendText(Resources.Recovered + _newWindowsUpdateLogDirSize + Resources.MB_from_removing_Windows_Update_Logs, Color.Green);
                    }
                    else
                    {
                        richTextBox1.AppendText(Resources._1MB_Recovered_From_Windows_Update_Logs, Color.Green);
                    }
                }
                ScrollToOutputBottom();
            }
            
            // Output the total space saved from the entire operation and other important completed actions.
            if (_totalSpaceSaved > 0)
            {
                richTextBox1.AppendText(Resources.Total_Space_Recovered + _totalSpaceSaved + Resources.MB + "\n", Color.Green);
            }
            else
            {
                richTextBox1.AppendText(Resources.Total_Space_Recovered__1MB, Color.Green);
            }
            ScrollToOutputBottom();
            _totalSpaceSaved = 0;
            LockCleaning(false);
            CanCleanStatus();
            SaveUserSelectedChecks();
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
                _checkedArrayBool[indexPosition] = true;
            }
            else
            {
                _checkedArrayBool[indexPosition] = false;
            }
            CanCleanStatus();
        }

        // If any box is checked, allow cleaning. Otherwise, disable cleaning.
        private void CanCleanStatus()
        {
            if (_checkedArrayBool.Contains(true))
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
                foreach (var box in _checkedArray)
                {
                    box.Enabled = false;
                }

                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                foreach (var box in _checkedArray)
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
            foreach (CheckBox box in _checkedArray)
            {
                box.Checked = false;
            }
        }

        private void PlaceAllChecks()
        {
            foreach (CheckBox box in _checkedArray)
            {
                box.Checked = true;
            }
        }

        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_octoClient == null)
            {
                _octoClient = new Octo.GitHubClient(new Octo.ProductHeaderValue("CleanSweep2"));
                CheckForUpdates();
            }
            else if (_octoClient != null)
            {
                CheckForUpdates();
            }
            else
            {
                MessageBox.Show(Resources.Couldn_t_connect_to_get_updates__Your_connection_might_be_down__you_re_checking_too_often__or_Github_is_down__Try_checking_again_later,
                    Resources.Couldn_t_Connect, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CheckForUpdates()
        {
            var releases = await _octoClient.Repository.Release.GetAll("thomasloupe", "CleanSweep2").ConfigureAwait(false);
            var latest = releases[0];
            if (CurrentVersion == latest.TagName)
            {
                MessageBox.Show(Resources.You_have_the_latest_version, CurrentVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (CurrentVersion != latest.TagName)
            {
                var result = MessageBox.Show(Resources.A_new_version_of_CleanSweep_is_available + latest.TagName + "\n"
                    + Resources.Would_you_like_to_visit_the_download_page_, Resources.Update_Available, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
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
            SaveUserSelectedChecks();
            Application.Exit();
        }

        private void DonateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(Resources.CleanSweep_will_always_be_free, Resources.CleanSweep_is_free__but_beer_is_not, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://paypal.me/thomasloupe");
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CleanSweep " + CurrentVersion + ".\n" +
            Resources.Created_by_Thomas_Loupe + "\n" +
            Resources.Github__https___github_com_thomasloupe + "\n" +
            Resources.Twitter__https___twitter_com_acid_rain + "\n" +
            Resources.Website__https___thomasloupe_com, Resources.About, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void VerboseModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (verboseModeToolStripMenuItem.Checked)
            {
                Settings.Default.Verbosity = true;
                _isVerboseMode = true;
            }
            else
            {
                Settings.Default.Verbosity = false;
                _isVerboseMode = false;
            }
            Settings.Default.Save();
        }

        private void ShowOperationWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showOperationWindowsToolStripMenuItem.Checked)
            {
                Settings.Default.OperationWindows = true;
                _showOperationWindows = true;
            }
            else
            {
                Settings.Default.OperationWindows = false;
                _showOperationWindows = false;
            }
            Settings.Default.Save();
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

        private void SaveUserSelectedChecks()
        {
            // Capture which options are selected
            if (checkBox1.Checked)
            {
                Settings.Default.Box1Checked = true;
            }
            else
            {
                Settings.Default.Box1Checked = false;
            }
            if (checkBox2.Checked)
            {
                Settings.Default.Box2Checked = true;
            }
            else
            {
                Settings.Default.Box2Checked = false;
            }
            if (checkBox3.Checked)
            {
                Settings.Default.Box3Checked = true;
            }
            else
            {
                Settings.Default.Box3Checked = false;
            }
            if (checkBox4.Checked)
            {
                Settings.Default.Box4Checked = true;
            }
            else
            {
                Settings.Default.Box4Checked = false;
            }
            if (checkBox5.Checked)
            {
                Settings.Default.Box5Checked = true;
            }
            else
            {
                Settings.Default.Box5Checked = false;
            }
            if (checkBox6.Checked)
            {
                Settings.Default.Box6Checked = true;
            }
            else
            {
                Settings.Default.Box6Checked = false;
            }
            if (checkBox7.Checked)
            {
                Settings.Default.Box7Checked = true;
            }
            else
            {
                Settings.Default.Box7Checked = false;
            }
            if (checkBox8.Checked)
            {
                Settings.Default.Box8Checked = true;
            }
            else
            {
                Settings.Default.Box8Checked = false;
            }
            if (checkBox9.Checked)
            {
                Settings.Default.Box9Checked = true;
            }
            else
            {
                Settings.Default.Box9Checked = false;
            }
            if (checkBox10.Checked)
            {
                Settings.Default.Box10Checked = true;
            }
            else
            {
                Settings.Default.Box10Checked = false;
            }
            if (checkBox11.Checked)
            {
                Settings.Default.Box11Checked = true;
            }
            else
            {
                Settings.Default.Box11Checked = false;
            }
            if (checkBox12.Checked)
            {
                Settings.Default.Box12Checked = true;
            }
            else
            {
                Settings.Default.Box12Checked = false;
            }
            if (checkBox13.Checked)
            {
                Settings.Default.Box13Checked = true;
            }
            else
            {
                Settings.Default.Box13Checked = false;
            }
            if (checkBox14.Checked)
            {
                Settings.Default.Box14Checked = true;
            }
            else
            {
                Settings.Default.Box14Checked = false;
            }
            if (checkBox15.Checked)
            {
                Settings.Default.Box15Checked = true;
            }
            else
            {
                Settings.Default.Box15Checked = false;
            }
            if (checkBox18.Checked)
            {
                Settings.Default.Box18Checked = true;
            }
            else
            {
                Settings.Default.Box18Checked = false;
            }
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void LanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void EnglishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var languageBox in _languageBoxes)
            {
                languageBox.Checked = false;
            }
            englishToolStripMenuItem.Checked = true;
            var changeLanguage = new ChangeLanguage();
            changeLanguage.UpdateConfig("language", "en");
            Application.Restart();
        }

        private void FrenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var languageBox in _languageBoxes)
            {
                languageBox.Checked = false;
            }
            frenchToolStripMenuItem.Checked = true;
            var changeLanguage = new ChangeLanguage();
            changeLanguage.UpdateConfig("language", "fr");
            Application.Restart();
        }

        private void GermanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var languageBox in _languageBoxes)
            {
                languageBox.Checked = false;
            }
            germanToolStripMenuItem.Checked = true;
            var changeLanguage = new ChangeLanguage();
            changeLanguage.UpdateConfig("language", "ger");
            Application.Restart();
        }

        private void ItalianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var languageBox in _languageBoxes)
            {
                languageBox.Checked = false;
            }
            italianToolStripMenuItem.Checked = true;
            var changeLanguage = new ChangeLanguage();
            changeLanguage.UpdateConfig("language", "it");
            Application.Restart();
        }

        private void SpanishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var languageBox in _languageBoxes)
            {
                languageBox.Checked = false;
            }

            spanishToolStripMenuItem.Checked = true;
            var changeLanguage = new ChangeLanguage();
            changeLanguage.UpdateConfig("language", "es");
            Application.Restart();
        }

        private void RussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var languageBox in _languageBoxes)
            {
                languageBox.Checked = false;
            }
            russianToolStripMenuItem.Checked = true;
            var changeLanguage = new ChangeLanguage();
            changeLanguage.UpdateConfig("language", "ru");
            Application.Restart();
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

public class ChangeLanguage
{
    public void UpdateConfig(string key, string value)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

        foreach (XmlElement xmlElement in xmlDoc.DocumentElement)
        {
            if (xmlElement.Name.Equals("appSettings"))
            {
                foreach (XmlNode xNode in xmlElement.ChildNodes)
                {
                    if (xNode.Attributes[0].Value.Equals(key))
                    {
                        xNode.Attributes[1].Value = value;
                    }
                }
            }
        }
        ConfigurationManager.RefreshSection("appSettings");
        xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
    }
}
