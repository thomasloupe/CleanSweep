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
        private const string CurrentVersion = "v2.0.5";
        private octo.GitHubClient _octoClient;
        readonly string userName = Environment.UserName;
        readonly string windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        readonly string programDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        bool isVerboseMode;
        bool eventLogsCleared = false;
        bool isRecycleBinEmpty = false;
        bool windowsErrorReportsCleared = false;
        bool[] checkedArrayBool;
        CheckBox[] checkedArray;

        // Temporary Files
        string tempDirectory;
        long tempDirSize;
        long tempDirSizeInMegaBytes;
        long tempSizeBeforeDelete;

        // Temporary Setup Files
        string tempSetupDirectory;
        long tempSetupDirSize;
        long tempSetupSizeInMegabytes;
        long tempSetupFilesSizeBeforeDelete;

        // Temporary Internet Files
        string tempInternetFilesDirectory;
        long tempInternetFilesDirSize;
        long tempInternetSizeInMegabytes;
        long tempInternetFilesBeforeDelete;

        //Windows Error Reports
        // %ProgramData%\Microsoft\Windows\WER\ReportArchive
        string windowsErrorReportsDirectory;
        long windowsErrorReportsDirSize;
        long windowsErrorReportsDirSizeInMegabytes;
        long windowsErrorReportsDirSizeBeforeDelete;
        #endregion

        public Form1()
        {
            InitializeComponent();
            SetWindowSizeAndLocation();
            SetVerbosity();
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
            checkedArray = new CheckBox[6] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6};
            checkedArrayBool = new bool[6] { checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked, checkBox5.Checked, checkBox6.Checked };

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
            catch (Exception ex)
            {
                // Skip files we don't have privileges to.
                if (isVerboseMode)
                {
                    richTextBox1.AppendText(ex.Message + "Skipping..." + "\n", Color.Red);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
                ScrollToOutputBottom();
            }
            tempInternetSizeInMegabytes = tempInternetFilesDirSize / 1024 / 1024;

            // Get size of Windows Error Reports
            windowsErrorReportsDirectory = programDataDirectory + "\\Microsoft\\Windows\\WER\\ReportArchive";
            windowsErrorReportsDirSize = Directory.GetFiles(windowsErrorReportsDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            windowsErrorReportsDirSizeInMegabytes = windowsErrorReportsDirSize / 1024 / 1024;

            // Show potential reclamation, then each individual category.
            richTextBox1.AppendText("Space to reclaim: " + 
                (tempDirSizeInMegaBytes + tempSetupSizeInMegabytes + tempInternetSizeInMegabytes + windowsErrorReportsDirSizeInMegabytes) + 
                "MB" + "\n" + "\n" + "Categorical breakdown:" + "\n" + 
                "----------------------------------------------------------------------------------------------------------------------------------------" + "\n");

            // List out total space reclamation per category.
            richTextBox1.AppendText("Temp Directory size: " + tempDirSizeInMegaBytes + "MB" + "\n");
            richTextBox1.AppendText("Temporary Setup Files directory size: " + tempSetupSizeInMegabytes + "MB" + "\n");
            richTextBox1.AppendText("Temporary Internet Files directory size: " + tempInternetSizeInMegabytes + "MB" + "\n");
            richTextBox1.AppendText("Windows Error Reports size: " + windowsErrorReportsDirSizeInMegabytes + "MB" + "\n" + "\n");

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
                    catch (Exception)
                    {
                        // Skip all failed files.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText(file.Name + " appears to be in use or locked. Skipping..." + "\n", Color.Red);
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                        ScrollToOutputBottom();
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
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                            ScrollToOutputBottom();
                        }
                    }
                    catch (Exception)
                    {
                        // Skip all failed directories.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText(dir.Name + " appears to be in use or locked. Skipping..." + "\n", Color.Red);
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                        ScrollToOutputBottom();
                    }
                }
                richTextBox1.AppendText("\n" + "\n");
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
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                            ScrollToOutputBottom();
                        }
                    }
                    catch (Exception)
                    {
                        // Skip all failed files.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText(file.Name + " appears to be in use or locked. Skipping..." + "\n", Color.Red);
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                        ScrollToOutputBottom();
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
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                            ScrollToOutputBottom();
                        }
                    }
                    catch (Exception)
                    {
                        // Skip all failed directories.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText(dir.Name + " appears to be in use or locked. Skipping..." + "\n", Color.Red);
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                        ScrollToOutputBottom();
                    }
                }
                richTextBox1.AppendText("\n" + "\n");
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
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                            ScrollToOutputBottom();
                        }
                    }
                    catch (Exception)
                    {
                        // Skip all failed files.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText(file.Name + " appears to be in use or locked. Skipping..." + "\n", Color.Red);
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                        ScrollToOutputBottom();
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
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                            ScrollToOutputBottom();
                        }
                    }
                    catch (Exception)
                    {
                        // Skip all failed directories.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText(dir.Name + " appears to be in use or locked. Skipping..." + "\n", Color.Red);
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                        ScrollToOutputBottom();
                    }
                }
                richTextBox1.AppendText("\n" + "\n");
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
                    if (isVerboseMode)
                    {
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    }
                    else
                    {
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    }
                    startInfo.FileName = "cmd.exe";
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
                
                eventLogsCleared = true;
                richTextBox1.AppendText("\n" + "Swept Event Viewer Logs!" + "\n" + "\n", Color.Green);
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
                    if (isVerboseMode)
                    {
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    }
                    else
                    {
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    }
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C rd /s /q %systemdrive%\\$Recycle.bin";
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
            }
            #endregion
            #region Windows Error Reports Removal
            // Temporary Setup Files removal
            if (checkBox6.Checked)
            {
                windowsErrorReportsDirSizeBeforeDelete = windowsErrorReportsDirSizeInMegabytes;

                richTextBox1.AppendText("Sweeping Windows Error Reports" + "\n", Color.Green);
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
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                            ScrollToOutputBottom();
                        }
                    }
                    catch (Exception)
                    {
                        // Skip all failed files.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText(file.Name + " appears to be in use or locked. Skipping..." + "\n", Color.Red);
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                        ScrollToOutputBottom();
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
                            }
                            else
                            {
                                richTextBox1.AppendText("o", Color.Green);
                            }
                            ScrollToOutputBottom();
                        }
                    }
                    catch (Exception)
                    {
                        // Skip all failed directories.
                        if (isVerboseMode)
                        {
                            richTextBox1.AppendText(dir.Name + " appears to be in use or locked. Skipping..." + "\n", Color.Red);
                        }
                        else
                        {
                            richTextBox1.AppendText("x", Color.Red);
                        }
                        ScrollToOutputBottom();
                    }
                }
                richTextBox1.AppendText("Swept Windows Error Reports!" + "\n", Color.Green);
                windowsErrorReportsCleared = true;
            }
                #endregion

                #region Calculate Space Saved
                richTextBox1.AppendText("\n" + "\n" + "Recovery results:", Color.Green);
            richTextBox1.AppendText("\n" + "----------------------------------------------------------------------------------------------------------------------------------------", Color.Green);

            // Get new Temporary Files size and output what was saved.
            tempDirSize = Directory.GetFiles(tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            tempDirSizeInMegaBytes = tempDirSize / 1024 / 1024;
            long newTempDirSize = tempSizeBeforeDelete - tempDirSizeInMegaBytes;
            if (newTempDirSize > 0)
            {
                richTextBox1.AppendText("\n" + "Recovered " + newTempDirSize + "MB from removing Temporary Files." + "\n", Color.Green);
            }
            else
            {
                richTextBox1.AppendText("<1MB recovered from Temporary Files..." + "\n", Color.Green);
            }
            ScrollToOutputBottom();

            // Get new Temporary Setup Files size and output what was saved.
            tempSetupDirSize = Directory.GetFiles(tempSetupDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            tempSetupSizeInMegabytes = tempSetupDirSize / 1024 / 1024;
            long newTempSetupDirSize = tempSetupFilesSizeBeforeDelete - tempSetupSizeInMegabytes;
            if (newTempSetupDirSize > 0)
            {
                richTextBox1.AppendText("Recovered " + newTempSetupDirSize + "MB from removing Temporary Setup Files." + "\n", Color.Green);
            }
            else
            {
                richTextBox1.AppendText("<1MB recovered from Temporary Setup Files..." + "\n", Color.Green);
            }
            ScrollToOutputBottom();

            // Get new Temporary Internet Files size and output what was saved.
            try
            {
                tempInternetFilesDirSize = Directory.GetFiles(tempInternetFilesDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            }
            catch (Exception ex)
            {
                // Skip files we don't have privileges to.
                if (isVerboseMode)
                {
                    richTextBox1.AppendText(ex.Message + "Skipping..." + "\n", Color.Red);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
                ScrollToOutputBottom();
            }
            tempInternetSizeInMegabytes = tempInternetFilesDirSize / 1024 / 1024;
            long newTempInternetFilesDirSize = tempInternetFilesBeforeDelete - tempInternetSizeInMegabytes;
            if (newTempInternetFilesDirSize > 0)
            {
                richTextBox1.AppendText("Recovered " + newTempInternetFilesDirSize + "MB from removing Temporary Internet Files." + "\n", Color.Green);
            }
            else
            {
                richTextBox1.AppendText("<1MB recovered from Temporary Internet Files..." + "\n", Color.Green);
            }
            ScrollToOutputBottom();

            if (eventLogsCleared)
            {
                // Mark the sweeping of Event Logs as completed to allow re-sweeping.
                eventLogsCleared = false;
                richTextBox1.AppendText("Event Logs were cleared." + "\n", Color.Green);
            }
            ScrollToOutputBottom();

            if (isRecycleBinEmpty)
            {
                isRecycleBinEmpty = false;
                richTextBox1.AppendText("Recycle Bin was emptied." + "\n", Color.Green);
            }
            ScrollToOutputBottom();

            if (windowsErrorReportsCleared)
            {
                windowsErrorReportsCleared = false;
                richTextBox1.AppendText("Windows Error Reports were cleared." + "\n", Color.Green);
            }
            ScrollToOutputBottom();

            // Get new Windows Error Reports Directory Size and output what was saved.
            windowsErrorReportsDirSize = Directory.GetFiles(windowsErrorReportsDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            windowsErrorReportsDirSizeInMegabytes = windowsErrorReportsDirSize / 1024 / 1024;
            long newWindowsErrorReportsDirSize = windowsErrorReportsDirSizeBeforeDelete - windowsErrorReportsDirSizeInMegabytes;
            if (newWindowsErrorReportsDirSize > 0)
            {
                richTextBox1.AppendText("Recovered " + newWindowsErrorReportsDirSize + "MB from removing Windows Error Reports." + "\n", Color.Green);
            }
            else
            {
                richTextBox1.AppendText("<1MB recovered from removing Windows Error Reports..." + "\n", Color.Green);
            }
            ScrollToOutputBottom();

            // Output the total space saved from the entire operation and other important completed actions.
            long totalSpaceSaved = newTempDirSize + newTempSetupDirSize + newTempInternetFilesDirSize + newWindowsErrorReportsDirSize;
            if (totalSpaceSaved > 0)
            {
                richTextBox1.AppendText("\n" + "Total space recovered: " + totalSpaceSaved + "MB" + "\n", Color.Green);
            }
            else
            {
                richTextBox1.AppendText("\n" + "Total space recovered: <1MB" + "\n", Color.Green);
            }
            ScrollToOutputBottom();

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

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

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

        // Check each checkboxe's status and update the cleanable status immediately.
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
            foreach (bool anyIsChecked in checkedArrayBool)
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
        }

        private void LockCleaning(bool isEnabled)
        {
            if (isEnabled)
            {
                // Lock all checkboxes so we can't stop the operation.
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                checkBox6.Enabled = false;
                button1.Enabled = false;
            }
            else
            {
                // Unlock all checkboxes so we can start the operation.
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                checkBox5.Enabled = true;
                checkBox6.Enabled = true;
                button1.Enabled = true;
            }
        }

        private void RemoveAllChecks()
        {
            foreach (CheckBox box in checkedArray)
            {
                box.Checked = false;
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
            + "Would you like to open the donation page now?", "Slackord is free, but beer is not!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
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

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength == 0)
            {
                richTextBox1.SelectAll();
                richTextBox1.Copy();
            }
        }

        private void ToolStripButton2_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
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
