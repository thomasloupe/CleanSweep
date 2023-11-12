using CleanSweep2.Properties;
using Octo = Octokit;
using System;
using System.Linq;
using System.Windows.Forms;
using CleanSweep2.Classes;
using System.Text;
using CleanSweep2.Interfaces;
using System.Collections.Generic;

namespace CleanSweep2
{
    public partial class Form1 : Form
    {
        private Octo.GitHubClient _octoClient;
        private bool[] _checkedArrayBool;
        private CheckBox[] _checkedArray;
        public static bool IsVerboseMode;
        private bool _showOperationWindows;

        public Form1()
        {
            InitializeComponent();
            SetWindowSizeAndLocation();
            SetVerbosity();
            SetOperationWindows();
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
            verboseModeToolStripMenuItem.Checked = Settings.Default.Verbosity;
            IsVerboseMode = Settings.Default.Verbosity;
        }

        private void SetOperationWindows()
        {
            showOperationWindowsToolStripMenuItem.Checked = Settings.Default.OperationWindows;
            _showOperationWindows = Settings.Default.OperationWindows;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            _checkedArray = new[]
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

            _checkedArrayBool = new[]
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

            RestoreSavedChecks();
            GetTotalReclaimableSpace();
        }

        private string GetTotalReclaimableSpace()
        {
            StringBuilder report = new StringBuilder();

            var cleaners = new List<ICleaner>
            {
                new WindowsUpdateLogsCleaner("C:\\Windows\\Logs", false),
                new WindowsOldDirectoryCleaner(false),
            };

            foreach (var cleaner in cleaners)
            {
                var (fileType, spaceInMB) = cleaner.GetReclaimableSpace();
                report.AppendLine($"{fileType}: {spaceInMB} MB");
            }

            return report.ToString();
        }

        // Sweep button function.
        private void Button1_Click_1(object sender, EventArgs e)
        {
            // Lock cleaning until the entire operation is done.
            LockCleaning(true);
        }

        private void ScrollToOutputBottom()
        {
            // Scroll to the bottom of the output window.
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }


        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        // Checkbox Checked Functions
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
            // Set enabled state for all checkboxes and buttons based on the isEnabled parameter.
            foreach (var box in _checkedArray)
            {
                box.Enabled = isEnabled;
            }

            button1.Enabled = isEnabled;
            button2.Enabled = isEnabled;
            button3.Enabled = isEnabled;
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
            // If _octoClient is null, instantiate it.
            if (_octoClient == null)
            {
                _octoClient = new Octo.GitHubClient(new Octo.ProductHeaderValue("CleanSweep2"));
            }

            // Proceed to check for updates.
            CheckForUpdates();
        }

        private async void CheckForUpdates()
        {
            try
            {
                var version = CleanSweepVersion.Version;
                var releases = await _octoClient.Repository.Release.GetAll("thomasloupe", "CleanSweep2").ConfigureAwait(false);
                var latest = releases[0];

                if (version == latest.TagName)
                {
                    MessageBox.Show($"{Resources.You_have_the_latest_version} {version}",
                                    "Version Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string message = $"{Resources.A_new_version_of_CleanSweep_is_available} {latest.TagName}\n{Resources.Would_you_like_to_visit_the_download_page_}";
                    var result = MessageBox.Show(message, "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                    if (result == DialogResult.Yes)
                    {
                        string url = $"https://github.com/thomasloupe/CleanSweep2/releases/tag/{latest.TagName}";
                        System.Diagnostics.Process.Start(url);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking for updates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            var version = CleanSweepVersion.Version;
            MessageBox.Show("CleanSweep " + version + ".\n" +
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
                IsVerboseMode = true;
            }
            else
            {
                Settings.Default.Verbosity = false;
                IsVerboseMode = false;
            }
            Settings.Default.Save();
        }

        private void ShowOperationWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Directly assign the checked state to the settings and the variable.
            Settings.Default.OperationWindows = showOperationWindowsToolStripMenuItem.Checked;
            _showOperationWindows = showOperationWindowsToolStripMenuItem.Checked;

            // Save the settings.
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
            // Directly assign the checked status of each checkbox to the corresponding setting
            Settings.Default.Box1Checked = checkBox1.Checked;
            Settings.Default.Box2Checked = checkBox2.Checked;
            Settings.Default.Box3Checked = checkBox3.Checked;
            Settings.Default.Box4Checked = checkBox4.Checked;
            Settings.Default.Box5Checked = checkBox5.Checked;
            Settings.Default.Box6Checked = checkBox6.Checked;
            Settings.Default.Box7Checked = checkBox7.Checked;
            Settings.Default.Box8Checked = checkBox8.Checked;
            Settings.Default.Box9Checked = checkBox9.Checked;
            Settings.Default.Box10Checked = checkBox10.Checked;
            Settings.Default.Box11Checked = checkBox11.Checked;
            Settings.Default.Box12Checked = checkBox12.Checked;
            Settings.Default.Box13Checked = checkBox13.Checked;
            Settings.Default.Box14Checked = checkBox14.Checked;
            Settings.Default.Box15Checked = checkBox15.Checked;
            Settings.Default.Box18Checked = checkBox18.Checked;
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
