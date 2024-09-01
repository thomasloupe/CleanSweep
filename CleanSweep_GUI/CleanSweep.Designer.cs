
using System;

namespace CleanSweep
{
    partial class CleanSweep
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CleanSweep));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.TemporaryFilesCheckbox = new System.Windows.Forms.Label();
            this.SweepItButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputWindow = new System.Windows.Forms.RichTextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.TemporarySetupFilesCheckbox = new System.Windows.Forms.Label();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.CopyLogButton = new System.Windows.Forms.ToolStripButton();
            this.ClearLogButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.TemporaryInternetFilesCheckbox = new System.Windows.Forms.Label();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.EventViewerLogsCheckbox = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.RecycleBinCheckbox = new System.Windows.Forms.Label();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.WindowsErrorReportsCheckbox = new System.Windows.Forms.Label();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.WindowsDeliveryOptimizationCheckbox = new System.Windows.Forms.Label();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.ThumbnailCacheCheckmark = new System.Windows.Forms.Label();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.UserFileHistoryCheckbox = new System.Windows.Forms.Label();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.WindowsOldFolderCheckbox = new System.Windows.Forms.Label();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.WindowsDefenderLogsCheckbox = new System.Windows.Forms.Label();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.MicrosoftOfficeCacheCheckbox = new System.Windows.Forms.Label();
            this.checkBox12 = new System.Windows.Forms.CheckBox();
            this.MicrosoftEdgeCacheCheckbox = new System.Windows.Forms.Label();
            this.checkBox13 = new System.Windows.Forms.CheckBox();
            this.ChomeCacheCheckmark = new System.Windows.Forms.Label();
            this.checkBox14 = new System.Windows.Forms.CheckBox();
            this.WindowsInstallerCacheCheckbox = new System.Windows.Forms.Label();
            this.checkBox15 = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.checkBox17 = new System.Windows.Forms.CheckBox();
            this.WindowsUpdateLogsCheckbox = new System.Windows.Forms.Label();
            this.checkBox18 = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.SelectAllOptionsButton = new System.Windows.Forms.Button();
            this.DeselectAllOptionsButton = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(7, 30);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // TemporaryFilesCheckbox
            // 
            this.TemporaryFilesCheckbox.AutoSize = true;
            this.TemporaryFilesCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.TemporaryFilesCheckbox.Location = new System.Drawing.Point(28, 30);
            this.TemporaryFilesCheckbox.Name = "TemporaryFilesCheckbox";
            this.TemporaryFilesCheckbox.Size = new System.Drawing.Size(81, 13);
            this.TemporaryFilesCheckbox.TabIndex = 8;
            this.TemporaryFilesCheckbox.Text = "Temporary Files";
            // 
            // SweepItButton
            // 
            this.SweepItButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SweepItButton.Location = new System.Drawing.Point(205, 206);
            this.SweepItButton.Name = "SweepItButton";
            this.SweepItButton.Size = new System.Drawing.Size(115, 46);
            this.SweepItButton.TabIndex = 6;
            this.SweepItButton.Text = "Sweep it!";
            this.SweepItButton.UseVisualStyleBackColor = true;
            this.SweepItButton.Click += new System.EventHandler(this.Button1_Click_1);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Black;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(534, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.fileToolStripMenuItem.ShowShortcutKeys = false;
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.FileToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.ToolTipText = "Exit CleanSweep";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.donateToolStripMenuItem});
            this.optionsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.optionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.optionsToolStripMenuItem.ShowShortcutKeys = false;
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check For Updates";
            this.checkForUpdatesToolStripMenuItem.ToolTipText = "Check the CleanSweep Github release page for the latest version and new features";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.ToolTipText = "Show the version number and other relevant information about CleanSweep";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // donateToolStripMenuItem
            // 
            this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            this.donateToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.donateToolStripMenuItem.Text = "Donate";
            this.donateToolStripMenuItem.ToolTipText = "Open a browser window to say thanks for CleanSweep";
            this.donateToolStripMenuItem.Click += new System.EventHandler(this.DonateToolStripMenuItem_Click);
            // 
            // outputWindow
            // 
            this.outputWindow.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.outputWindow.BackColor = System.Drawing.SystemColors.Desktop;
            this.outputWindow.Location = new System.Drawing.Point(0, 259);
            this.outputWindow.Name = "outputWindow";
            this.outputWindow.ReadOnly = true;
            this.outputWindow.Size = new System.Drawing.Size(534, 281);
            this.outputWindow.TabIndex = 11;
            this.outputWindow.Text = "";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(7, 58);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(15, 14);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // TemporarySetupFilesCheckbox
            // 
            this.TemporarySetupFilesCheckbox.AutoSize = true;
            this.TemporarySetupFilesCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.TemporarySetupFilesCheckbox.Location = new System.Drawing.Point(28, 58);
            this.TemporarySetupFilesCheckbox.Name = "TemporarySetupFilesCheckbox";
            this.TemporarySetupFilesCheckbox.Size = new System.Drawing.Size(112, 13);
            this.TemporarySetupFilesCheckbox.TabIndex = 13;
            this.TemporarySetupFilesCheckbox.Text = "Temporary Setup Files";
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            this.TopToolStripPanel.Click += new System.EventHandler(this.ToolStripContainer1_TopToolStripPanel_Click);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(434, 1);
            // 
            // CopyLogButton
            // 
            this.CopyLogButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CopyLogButton.Image = global::CleanSweep.Properties.Resources.copy;
            this.CopyLogButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CopyLogButton.Name = "CopyLogButton";
            this.CopyLogButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.CopyLogButton.Padding = new System.Windows.Forms.Padding(0, 5, 20, 0);
            this.CopyLogButton.Size = new System.Drawing.Size(40, 25);
            this.CopyLogButton.Text = "toolStripButton1";
            this.CopyLogButton.ToolTipText = "Copy selected/all text";
            this.CopyLogButton.Click += new System.EventHandler(this.ToolStripButton1_Click_1);
            // 
            // ClearLogButton
            // 
            this.ClearLogButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ClearLogButton.Image = ((System.Drawing.Image)(resources.GetObject("ClearLogButton.Image")));
            this.ClearLogButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ClearLogButton.Name = "ClearLogButton";
            this.ClearLogButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.ClearLogButton.Padding = new System.Windows.Forms.Padding(20, 5, 0, 0);
            this.ClearLogButton.Size = new System.Drawing.Size(40, 25);
            this.ClearLogButton.Text = "toolStripButton2";
            this.ClearLogButton.ToolTipText = "Clear the entire log window";
            this.ClearLogButton.Click += new System.EventHandler(this.ToolStripButton2_Click_2);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toolStrip1.BackColor = System.Drawing.Color.Black;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CopyLogButton,
            this.ClearLogButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 543);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(83, 28);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // TemporaryInternetFilesCheckbox
            // 
            this.TemporaryInternetFilesCheckbox.AutoSize = true;
            this.TemporaryInternetFilesCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.TemporaryInternetFilesCheckbox.Location = new System.Drawing.Point(28, 87);
            this.TemporaryInternetFilesCheckbox.Name = "TemporaryInternetFilesCheckbox";
            this.TemporaryInternetFilesCheckbox.Size = new System.Drawing.Size(120, 13);
            this.TemporaryInternetFilesCheckbox.TabIndex = 16;
            this.TemporaryInternetFilesCheckbox.Text = "Temporary Internet Files";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(7, 87);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(15, 14);
            this.checkBox3.TabIndex = 15;
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // EventViewerLogsCheckbox
            // 
            this.EventViewerLogsCheckbox.AutoSize = true;
            this.EventViewerLogsCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.EventViewerLogsCheckbox.Location = new System.Drawing.Point(28, 117);
            this.EventViewerLogsCheckbox.Name = "EventViewerLogsCheckbox";
            this.EventViewerLogsCheckbox.Size = new System.Drawing.Size(96, 13);
            this.EventViewerLogsCheckbox.TabIndex = 18;
            this.EventViewerLogsCheckbox.Text = "Event Viewer Logs";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(7, 117);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(15, 14);
            this.checkBox4.TabIndex = 17;
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // RecycleBinCheckbox
            // 
            this.RecycleBinCheckbox.AutoSize = true;
            this.RecycleBinCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.RecycleBinCheckbox.Location = new System.Drawing.Point(28, 146);
            this.RecycleBinCheckbox.Name = "RecycleBinCheckbox";
            this.RecycleBinCheckbox.Size = new System.Drawing.Size(64, 13);
            this.RecycleBinCheckbox.TabIndex = 20;
            this.RecycleBinCheckbox.Text = "Recycle Bin";
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(7, 146);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(15, 14);
            this.checkBox5.TabIndex = 19;
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // WindowsErrorReportsCheckbox
            // 
            this.WindowsErrorReportsCheckbox.AutoSize = true;
            this.WindowsErrorReportsCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.WindowsErrorReportsCheckbox.Location = new System.Drawing.Point(376, 88);
            this.WindowsErrorReportsCheckbox.Name = "WindowsErrorReportsCheckbox";
            this.WindowsErrorReportsCheckbox.Size = new System.Drawing.Size(116, 13);
            this.WindowsErrorReportsCheckbox.TabIndex = 22;
            this.WindowsErrorReportsCheckbox.Text = "Windows Error Reports";
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(355, 88);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(15, 14);
            this.checkBox6.TabIndex = 21;
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // WindowsDeliveryOptimizationCheckbox
            // 
            this.WindowsDeliveryOptimizationCheckbox.AutoSize = true;
            this.WindowsDeliveryOptimizationCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.WindowsDeliveryOptimizationCheckbox.Location = new System.Drawing.Point(376, 119);
            this.WindowsDeliveryOptimizationCheckbox.Name = "WindowsDeliveryOptimizationCheckbox";
            this.WindowsDeliveryOptimizationCheckbox.Size = new System.Drawing.Size(152, 13);
            this.WindowsDeliveryOptimizationCheckbox.TabIndex = 24;
            this.WindowsDeliveryOptimizationCheckbox.Text = "Windows Delivery Optimization";
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(355, 119);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(15, 14);
            this.checkBox7.TabIndex = 23;
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox7.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // ThumbnailCacheCheckmark
            // 
            this.ThumbnailCacheCheckmark.AutoSize = true;
            this.ThumbnailCacheCheckmark.ForeColor = System.Drawing.SystemColors.Control;
            this.ThumbnailCacheCheckmark.Location = new System.Drawing.Point(196, 29);
            this.ThumbnailCacheCheckmark.Name = "ThumbnailCacheCheckmark";
            this.ThumbnailCacheCheckmark.Size = new System.Drawing.Size(90, 13);
            this.ThumbnailCacheCheckmark.TabIndex = 26;
            this.ThumbnailCacheCheckmark.Text = "Thumbnail Cache";
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Location = new System.Drawing.Point(175, 29);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(15, 14);
            this.checkBox8.TabIndex = 25;
            this.checkBox8.UseVisualStyleBackColor = true;
            this.checkBox8.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // UserFileHistoryCheckbox
            // 
            this.UserFileHistoryCheckbox.AutoSize = true;
            this.UserFileHistoryCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.UserFileHistoryCheckbox.Location = new System.Drawing.Point(196, 58);
            this.UserFileHistoryCheckbox.Name = "UserFileHistoryCheckbox";
            this.UserFileHistoryCheckbox.Size = new System.Drawing.Size(83, 13);
            this.UserFileHistoryCheckbox.TabIndex = 28;
            this.UserFileHistoryCheckbox.Text = "User File History";
            // 
            // checkBox9
            // 
            this.checkBox9.AutoSize = true;
            this.checkBox9.Location = new System.Drawing.Point(175, 58);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(15, 14);
            this.checkBox9.TabIndex = 27;
            this.checkBox9.UseVisualStyleBackColor = true;
            this.checkBox9.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // WindowsOldFolderCheckbox
            // 
            this.WindowsOldFolderCheckbox.AutoSize = true;
            this.WindowsOldFolderCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.WindowsOldFolderCheckbox.Location = new System.Drawing.Point(196, 87);
            this.WindowsOldFolderCheckbox.Name = "WindowsOldFolderCheckbox";
            this.WindowsOldFolderCheckbox.Size = new System.Drawing.Size(108, 13);
            this.WindowsOldFolderCheckbox.TabIndex = 30;
            this.WindowsOldFolderCheckbox.Text = "Windows OLD Folder";
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.Location = new System.Drawing.Point(175, 87);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(15, 14);
            this.checkBox10.TabIndex = 29;
            this.checkBox10.UseVisualStyleBackColor = true;
            this.checkBox10.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // WindowsDefenderLogsCheckbox
            // 
            this.WindowsDefenderLogsCheckbox.AutoSize = true;
            this.WindowsDefenderLogsCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.WindowsDefenderLogsCheckbox.Location = new System.Drawing.Point(196, 117);
            this.WindowsDefenderLogsCheckbox.Name = "WindowsDefenderLogsCheckbox";
            this.WindowsDefenderLogsCheckbox.Size = new System.Drawing.Size(124, 13);
            this.WindowsDefenderLogsCheckbox.TabIndex = 32;
            this.WindowsDefenderLogsCheckbox.Text = "Windows Defender Logs";
            // 
            // checkBox11
            // 
            this.checkBox11.AutoSize = true;
            this.checkBox11.Location = new System.Drawing.Point(175, 117);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(15, 14);
            this.checkBox11.TabIndex = 31;
            this.checkBox11.UseVisualStyleBackColor = true;
            this.checkBox11.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // MicrosoftOfficeCacheCheckbox
            // 
            this.MicrosoftOfficeCacheCheckbox.AutoSize = true;
            this.MicrosoftOfficeCacheCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.MicrosoftOfficeCacheCheckbox.Location = new System.Drawing.Point(196, 146);
            this.MicrosoftOfficeCacheCheckbox.Name = "MicrosoftOfficeCacheCheckbox";
            this.MicrosoftOfficeCacheCheckbox.Size = new System.Drawing.Size(115, 13);
            this.MicrosoftOfficeCacheCheckbox.TabIndex = 34;
            this.MicrosoftOfficeCacheCheckbox.Text = "Microsoft Office Cache";
            // 
            // checkBox12
            // 
            this.checkBox12.AutoSize = true;
            this.checkBox12.Location = new System.Drawing.Point(175, 146);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new System.Drawing.Size(15, 14);
            this.checkBox12.TabIndex = 33;
            this.checkBox12.UseVisualStyleBackColor = true;
            this.checkBox12.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // MicrosoftEdgeCacheCheckbox
            // 
            this.MicrosoftEdgeCacheCheckbox.AutoSize = true;
            this.MicrosoftEdgeCacheCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.MicrosoftEdgeCacheCheckbox.Location = new System.Drawing.Point(196, 177);
            this.MicrosoftEdgeCacheCheckbox.Name = "MicrosoftEdgeCacheCheckbox";
            this.MicrosoftEdgeCacheCheckbox.Size = new System.Drawing.Size(112, 13);
            this.MicrosoftEdgeCacheCheckbox.TabIndex = 36;
            this.MicrosoftEdgeCacheCheckbox.Text = "Microsoft Edge Cache";
            // 
            // checkBox13
            // 
            this.checkBox13.AutoSize = true;
            this.checkBox13.Location = new System.Drawing.Point(175, 177);
            this.checkBox13.Name = "checkBox13";
            this.checkBox13.Size = new System.Drawing.Size(15, 14);
            this.checkBox13.TabIndex = 35;
            this.checkBox13.UseVisualStyleBackColor = true;
            this.checkBox13.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // ChomeCacheCheckmark
            // 
            this.ChomeCacheCheckmark.AutoSize = true;
            this.ChomeCacheCheckmark.ForeColor = System.Drawing.SystemColors.Control;
            this.ChomeCacheCheckmark.Location = new System.Drawing.Point(28, 176);
            this.ChomeCacheCheckmark.Name = "ChomeCacheCheckmark";
            this.ChomeCacheCheckmark.Size = new System.Drawing.Size(77, 13);
            this.ChomeCacheCheckmark.TabIndex = 38;
            this.ChomeCacheCheckmark.Text = "Chrome Cache";
            // 
            // checkBox14
            // 
            this.checkBox14.AutoSize = true;
            this.checkBox14.Location = new System.Drawing.Point(7, 176);
            this.checkBox14.Name = "checkBox14";
            this.checkBox14.Size = new System.Drawing.Size(15, 14);
            this.checkBox14.TabIndex = 37;
            this.checkBox14.UseVisualStyleBackColor = true;
            this.checkBox14.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // WindowsInstallerCacheCheckbox
            // 
            this.WindowsInstallerCacheCheckbox.AutoSize = true;
            this.WindowsInstallerCacheCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.WindowsInstallerCacheCheckbox.Location = new System.Drawing.Point(376, 31);
            this.WindowsInstallerCacheCheckbox.Name = "WindowsInstallerCacheCheckbox";
            this.WindowsInstallerCacheCheckbox.Size = new System.Drawing.Size(124, 13);
            this.WindowsInstallerCacheCheckbox.TabIndex = 40;
            this.WindowsInstallerCacheCheckbox.Text = "Windows Installer Cache";
            // 
            // checkBox15
            // 
            this.checkBox15.AutoSize = true;
            this.checkBox15.Location = new System.Drawing.Point(355, 31);
            this.checkBox15.Name = "checkBox15";
            this.checkBox15.Size = new System.Drawing.Size(15, 14);
            this.checkBox15.TabIndex = 39;
            this.checkBox15.UseVisualStyleBackColor = true;
            this.checkBox15.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(0, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 23);
            this.label17.TabIndex = 0;
            // 
            // checkBox17
            // 
            this.checkBox17.Location = new System.Drawing.Point(0, 0);
            this.checkBox17.Name = "checkBox17";
            this.checkBox17.Size = new System.Drawing.Size(104, 24);
            this.checkBox17.TabIndex = 0;
            // 
            // WindowsUpdateLogsCheckbox
            // 
            this.WindowsUpdateLogsCheckbox.AutoSize = true;
            this.WindowsUpdateLogsCheckbox.ForeColor = System.Drawing.SystemColors.Control;
            this.WindowsUpdateLogsCheckbox.Location = new System.Drawing.Point(376, 58);
            this.WindowsUpdateLogsCheckbox.Name = "WindowsUpdateLogsCheckbox";
            this.WindowsUpdateLogsCheckbox.Size = new System.Drawing.Size(115, 13);
            this.WindowsUpdateLogsCheckbox.TabIndex = 46;
            this.WindowsUpdateLogsCheckbox.Text = "Windows Update Logs";
            // 
            // checkBox18
            // 
            this.checkBox18.AutoSize = true;
            this.checkBox18.Location = new System.Drawing.Point(355, 58);
            this.checkBox18.Name = "checkBox18";
            this.checkBox18.Size = new System.Drawing.Size(15, 14);
            this.checkBox18.TabIndex = 47;
            this.checkBox18.UseVisualStyleBackColor = true;
            this.checkBox18.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(0, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 23);
            this.label16.TabIndex = 0;
            // 
            // SelectAllOptionsButton
            // 
            this.SelectAllOptionsButton.Location = new System.Drawing.Point(355, 166);
            this.SelectAllOptionsButton.Name = "SelectAllOptionsButton";
            this.SelectAllOptionsButton.Size = new System.Drawing.Size(75, 23);
            this.SelectAllOptionsButton.TabIndex = 49;
            this.SelectAllOptionsButton.Text = "Select All";
            this.SelectAllOptionsButton.UseVisualStyleBackColor = true;
            this.SelectAllOptionsButton.Click += new System.EventHandler(this.Button2_Click);
            // 
            // DeselectAllOptionsButton
            // 
            this.DeselectAllOptionsButton.Location = new System.Drawing.Point(436, 166);
            this.DeselectAllOptionsButton.Name = "DeselectAllOptionsButton";
            this.DeselectAllOptionsButton.Size = new System.Drawing.Size(75, 23);
            this.DeselectAllOptionsButton.TabIndex = 50;
            this.DeselectAllOptionsButton.Text = "Deselect All";
            this.DeselectAllOptionsButton.UseVisualStyleBackColor = true;
            this.DeselectAllOptionsButton.Click += new System.EventHandler(this.Button3_Click);
            // 
            // label19
            // 
            this.label19.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label19.Location = new System.Drawing.Point(0, 203);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(540, 2);
            this.label19.TabIndex = 52;
            // 
            // CleanSweep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(534, 568);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.DeselectAllOptionsButton);
            this.Controls.Add(this.SelectAllOptionsButton);
            this.Controls.Add(this.checkBox18);
            this.Controls.Add(this.WindowsUpdateLogsCheckbox);
            this.Controls.Add(this.WindowsInstallerCacheCheckbox);
            this.Controls.Add(this.checkBox15);
            this.Controls.Add(this.ChomeCacheCheckmark);
            this.Controls.Add(this.checkBox14);
            this.Controls.Add(this.MicrosoftEdgeCacheCheckbox);
            this.Controls.Add(this.checkBox13);
            this.Controls.Add(this.MicrosoftOfficeCacheCheckbox);
            this.Controls.Add(this.checkBox12);
            this.Controls.Add(this.WindowsDefenderLogsCheckbox);
            this.Controls.Add(this.checkBox11);
            this.Controls.Add(this.WindowsOldFolderCheckbox);
            this.Controls.Add(this.checkBox10);
            this.Controls.Add(this.UserFileHistoryCheckbox);
            this.Controls.Add(this.checkBox9);
            this.Controls.Add(this.ThumbnailCacheCheckmark);
            this.Controls.Add(this.checkBox8);
            this.Controls.Add(this.WindowsDeliveryOptimizationCheckbox);
            this.Controls.Add(this.checkBox7);
            this.Controls.Add(this.WindowsErrorReportsCheckbox);
            this.Controls.Add(this.checkBox6);
            this.Controls.Add(this.RecycleBinCheckbox);
            this.Controls.Add(this.checkBox5);
            this.Controls.Add(this.EventViewerLogsCheckbox);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.TemporaryInternetFilesCheckbox);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.TemporarySetupFilesCheckbox);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.outputWindow);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.TemporaryFilesCheckbox);
            this.Controls.Add(this.SweepItButton);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "CleanSweep";
            this.Text = "CleanSweep";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label TemporaryFilesCheckbox;
        private System.Windows.Forms.Button SweepItButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donateToolStripMenuItem;
        private System.Windows.Forms.RichTextBox outputWindow;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label TemporarySetupFilesCheckbox;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStripButton CopyLogButton;
        private System.Windows.Forms.ToolStripButton ClearLogButton;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Label TemporaryInternetFilesCheckbox;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Label EventViewerLogsCheckbox;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Label RecycleBinCheckbox;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.Label WindowsErrorReportsCheckbox;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.Label WindowsDeliveryOptimizationCheckbox;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.Label ThumbnailCacheCheckmark;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.Label UserFileHistoryCheckbox;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.Label WindowsOldFolderCheckbox;
        private System.Windows.Forms.CheckBox checkBox10;
        private System.Windows.Forms.Label WindowsDefenderLogsCheckbox;
        private System.Windows.Forms.CheckBox checkBox11;
        private System.Windows.Forms.Label MicrosoftOfficeCacheCheckbox;
        private System.Windows.Forms.CheckBox checkBox12;
        private System.Windows.Forms.Label MicrosoftEdgeCacheCheckbox;
        private System.Windows.Forms.CheckBox checkBox13;
        private System.Windows.Forms.Label ChomeCacheCheckmark;
        private System.Windows.Forms.CheckBox checkBox14;
        private System.Windows.Forms.Label WindowsInstallerCacheCheckbox;
        private System.Windows.Forms.CheckBox checkBox15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox checkBox17;
        private System.Windows.Forms.Label WindowsUpdateLogsCheckbox;
        private System.Windows.Forms.CheckBox checkBox18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button SelectAllOptionsButton;
        private System.Windows.Forms.Button DeselectAllOptionsButton;
        private System.Windows.Forms.Label label19;
    }
}

