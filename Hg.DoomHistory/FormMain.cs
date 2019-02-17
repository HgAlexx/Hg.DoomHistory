using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Hg.DoomHistory.Properties;

namespace Hg.DoomHistory
{
    public partial class FormMain : Form
    {
        private string _backupFolder;
        private MessageMode _notificationMode;
        private string _savedGameFolder;

        private SlotManager _slot1;
        private SlotManager _slot2;
        private SlotManager _slot3;

        public FormMain()
        {
            InitializeComponent();
        }

        private void LoadSettings()
        {
            Settings.Default.Upgrade();

            _savedGameFolder = Settings.Default.SavedGameFolder;
            _backupFolder = Settings.Default.BackupFolder;
            _notificationMode = (MessageMode) Settings.Default.NotificationMode;
            if (_notificationMode == MessageMode.None)
                _notificationMode = MessageMode.MessageBox;

            if (!Directory.Exists(_savedGameFolder))
            {
                _savedGameFolder = "";
                Settings.Default.SavedGameFolder = "";
                Settings.Default.Save();
            }

            if (!Directory.Exists(_backupFolder))
            {
                _backupFolder = "";
                Settings.Default.BackupFolder = "";
                Settings.Default.Save();
            }

            SetFolderTextBox(textBoxBackupFolder, _backupFolder);
            SetFolderTextBox(textBoxSavedGamesFolder, _savedGameFolder);
        }

        private void SaveSettings()
        {
            Settings.Default.BackupFolder = _backupFolder;
            Settings.Default.SavedGameFolder = _savedGameFolder;
            Settings.Default.NotificationMode = (int) _notificationMode;

            Settings.Default.Save();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            LoadSettings();
            buttonBackupFolderOpen.Enabled = IsBackupFolderValid();
            messageBoxToolStripMenuItem.Checked = _notificationMode == MessageMode.MessageBox;
            statusBarToolStripMenuItem.Checked = _notificationMode == MessageMode.Status;
            Message("Ready", "", MessageType.Information, MessageMode.Status);
            CreateSlots();
        }

        private void Release()
        {
            ReleaseSlots();
            SaveSettings();
        }

        private DialogResult Message(string text, string caption, MessageType type, MessageMode mode)
        {
            DialogResult dialogResult = DialogResult.None;
            if (InvokeRequired)
            {
                Invoke(new Action(() => { dialogResult = Message(text, caption, type, mode); }));
            }
            else
            {
                if (mode == MessageMode.User && _notificationMode == MessageMode.MessageBox ||
                    mode == MessageMode.MessageBox)
                {
                    if (type != MessageType.None)
                    {
                        MessageBoxButtons button = MessageBoxButtons.OK;
                        MessageBoxIcon icon = MessageBoxIcon.Information;
                        switch (type)
                        {
                            case MessageType.Error:
                                button = MessageBoxButtons.OK;
                                icon = MessageBoxIcon.Error;
                                break;
                            case MessageType.Information:
                                button = MessageBoxButtons.OK;
                                icon = MessageBoxIcon.Information;
                                break;
                            case MessageType.Question:
                                button = MessageBoxButtons.YesNoCancel;
                                icon = MessageBoxIcon.Question;
                                break;
                            case MessageType.Warning:
                                button = MessageBoxButtons.OK;
                                icon = MessageBoxIcon.Warning;
                                break;
                        }

                        dialogResult = MessageBox.Show(text, caption, button, icon);
                    }
                }

                toolStripStatus.Image = null;
                if (mode == MessageMode.User && _notificationMode == MessageMode.Status || mode == MessageMode.Status)
                {
                    switch (type)
                    {
                        case MessageType.Error:
                            toolStripStatus.Image = imageListMessageType.Images[0];
                            break;
                        case MessageType.Information:
                            toolStripStatus.Image = imageListMessageType.Images[1];
                            break;
                        case MessageType.Question:
                            toolStripStatus.Image = imageListMessageType.Images[2];
                            break;
                        case MessageType.Warning:
                            toolStripStatus.Image = imageListMessageType.Images[3];
                            break;
                    }

                    toolStripStatus.Text = text;
                }
            }

            return dialogResult;
        }

        private void CreateSlots()
        {
            ReleaseSlots();

            if (!Directory.Exists(_savedGameFolder) || !Directory.Exists(_backupFolder))
            {
                Message("Folders not set", "", MessageType.Information, MessageMode.Status);
                return;
            }

            // Load stuff
            _slot1 = new SlotManager(slotControl1, 1, _savedGameFolder, _backupFolder, Message);
            _slot2 = new SlotManager(slotControl2, 2, _savedGameFolder, _backupFolder, Message);
            _slot3 = new SlotManager(slotControl3, 3, _savedGameFolder, _backupFolder, Message);
        }

