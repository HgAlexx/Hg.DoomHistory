namespace Hg.DoomHistory
{
    partial class SlotControl
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

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.comboBoxMaps = new System.Windows.Forms.ComboBox();
            this.labelMaps = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listViewSavedGames = new System.Windows.Forms.ListView();
            this.columnHeaderLastSave = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDiff = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPlayedTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderIsDeath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderNotes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.propertyGridGameDetails = new System.Windows.Forms.PropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pictureBoxScreenshot = new System.Windows.Forms.PictureBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonRestore = new System.Windows.Forms.Button();
            this.groupBoxConfig = new System.Windows.Forms.GroupBox();
            this.checkBoxScreenShot = new System.Windows.Forms.CheckBox();
            this.buttonBackupNow = new System.Windows.Forms.Button();
            this.checkBoxIncludeDeath = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoBackup = new System.Windows.Forms.CheckBox();
            this.toolTipHelp = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.contextMenuStripListView.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenshot)).BeginInit();
            this.groupBoxConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxMaps
            // 
            this.comboBoxMaps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxMaps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMaps.FormattingEnabled = true;
            this.comboBoxMaps.Location = new System.Drawing.Point(66, 77);
            this.comboBoxMaps.Name = "comboBoxMaps";
            this.comboBoxMaps.Size = new System.Drawing.Size(601, 21);
            this.comboBoxMaps.TabIndex = 0;
            // 
            // labelMaps
            // 
            this.labelMaps.Location = new System.Drawing.Point(3, 80);
            this.labelMaps.Name = "labelMaps";
            this.labelMaps.Size = new System.Drawing.Size(57, 13);
            this.labelMaps.TabIndex = 1;
            this.labelMaps.Text = "Maps";
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 104);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listViewSavedGames);
            this.splitContainer.Panel1MinSize = 300;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer.Panel2.Controls.Add(this.buttonRestore);
            this.splitContainer.Size = new System.Drawing.Size(670, 274);
            this.splitContainer.SplitterDistance = 400;
            this.splitContainer.TabIndex = 2;
            // 
            // listViewSavedGames
            // 
            this.listViewSavedGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderLastSave,
            this.columnHeaderDiff,
            this.columnHeaderPlayedTime,
            this.columnHeaderIsDeath,
            this.columnHeaderNotes});
            this.listViewSavedGames.ContextMenuStrip = this.contextMenuStripListView;
            this.listViewSavedGames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewSavedGames.FullRowSelect = true;
            this.listViewSavedGames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewSavedGames.HideSelection = false;
            this.listViewSavedGames.Location = new System.Drawing.Point(0, 0);
            this.listViewSavedGames.Name = "listViewSavedGames";
            this.listViewSavedGames.Size = new System.Drawing.Size(400, 274);
            this.listViewSavedGames.TabIndex = 0;
            this.listViewSavedGames.UseCompatibleStateImageBehavior = false;
            this.listViewSavedGames.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderLastSave
            // 
            this.columnHeaderLastSave.Text = "Last Saved";
            this.columnHeaderLastSave.Width = 120;
            // 
            // columnHeaderDiff
            // 
            this.columnHeaderDiff.Text = "Difficulty";
            this.columnHeaderDiff.Width = 120;
            // 
            // columnHeaderPlayedTime
            // 
            this.columnHeaderPlayedTime.Text = "PlayedTime";
            this.columnHeaderPlayedTime.Width = 85;
            // 
            // columnHeaderIsDeath
            // 
            this.columnHeaderIsDeath.Text = "";
            this.columnHeaderIsDeath.Width = 20;
            // 
            // columnHeaderNotes
            // 
            this.columnHeaderNotes.Text = "Notes";
            this.columnHeaderNotes.Width = 100;
            // 
            // contextMenuStripListView
            // 
            this.contextMenuStripListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRestore,
            this.toolStripMenuItemEdit,
            this.toolStripSeparator1,
            this.toolStripMenuItemDelete});
            this.contextMenuStripListView.Name = "contextMenuStripListView";
            this.contextMenuStripListView.Size = new System.Drawing.Size(127, 76);
            // 
            // toolStripMenuItemRestore
            // 
            this.toolStripMenuItemRestore.Name = "toolStripMenuItemRestore";
            this.toolStripMenuItemRestore.Size = new System.Drawing.Size(126, 22);
            this.toolStripMenuItemRestore.Text = "Restore";
            // 
            // toolStripMenuItemEdit
            // 
            this.toolStripMenuItemEdit.Name = "toolStripMenuItemEdit";
            this.toolStripMenuItemEdit.Size = new System.Drawing.Size(126, 22);
            this.toolStripMenuItemEdit.Text = "Edit notes";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(123, 6);
            // 
            // toolStripMenuItemDelete
            // 
            this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            this.toolStripMenuItemDelete.Size = new System.Drawing.Size(126, 22);
            this.toolStripMenuItemDelete.Text = "Delete";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(266, 216);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.propertyGridGameDetails);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(258, 190);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Details";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propertyGridGameDetails
            // 
            this.propertyGridGameDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridGameDetails.HelpVisible = false;
            this.propertyGridGameDetails.Location = new System.Drawing.Point(3, 3);
            this.propertyGridGameDetails.Name = "propertyGridGameDetails";
            this.propertyGridGameDetails.Size = new System.Drawing.Size(252, 184);
            this.propertyGridGameDetails.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pictureBoxScreenshot);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(258, 190);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Screenshot";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pictureBoxScreenshot
            // 
            this.pictureBoxScreenshot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxScreenshot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxScreenshot.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxScreenshot.Name = "pictureBoxScreenshot";
            this.pictureBoxScreenshot.Size = new System.Drawing.Size(252, 184);
            this.pictureBoxScreenshot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxScreenshot.TabIndex = 0;
            this.pictureBoxScreenshot.TabStop = false;
            this.toolTipHelp.SetToolTip(this.pictureBoxScreenshot, "Double clic to open");
            this.pictureBoxScreenshot.DoubleClick += new System.EventHandler(this.pictureBoxScreenshot_DoubleClick);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.Location = new System.Drawing.Point(0, 251);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(266, 23);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "Delete Selected Saved Game";
            this.buttonDelete.UseVisualStyleBackColor = true;
            // 
            // buttonRestore
            // 
            this.buttonRestore.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRestore.Location = new System.Drawing.Point(0, 222);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(266, 23);
            this.buttonRestore.TabIndex = 1;
            this.buttonRestore.Text = "Restore Selected Saved Game";
            this.toolTipHelp.SetToolTip(this.buttonRestore, "Before Restoring, be on the Main Title screen of Doom");
            this.buttonRestore.UseVisualStyleBackColor = true;
            // 
            // groupBoxConfig
            // 
            this.groupBoxConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxConfig.Controls.Add(this.checkBoxScreenShot);
            this.groupBoxConfig.Controls.Add(this.buttonBackupNow);
            this.groupBoxConfig.Controls.Add(this.checkBoxIncludeDeath);
            this.groupBoxConfig.Controls.Add(this.checkBoxAutoBackup);
            this.groupBoxConfig.Location = new System.Drawing.Point(3, 3);
            this.groupBoxConfig.Name = "groupBoxConfig";
            this.groupBoxConfig.Size = new System.Drawing.Size(667, 68);
            this.groupBoxConfig.TabIndex = 3;
            this.groupBoxConfig.TabStop = false;
            this.groupBoxConfig.Text = "Slot Configuration";
            // 
            // checkBoxScreenShot
            // 
            this.checkBoxScreenShot.AutoSize = true;
            this.checkBoxScreenShot.Enabled = false;
            this.checkBoxScreenShot.Location = new System.Drawing.Point(197, 19);
            this.checkBoxScreenShot.Name = "checkBoxScreenShot";
            this.checkBoxScreenShot.Size = new System.Drawing.Size(80, 17);
            this.checkBoxScreenShot.TabIndex = 3;
            this.checkBoxScreenShot.Text = "Screenshot";
            this.toolTipHelp.SetToolTip(this.checkBoxScreenShot, "Work only if the game is set to Windowed mode");
            this.checkBoxScreenShot.UseVisualStyleBackColor = true;
            // 
            // buttonBackupNow
            // 
            this.buttonBackupNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBackupNow.Location = new System.Drawing.Point(538, 19);
            this.buttonBackupNow.Name = "buttonBackupNow";
            this.buttonBackupNow.Size = new System.Drawing.Size(123, 40);
            this.buttonBackupNow.TabIndex = 2;
            this.buttonBackupNow.Text = "Backup Now";
            this.buttonBackupNow.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeDeath
            // 
            this.checkBoxIncludeDeath.AutoSize = true;
            this.checkBoxIncludeDeath.Enabled = false;
            this.checkBoxIncludeDeath.Location = new System.Drawing.Point(100, 19);
            this.checkBoxIncludeDeath.Name = "checkBoxIncludeDeath";
            this.checkBoxIncludeDeath.Size = new System.Drawing.Size(91, 17);
            this.checkBoxIncludeDeath.TabIndex = 1;
            this.checkBoxIncludeDeath.Text = "Include death";
            this.toolTipHelp.SetToolTip(this.checkBoxIncludeDeath, "Also do a backup on death checkpoint");
            this.checkBoxIncludeDeath.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoBackup
            // 
            this.checkBoxAutoBackup.AutoSize = true;
            this.checkBoxAutoBackup.Location = new System.Drawing.Point(6, 19);
            this.checkBoxAutoBackup.Name = "checkBoxAutoBackup";
            this.checkBoxAutoBackup.Size = new System.Drawing.Size(88, 17);
            this.checkBoxAutoBackup.TabIndex = 0;
            this.checkBoxAutoBackup.Text = "Auto Backup";
            this.toolTipHelp.SetToolTip(this.checkBoxAutoBackup, "Backup automatically as soon as files changes");
            this.checkBoxAutoBackup.UseVisualStyleBackColor = true;
            // 
            // toolTipHelp
            // 
            this.toolTipHelp.AutoPopDelay = 20000;
            this.toolTipHelp.InitialDelay = 100;
            this.toolTipHelp.ReshowDelay = 10;
            this.toolTipHelp.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipHelp.ToolTipTitle = "Help";
            // 
            // SlotControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxConfig);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.labelMaps);
            this.Controls.Add(this.comboBoxMaps);
            this.Name = "SlotControl";
            this.Size = new System.Drawing.Size(670, 378);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.contextMenuStripListView.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenshot)).EndInit();
            this.groupBoxConfig.ResumeLayout(false);
            this.groupBoxConfig.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ComboBox comboBoxMaps;
        public System.Windows.Forms.Label labelMaps;
        public System.Windows.Forms.SplitContainer splitContainer;
        public System.Windows.Forms.ListView listViewSavedGames;
        public System.Windows.Forms.Button buttonRestore;
        public System.Windows.Forms.ColumnHeader columnHeaderLastSave;
        public System.Windows.Forms.ColumnHeader columnHeaderDiff;
        public System.Windows.Forms.ColumnHeader columnHeaderPlayedTime;
        public System.Windows.Forms.GroupBox groupBoxConfig;
        public System.Windows.Forms.Button buttonBackupNow;
        public System.Windows.Forms.CheckBox checkBoxIncludeDeath;
        public System.Windows.Forms.CheckBox checkBoxAutoBackup;
        private System.Windows.Forms.ColumnHeader columnHeaderIsDeath;
        public System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.ToolTip toolTipHelp;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        public System.Windows.Forms.PropertyGrid propertyGridGameDetails;
        private System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.PictureBox pictureBoxScreenshot;
        public System.Windows.Forms.CheckBox checkBoxScreenShot;
        private System.Windows.Forms.ColumnHeader columnHeaderNotes;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        public System.Windows.Forms.ContextMenuStrip contextMenuStripListView;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRestore;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEdit;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;
    }
}
