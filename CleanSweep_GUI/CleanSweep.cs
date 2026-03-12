using CleanSweep.Properties;
using System;
using System.Linq;
using System.Windows.Forms;
using CleanSweep.Classes;
using System.Text;
using CleanSweep.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace CleanSweep
{
    public partial class CleanSweep : Form
    {
        private bool[] _checkedArrayBool;
        private CheckBox[] _checkedArray;

        public CleanSweep()
        {
            InitializeComponent();
            SetWindowSizeAndLocation();
            SweepItButton.Enabled = false;
        }

        private async Task CheckForUpdates()
        {
            await UpdateCheck.CheckForUpdates();
        }

        private void SetWindowSizeAndLocation()
        {
            Size = new Size(550, 600);
            Location = Settings.Default.FormLocation;
            Height = Settings.Default.FormHeight;
            Width = Settings.Default.FormWidth;
            FormClosing += SaveSettingsEventHandler;
            StartPosition = FormStartPosition.Manual;
        }

        private void RestoreSavedChecks()
        {
            var settingsArray = new bool[]
            {
                Settings.Default.Box1Checked, Settings.Default.Box2Checked, Settings.Default.Box3Checked,
                Settings.Default.Box4Checked, Settings.Default.Box5Checked, Settings.Default.Box6Checked,
                Settings.Default.Box7Checked, Settings.Default.Box8Checked, Settings.Default.Box9Checked,
                Settings.Default.Box10Checked, Settings.Default.Box11Checked, Settings.Default.Box12Checked,
                Settings.Default.Box13Checked, Settings.Default.Box14Checked, Settings.Default.Box15Checked,
                Settings.Default.Box18Checked
            };

            for (int i = 0; i < settingsArray.Length; i++)
            {
                _checkedArray[i].Checked = settingsArray[i];
            }

            UpdateSweepItButtonState();
        }

        private void SaveSettingsEventHandler(object sender, FormClosingEventArgs e)
        {
            Settings.Default.FormHeight = Height;
            Settings.Default.FormWidth = Width;
            Settings.Default.FormLocation = Location;
            if (Settings.Default.FirstRun)
            {
                Settings.Default.FirstRun = false;
            }
            Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _checkedArray = new CheckBox[]
            {
                checkBox1, checkBox2, checkBox3, checkBox4, checkBox5,
                checkBox6, checkBox7, checkBox8, checkBox9, checkBox10,
                checkBox11, checkBox12, checkBox13, checkBox14, checkBox15, checkBox18
            };

            _checkedArrayBool = _checkedArray.Select(c => c.Checked).ToArray();

            RestoreSavedChecks();
            SetTheme();
            GetTotalReclaimableSpace();
            UpdateSweepItButtonState();
        }

        private string GetTotalReclaimableSpace()
        {
            double totalSpace = 0;
            StringBuilder report = new StringBuilder();

            var cleaners = new List<ICleaner>
            {
                new TemporaryFilesCleaner(outputWindow),
                new TemporarySetupFilesCleaner(outputWindow),
                new TemporaryInternetFilesCleaner(outputWindow),
                new EventViewerLogsCleaner(outputWindow),
                new RecycleBinCleaner(outputWindow),
                new ChromeCacheCleaner(outputWindow),
                new ThumbnailCacheCleaner(outputWindow),
                new UserFileHistoryCleaner(outputWindow),
                new WindowsOldDirectoryCleaner(outputWindow),
                new WindowsDefenderLogFilesCleaner(outputWindow),
                new MicrosoftOfficeCacheCleaner(outputWindow),
                new MicrosoftEdgeCacheCleaner(outputWindow),
                new WindowsInstallerCacheCleaner(outputWindow),
                new WindowsUpdateLogsCleaner(outputWindow),
                new WindowsErrorReportsCleaner(outputWindow),
                new DeliveryOptimizationFilesCleaner(outputWindow),
            };

            foreach (var cleaner in cleaners)
            {
                try
                {
                    var (fileType, spaceInMB) = cleaner.GetReclaimableSpace();
                    report.AppendLine($"{fileType}: {spaceInMB} MB");
                    totalSpace += spaceInMB;
                }
                catch (Exception ex)
                {
                    report.AppendLine($"Error calculating space: {ex.Message}");
                }
            }

            return outputWindow.Text += report.ToString() + $"\nTotal Reclaimable Space: {totalSpace}MB\n";
        }

        private async void Button1_Click_1(object sender, EventArgs e)
        {
            LockCleaning(true);

            double totalSpaceBefore = 0;
            double totalSpaceAfter = 0;

            var cleaners = new List<ICleaner>
            {
                _checkedArrayBool[0] ? new TemporaryFilesCleaner(outputWindow) : null,
                _checkedArrayBool[1] ? new TemporarySetupFilesCleaner(outputWindow) : null,
                _checkedArrayBool[2] ? new TemporaryInternetFilesCleaner(outputWindow) : null,
                _checkedArrayBool[3] ? new EventViewerLogsCleaner(outputWindow) : null,
                _checkedArrayBool[4] ? new RecycleBinCleaner(outputWindow) : null,
                _checkedArrayBool[5] ? new WindowsErrorReportsCleaner(outputWindow) : null,
                _checkedArrayBool[6] ? new DeliveryOptimizationFilesCleaner(outputWindow) : null,
                _checkedArrayBool[7] ? new ThumbnailCacheCleaner(outputWindow) : null,
                _checkedArrayBool[8] ? new UserFileHistoryCleaner(outputWindow) : null,
                _checkedArrayBool[9] ? new WindowsOldDirectoryCleaner(outputWindow) : null,
                _checkedArrayBool[10] ? new WindowsDefenderLogFilesCleaner(outputWindow) : null,
                _checkedArrayBool[11] ? new MicrosoftOfficeCacheCleaner(outputWindow) : null,
                _checkedArrayBool[12] ? new MicrosoftEdgeCacheCleaner(outputWindow) : null,
                _checkedArrayBool[13] ? new ChromeCacheCleaner(outputWindow) : null,
                _checkedArrayBool[14] ? new WindowsInstallerCacheCleaner(outputWindow) : null,
                _checkedArrayBool[15] ? new WindowsUpdateLogsCleaner(outputWindow) : null,
            };

            foreach (var cleaner in cleaners)
            {
                if (cleaner != null)
                {
                    var (fileType, spaceInMB) = cleaner.GetReclaimableSpace();
                    totalSpaceBefore += spaceInMB;
                }
            }

            foreach (var cleaner in cleaners)
            {
                if (cleaner != null)
                {
                    await cleaner.Reclaim();
                }
            }

            foreach (var cleaner in cleaners)
            {
                if (cleaner != null)
                {
                    var (fileType, spaceInMB) = cleaner.GetReclaimableSpace();
                    totalSpaceAfter += spaceInMB;
                }
            }

            double spaceReclaimed = totalSpaceBefore - totalSpaceAfter;
            outputWindow.Text += $"\nTotal Space Reclaimed: {spaceReclaimed}MB\n";

            LockCleaning(false);
        }

        public static void ScrollToOutputBottom()
        {
            var outputWindow = Application.OpenForms[0].Controls.Find("outputWindow", true).FirstOrDefault() as RichTextBox;
            outputWindow.SelectionStart = outputWindow.Text.Length;
            outputWindow.ScrollToCaret();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBox;
            int index = Array.IndexOf(_checkedArray, checkBox);
            _checkedArrayBool[index] = checkBox.Checked;
            UpdateSweepItButtonState();
        }

        private void UpdateSweepItButtonState()
        {
            SweepItButton.Enabled = _checkedArrayBool.Any(checkedState => checkedState);
        }

        private void LockCleaning(bool isEnabled)
        {
            isEnabled = !isEnabled;

            foreach (var box in _checkedArray)
            {
                box.Enabled = isEnabled;
            }

            SweepItButton.Enabled = isEnabled;
            SelectAllOptionsButton.Enabled = isEnabled;
            DeselectAllOptionsButton.Enabled = isEnabled;
        }

        private void HandleCheckmarks(bool value)
        {
            foreach (CheckBox box in _checkedArray)
            {
                box.Checked = value;
            }
        }

        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ = CheckForUpdates().ConfigureAwait(false);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveUserSelectedChecks();
            Dispose();
            Application.Exit();
        }

        private void DonateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("CleanSweep is provided completely free and it always will be!\n\nDonations aren't required but are always welcomed.\n\nWould you like to visit the donation page now?", "CleanSweep is free but beer is not!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://paypal.me/thomasloupe");
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var version = CleanSweepVersion.Version;
            var message = $"CleanSweep {version}\nCreated by: Thomas Loupe";
            var title = "About";

            CustomMessageBox.Show(
                message,
                title,
                ("Created by Thomas Loupe", "https://thomasloupe.com"),
                ("CleanSweep Github", "https://github.com/thomasloupe/CleanSweep"),
                ("Thomas's Github", "https://github.com/thomasloupe"),
                ("Twitter", "https://twitter.com/acid_rain")
            );
        }

        private void ToolStripButton1_Click_1(object sender, EventArgs e)
        {
            if (outputWindow.SelectionLength == 0)
            {
                outputWindow.SelectAll();
                outputWindow.Copy();
            }
        }

        private void ToolStripButton2_Click_2(object sender, EventArgs e)
        {
            outputWindow.Text = "";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            HandleCheckmarks(true);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            HandleCheckmarks(false);
        }

        private void SaveUserSelectedChecks()
        {
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
            Settings.Default.DarkMode = darkToolStripMenuItem.Checked;
        }

        private void SetTheme()
        {
            if (Settings.Default.DarkMode)
            {
                SetDarkMode();
                darkToolStripMenuItem.Checked = true;
                lightToolStripMenuItem.Checked = false;
            }
            else
            {
                SetLightMode();
                darkToolStripMenuItem.Checked = false;
                lightToolStripMenuItem.Checked = true;
            }
        }

        private void SetLightMode()
        {
            Color backgroundColor = Color.White;
            Color textColor = Color.Black;
            Color buttonBackColor = Color.LightGray;
            Color buttonTextColor = Color.Black;

            ApplyThemeToControls(this.Controls, backgroundColor, textColor, buttonBackColor, buttonTextColor);

            menuStrip1.BackColor = backgroundColor;
            menuStrip1.ForeColor = textColor;
            menuStrip1.Renderer = new CustomToolStripRenderer(backgroundColor, textColor);

            foreach (ToolStripMenuItem item in menuStrip1.Items)
            {
                item.ForeColor = textColor;
                item.BackColor = backgroundColor;
                UpdateDropDownItemsColor(item, backgroundColor, textColor);
            }

            BackColor = backgroundColor;
            Settings.Default.DarkMode = false;
            Settings.Default.Save();
        }

        private void SetDarkMode()
        {
            Color backgroundColor = Color.Black;
            Color textColor = Color.White;
            Color buttonBackColor = Color.DarkGray;
            Color buttonTextColor = Color.White;

            ApplyThemeToControls(this.Controls, backgroundColor, textColor, buttonBackColor, buttonTextColor);

            menuStrip1.BackColor = backgroundColor;
            menuStrip1.ForeColor = textColor;
            menuStrip1.Renderer = new CustomToolStripRenderer(backgroundColor, textColor);

            foreach (ToolStripMenuItem item in menuStrip1.Items)
            {
                item.ForeColor = textColor;
                item.BackColor = backgroundColor;
                UpdateDropDownItemsColor(item, backgroundColor, textColor);
            }

            BackColor = backgroundColor;
            Settings.Default.DarkMode = true;
            Settings.Default.Save();
        }

        private void ApplyThemeToControls(Control.ControlCollection controls, Color backgroundColor, Color textColor, Color buttonBackColor, Color buttonTextColor)
        {
            foreach (Control control in controls)
            {
                if (control is RichTextBox)
                {
                    control.BackColor = backgroundColor;
                    control.ForeColor = textColor;
                }
                else if (control is Button)
                {
                    control.BackColor = buttonBackColor;
                    control.ForeColor = buttonTextColor;
                }
                else if (control is CheckBox)
                {
                    control.BackColor = backgroundColor;
                    control.ForeColor = textColor;
                }
                else if (control is Panel || control is GroupBox)
                {
                    control.BackColor = backgroundColor;
                }
                else if (control is ToolStrip strip)
                {
                    strip.BackColor = backgroundColor;
                    strip.ForeColor = textColor;
                }
                else
                {
                    control.BackColor = backgroundColor;
                    control.ForeColor = textColor;
                }

                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls, backgroundColor, textColor, buttonBackColor, buttonTextColor);
                }
            }
        }

        private void UpdateDropDownItemsColor(ToolStripMenuItem menuItem, Color backgroundColor, Color textColor)
        {
            foreach (ToolStripItem subItem in menuItem.DropDownItems)
            {
                subItem.BackColor = backgroundColor;
                subItem.ForeColor = textColor;

                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    UpdateDropDownItemsColor(subMenuItem, backgroundColor, textColor);
                }
            }
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            darkToolStripMenuItem.Checked = true;
            lightToolStripMenuItem.Checked = false;
            SetDarkMode();
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lightToolStripMenuItem.Checked = true;
            darkToolStripMenuItem.Checked = false;
            SetLightMode();
        }

        private class CustomToolStripRenderer : ToolStripProfessionalRenderer
        {
            private readonly Color backgroundColor;
            private readonly Color textColor;
            private readonly Color hoverBackgroundColor;
            private readonly Color hoverTextColor;

            public CustomToolStripRenderer(Color backColor, Color foreColor) : base(new CustomColorTable(backColor, foreColor))
            {
                backgroundColor = backColor;
                textColor = foreColor;
                hoverBackgroundColor = backColor == Color.Black ? Color.Gray : Color.LightGray;
                hoverTextColor = foreColor;
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

                if (e.Item.Selected)
                {
                    using (SolidBrush brush = new SolidBrush(hoverBackgroundColor))
                    {
                        e.Graphics.FillRectangle(brush, rc);
                    }
                    e.Item.ForeColor = hoverTextColor;
                }
                else if (e.Item.Pressed)
                {
                    using (SolidBrush brush = new SolidBrush(hoverBackgroundColor))
                    {
                        e.Graphics.FillRectangle(brush, rc);
                    }
                    e.Item.ForeColor = hoverTextColor;
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(backgroundColor))
                    {
                        e.Graphics.FillRectangle(brush, rc);
                    }
                    e.Item.ForeColor = textColor;
                }
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                // Set the overall background of the tool strip
                using (SolidBrush brush = new SolidBrush(backgroundColor))
                {
                    e.Graphics.FillRectangle(brush, e.AffectedBounds);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                // Ensure the text is always visible and clear
                e.TextColor = e.Item.Selected ? hoverTextColor : textColor;
                base.OnRenderItemText(e);
            }
        }

        private class CustomColorTable : ProfessionalColorTable
        {
            private readonly Color menuBackColor;
            private readonly Color menuTextColor;

            public CustomColorTable(Color backColor, Color textColor)
            {
                menuBackColor = backColor;
                menuTextColor = textColor;
            }

            public override Color MenuItemSelected => menuTextColor;
            public override Color MenuItemSelectedGradientBegin => menuBackColor;
            public override Color MenuItemSelectedGradientEnd => menuBackColor;
            public override Color MenuItemBorder => menuTextColor;
            public override Color MenuItemPressedGradientBegin => menuBackColor;
            public override Color MenuItemPressedGradientEnd => menuBackColor;
            public override Color ToolStripDropDownBackground => menuBackColor;
        }

        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void ToolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void themeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