        private void ReleaseSlots()
        {
            _slot1?.Release();
            _slot2?.Release();
            _slot3?.Release();

            _slot1 = null;
            _slot2 = null;
            _slot3 = null;
        }

        private bool IsBackupFolderValid()
        {
            return !string.IsNullOrEmpty(_backupFolder) && Directory.Exists(_backupFolder);
        }

        private void ButtonSavedGamesBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_savedGameFolder))
                folderBrowserDialogSavedGames.SelectedPath = _savedGameFolder;

            if (folderBrowserDialogSavedGames.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialogSavedGames.SelectedPath))
                {
                    SetSavedGameFolder(folderBrowserDialogSavedGames.SelectedPath);
                }
            }
        }

        private static void SetFolderTextBox(TextBox textBox, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                textBox.Text = "";
                return;
            }

            int partLength = 30;
            if (value.Length < partLength * 2 + 5)
            {
                textBox.Text = value;
                return;
            }

            string partialValue = value.Substring(0, partLength) + " ... " + value.Substring(value.Length - partLength);
            textBox.Text = partialValue;
        }

        private void SetSavedGameFolder(string path)
        {
            _savedGameFolder = path;
            SetFolderTextBox(textBoxSavedGamesFolder, _savedGameFolder);

            Settings.Default.SavedGameFolder = _savedGameFolder;
            Settings.Default.Save();

            CreateSlots();
        }

        private void ButtonBackupBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_backupFolder))
                folderBrowserDialogBackup.SelectedPath = _backupFolder;

            if (folderBrowserDialogBackup.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialogBackup.SelectedPath))
                {
                    _backupFolder = folderBrowserDialogBackup.SelectedPath;
                    SetFolderTextBox(textBoxBackupFolder, _backupFolder);

                    Message("", "", MessageType.None, MessageMode.Status);

                    Settings.Default.BackupFolder = _backupFolder;
                    Settings.Default.Save();

                    CreateSlots();
                }
            }

            buttonBackupFolderOpen.Enabled = IsBackupFolderValid();
        }

        private void ButtonAutoDetect_Click(object sender, EventArgs e)
        {
            string path =
                Environment.ExpandEnvironmentVariables(
                    @"%USERPROFILE%\Saved Games\id Software\DOOM\base\savegame.user");
            if (!Directory.Exists(path))
            {
                Message(@"Folder not found, be sure to play Doom at least once first!", @"Hmm :(", MessageType.Warning,
                    MessageMode.User);
                return;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            Regex regex = new Regex(@"^\d+$");
            int count = 0;

            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                if (regex.IsMatch(directory.Name))
                {
                    count++;
                    DialogResult result = Message(@"Is this path correct ?" + Environment.NewLine + directory.FullName,
                        @"Confirmation", MessageType.Question, MessageMode.MessageBox);
                    if (result == DialogResult.Yes)
                    {
                        SetSavedGameFolder(directory.FullName);
                        Message(@"Saved game folder set successfully", @"Auto Detect", MessageType.Information,
                            MessageMode.User);
                        return;
                    }

                    if (result == DialogResult.Cancel)
                    {
                        Message(@"", @"", MessageType.None, MessageMode.None);
                        return;
                    }
                }
            }

            if (count > 0)
                Message(@"No folder selected", @"Auto Detect", MessageType.Information, MessageMode.User);
        }

        private void ButtonBackupFolderOpen_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_backupFolder))
                return;
            if (!Directory.Exists(_backupFolder))
                return;
            try
            {
                Process.Start(_backupFolder);
            }
            catch (Exception)
            {
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Release();
        }

        private void clearSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Message("Are you sure you want to reset all global settings?", "Reset global settings?",
                    MessageType.Question, MessageMode.MessageBox) == DialogResult.Yes)
            {
                _backupFolder = "";
                _savedGameFolder = "";
                _notificationMode = MessageMode.MessageBox;
                Release();
                Init();
            }
        }

        private void messageBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _notificationMode = MessageMode.MessageBox;
            messageBoxToolStripMenuItem.Checked = _notificationMode == MessageMode.MessageBox;
            statusBarToolStripMenuItem.Checked = _notificationMode == MessageMode.Status;
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _notificationMode = MessageMode.Status;
            messageBoxToolStripMenuItem.Checked = _notificationMode == MessageMode.MessageBox;
            statusBarToolStripMenuItem.Checked = _notificationMode == MessageMode.Status;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog(this);
        }

        private void importExistingBackupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormImport import = new FormImport(_backupFolder);
            import.ShowDialog(this);

            if (import.Imported > 0)
            {
                Release();
                Init();
            }
        }
    }
}
