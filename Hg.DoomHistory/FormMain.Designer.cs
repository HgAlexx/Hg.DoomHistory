namespace Hg.DoomHistory
{
    partial class FormMain
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.groupBoxGlobalConf = new System.Windows.Forms.GroupBox();
            this.buttonBackupFolderOpen = new System.Windows.Forms.Button();
            this.buttonBackupBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxBackupFolder = new System.Windows.Forms.TextBox();
            this.buttonSavedGamesBrowse = new System.Windows.Forms.Button();
            this.buttonAutoDetect = new System.Windows.Forms.Button();
            this.labelSavedGamesFolder = new System.Windows.Forms.Label();
            this.textBoxSavedGamesFolder = new System.Windows.Forms.TextBox();
            this.folderBrowserDialogSavedGames = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSlot1 = new System.Windows.Forms.TabPage();
            this.slotControl1 = new Hg.DoomHistory.SlotControl();
            this.tabPageSlot2 = new System.Windows.Forms.TabPage();
            this.slotControl2 = new Hg.DoomHistory.SlotControl();
            this.tabPageSlot3 = new System.Windows.Forms.TabPage();
            this.slotControl3 = new Hg.DoomHistory.SlotControl();
            this.folderBrowserDialogBackup = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTipHelp = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.messageBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListMessageType = new System.Windows.Forms.ImageList(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importExistingBackupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxGlobalConf.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageSlot1.SuspendLayout();
            this.tabPageSlot2.SuspendLayout();
            this.tabPageSlot3.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxGlobalConf
            // 
            this.groupBoxGlobalConf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGlobalConf.Controls.Add(this.buttonBackupFolderOpen);
            this.groupBoxGlobalConf.Controls.Add(this.buttonBackupBrowse);
            this.groupBoxGlobalConf.Controls.Add(this.label1);
            this.groupBoxGlobalConf.Controls.Add(this.textBoxBackupFolder);
            this.groupBoxGlobalConf.Controls.Add(this.buttonSavedGamesBrowse);
            this.groupBoxGlobalConf.Controls.Add(this.buttonAutoDetect);
            this.groupBoxGlobalConf.Controls.Add(this.labelSavedGamesFolder);
            this.groupBoxGlobalConf.Controls.Add(this.textBoxSavedGamesFolder);
            this.groupBoxGlobalConf.Location = new System.Drawing.Point(12, 27);
            this.groupBoxGlobalConf.Name = "groupBoxGlobalConf";
            this.groupBoxGlobalConf.Size = new System.Drawing.Size(809, 73);
            this.groupBoxGlobalConf.TabIndex = 0;
            this.groupBoxGlobalConf.TabStop = false;
            this.groupBoxGlobalConf.Text = "Global Settings";
            // 
            // buttonBackupFolderOpen
            // 
            this.buttonBackupFolderOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBackupFolderOpen.Location = new System.Drawing.Point(728, 43);
            this.buttonBackupFolderOpen.Name = "buttonBackupFolderOpen";
            this.buttonBackupFolderOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonBackupFolderOpen.TabIndex = 8;
            this.buttonBackupFolderOpen.Text = "Open";
            this.buttonBackupFolderOpen.UseVisualStyleBackColor = true;
            this.buttonBackupFolderOpen.Click += new System.EventHandler(this.ButtonBackupFolderOpen_Click);
            // 
            // buttonBackupBrowse
            // 
            this.buttonBackupBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBackupBrowse.Location = new System.Drawing.Point(647, 43);
            this.buttonBackupBrowse.Name = "buttonBackupBrowse";
            this.buttonBackupBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBackupBrowse.TabIndex = 7;
            this.buttonBackupBrowse.Text = "Browse";
            this.buttonBackupBrowse.UseVisualStyleBackColor = true;
            this.buttonBackupBrowse.Click += new System.EventHandler(this.ButtonBackupBrowse_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Backup Folder";
            // 
            // textBoxBackupFolder
            // 
            this.textBoxBackupFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBackupFolder.Location = new System.Drawing.Point(146, 45);
            this.textBoxBackupFolder.Name = "textBoxBackupFolder";
            this.textBoxBackupFolder.ReadOnly = true;
            this.textBoxBackupFolder.Size = new System.Drawing.Size(495, 20);
            this.textBoxBackupFolder.TabIndex = 4;
            // 
            // buttonSavedGamesBrowse
            // 
            this.buttonSavedGamesBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSavedGamesBrowse.Location = new System.Drawing.Point(647, 17);
            this.buttonSavedGamesBrowse.Name = "buttonSavedGamesBrowse";
            this.buttonSavedGamesBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonSavedGamesBrowse.TabIndex = 3;
            this.buttonSavedGamesBrowse.Text = "Browse";
            this.buttonSavedGamesBrowse.UseVisualStyleBackColor = true;
            this.buttonSavedGamesBrowse.Click += new System.EventHandler(this.ButtonSavedGamesBrowse_Click);
            // 
            // buttonAutoDetect
            // 
            this.buttonAutoDetect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAutoDetect.Location = new System.Drawing.Point(728, 17);
            this.buttonAutoDetect.Name = "buttonAutoDetect";
            this.buttonAutoDetect.Size = new System.Drawing.Size(75, 23);
            this.buttonAutoDetect.TabIndex = 2;
            this.buttonAutoDetect.Text = "Auto Detect";
            this.buttonAutoDetect.UseVisualStyleBackColor = true;
            this.buttonAutoDetect.Click += new System.EventHandler(this.ButtonAutoDetect_Click);
            // 
            // labelSavedGamesFolder
            // 
            this.labelSavedGamesFolder.Location = new System.Drawing.Point(6, 22);
            this.labelSavedGamesFolder.Name = "labelSavedGamesFolder";
            this.labelSavedGamesFolder.Size = new System.Drawing.Size(134, 17);
            this.labelSavedGamesFolder.TabIndex = 1;
            this.labelSavedGamesFolder.Text = "Saved Games Folder";
            // 
            // textBoxSavedGamesFolder
            // 
            this.textBoxSavedGamesFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSavedGamesFolder.Location = new System.Drawing.Point(146, 19);
            this.textBoxSavedGamesFolder.Name = "textBoxSavedGamesFolder";
            this.textBoxSavedGamesFolder.ReadOnly = true;
            this.textBoxSavedGamesFolder.Size = new System.Drawing.Size(495, 20);
            this.textBoxSavedGamesFolder.TabIndex = 0;
            this.toolTipHelp.SetToolTip(this.textBoxSavedGamesFolder, "This must be the folder containing the GAME-AUTOSAVEX folders");
            // 
            // folderBrowserDialogSavedGames
            // 
            this.folderBrowserDialogSavedGames.RootFolder = System.Environment.SpecialFolder.UserProfile;
            this.folderBrowserDialogSavedGames.ShowNewFolderButton = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageSlot1);
            this.tabControl1.Controls.Add(this.tabPageSlot2);
            this.tabControl1.Controls.Add(this.tabPageSlot3);
            this.tabControl1.Location = new System.Drawing.Point(12, 106);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(809, 465);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageSlot1
            // 
            this.tabPageSlot1.Controls.Add(this.slotControl1);
            this.tabPageSlot1.Location = new System.Drawing.Point(4, 22);
            this.tabPageSlot1.Name = "tabPageSlot1";
            this.tabPageSlot1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSlot1.Size = new System.Drawing.Size(801, 439);
            this.tabPageSlot1.TabIndex = 0;
            this.tabPageSlot1.Text = "Slot 1";
            this.tabPageSlot1.UseVisualStyleBackColor = true;
            // 
            // slotControl1
            // 
            this.slotControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slotControl1.Location = new System.Drawing.Point(3, 3);
            this.slotControl1.Name = "slotControl1";
            this.slotControl1.Size = new System.Drawing.Size(795, 433);
            this.slotControl1.TabIndex = 0;
            // 
            // tabPageSlot2
            // 
            this.tabPageSlot2.Controls.Add(this.slotControl2);
            this.tabPageSlot2.Location = new System.Drawing.Point(4, 22);
            this.tabPageSlot2.Name = "tabPageSlot2";
            this.tabPageSlot2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSlot2.Size = new System.Drawing.Size(801, 439);
            this.tabPageSlot2.TabIndex = 1;
            this.tabPageSlot2.Text = "Slot 2";
            this.tabPageSlot2.UseVisualStyleBackColor = true;
            // 
            // slotControl2
            // 
            this.slotControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slotControl2.Location = new System.Drawing.Point(3, 3);
            this.slotControl2.Name = "slotControl2";
            this.slotControl2.Size = new System.Drawing.Size(795, 433);
            this.slotControl2.TabIndex = 0;
            // 
            // tabPageSlot3
            // 
            this.tabPageSlot3.Controls.Add(this.slotControl3);
            this.tabPageSlot3.Location = new System.Drawing.Point(4, 22);
            this.tabPageSlot3.Name = "tabPageSlot3";
            this.tabPageSlot3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSlot3.Size = new System.Drawing.Size(801, 439);
            this.tabPageSlot3.TabIndex = 2;
            this.tabPageSlot3.Text = "Slot 3";
            this.tabPageSlot3.UseVisualStyleBackColor = true;
            // 
            // slotControl3
            // 
            this.slotControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slotControl3.Location = new System.Drawing.Point(3, 3);
            this.slotControl3.Name = "slotControl3";
            this.slotControl3.Size = new System.Drawing.Size(795, 433);
            this.slotControl3.TabIndex = 0;
            // 
            // toolTipHelp
            // 
            this.toolTipHelp.AutoPopDelay = 20000;
            this.toolTipHelp.InitialDelay = 100;
            this.toolTipHelp.ReshowDelay = 10;
            this.toolTipHelp.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipHelp.ToolTipTitle = "Help";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 574);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(833, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatus.Text = "Status";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(833, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.importExistingBackupsToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearSettingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.messageBoxToolStripMenuItem,
            this.statusBarToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
            this.toolStripMenuItem1.Text = "Notification mode";
            // 
            // messageBoxToolStripMenuItem
            // 
            this.messageBoxToolStripMenuItem.Name = "messageBoxToolStripMenuItem";
            this.messageBoxToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.messageBoxToolStripMenuItem.Text = "Message box";
            this.messageBoxToolStripMenuItem.Click += new System.EventHandler(this.messageBoxToolStripMenuItem_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.statusBarToolStripMenuItem.Text = "Status bar";
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.statusBarToolStripMenuItem_Click);
            // 
            // clearSettingsToolStripMenuItem
            // 
            this.clearSettingsToolStripMenuItem.Name = "clearSettingsToolStripMenuItem";
            this.clearSettingsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.clearSettingsToolStripMenuItem.Text = "Clear settings";
            this.clearSettingsToolStripMenuItem.Click += new System.EventHandler(this.clearSettingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // imageListMessageType
            // 
            this.imageListMessageType.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMessageType.ImageStream")));
            this.imageListMessageType.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMessageType.Images.SetKeyName(0, "messagebox-error.png");
            this.imageListMessageType.Images.SetKeyName(1, "messagebox-information.png");
            this.imageListMessageType.Images.SetKeyName(2, "messagebox-question.png");
            this.imageListMessageType.Images.SetKeyName(3, "messagebox-exclamation.png");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // importExistingBackupsToolStripMenuItem
            // 
            this.importExistingBackupsToolStripMenuItem.Name = "importExistingBackupsToolStripMenuItem";
            this.importExistingBackupsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.importExistingBackupsToolStripMenuItem.Text = "Import existing backups";
            this.importExistingBackupsToolStripMenuItem.Click += new System.EventHandler(this.importExistingBackupsToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 596);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBoxGlobalConf);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(725, 550);
            this.Name = "FormMain";
            this.Text = "Hg.DoomHistory";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBoxGlobalConf.ResumeLayout(false);
            this.groupBoxGlobalConf.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageSlot1.ResumeLayout(false);
            this.tabPageSlot2.ResumeLayout(false);
            this.tabPageSlot3.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxGlobalConf;
        private System.Windows.Forms.Button buttonSavedGamesBrowse;
        private System.Windows.Forms.Button buttonAutoDetect;
        private System.Windows.Forms.Label labelSavedGamesFolder;
        private System.Windows.Forms.TextBox textBoxSavedGamesFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogSavedGames;
        private System.Windows.Forms.Button buttonBackupBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxBackupFolder;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSlot1;
        private System.Windows.Forms.TabPage tabPageSlot2;
        private System.Windows.Forms.TabPage tabPageSlot3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogBackup;
        private SlotControl slotControl1;
        private SlotControl slotControl2;
        private SlotControl slotControl3;
        private System.Windows.Forms.Button buttonBackupFolderOpen;
        private System.Windows.Forms.ToolTip toolTipHelp;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem messageBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSettingsToolStripMenuItem;
        private System.Windows.Forms.ImageList imageListMessageType;
        private System.Windows.Forms.ToolStripMenuItem importExistingBackupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

