using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Hg.DoomHistory.Properties;

namespace Hg.DoomHistory
{
    public partial class FormMain : Form
    {
        private readonly Version _version;
        private string _backupFolder;

        private FormDebugConsole _debugConsole;
        private MessageMode _notificationMode;
        private string _savedGameFolder;
        private ScreenshotQuality _screenshotQuality;

        private SlotManager _slot1;
        private SlotManager _slot2;
        private SlotManager _slot3;

        public FormMain()
        {
            InitializeComponent();
            _version = Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void LoadSettings()
        {
            _savedGameFolder = Settings.Default.SavedGameFolder;
            _backupFolder = Settings.Default.BackupFolder;
            _notificationMode = (MessageMode) Settings.Default.NotificationMode;
            _screenshotQuality = (ScreenshotQuality) Settings.Default.ScreenshotQuality;

            if (_notificationMode == MessageMode.None)
                _notificationMode = MessageMode.MessageBox;

            if (_screenshotQuality == ScreenshotQuality.None)
                _screenshotQuality = ScreenshotQuality.Jpg;

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
            Settings.Default.ScreenshotQuality = (int) _screenshotQuality;

            Settings.Default.Save();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string versionFormatted = $"v{_version.Major}.{_version.Minor}.{_version.Build}";
            Text += @" " + versionFormatted;

            Init();
        }

        private void Init()
        {
            LoadSettings();

            buttonBackupFolderOpen.Enabled = IsBackupFolderValid();

            messageBoxToolStripMenuItem.Checked = _notificationMode == MessageMode.MessageBox;
            statusBarToolStripMenuItem.Checked = _notificationMode == MessageMode.Status;

            giflowSizeToolStripMenuItem.Checked = _screenshotQuality == ScreenshotQuality.Gif;
            jpgmediumToolStripMenuItem.Checked = _screenshotQuality == ScreenshotQuality.Jpg;
            pnghugeSizeToolStripMenuItem.Checked = _screenshotQuality == ScreenshotQuality.Png;

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

            SetScreenshotQuality();
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
            catch (Exception ex)
            {
                Logger.Log("ButtonBackupFolderOpen_Click: Exception: " + ex.Message, LogLevel.Debug);
                Logger.LogException(ex);
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

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string responseString = null;
            try
            {
                // We'll get all release for now and change this to latest when a proper release is made
                HttpWebRequest request =
                    (HttpWebRequest) WebRequest.Create("https://api.github.com/repos/HgAlexx/Hg.DoomHistory/releases");
                request.UserAgent = "HgAlexx/Hg.DoomHistory";

                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream stream = response.GetResponseStream();
                    if (stream != null)
                        responseString = new StreamReader(stream).ReadToEnd();
                }
            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(responseString))
            {
                Version maxVesion = _version;
                try
                {
                    // Fast and ugly way to parse the json response from the github api.. just to not add one dependency to the project
                    Regex regex = new Regex(@"""tag_name"":""v([0-9.]+)"",");
                    Match match = regex.Match(responseString);

                    while (match.Success)
                    {
                        Version v = new Version(match.Groups[1].Value);
                        if (v > _version)
                            if (v > maxVesion)
                                maxVesion = v;
                        match = match.NextMatch();
                    }
                }
                catch (Exception)
                {
                }

                if (maxVesion > _version)
                {
                    if (Message("A new version is available, do you want to open the release page?",
                            "New version available!", MessageType.Question, MessageMode.MessageBox) == DialogResult.Yes)
                    {
                        Process.Start("https://github.com/HgAlexx/Hg.DoomHistory/releases");
                    }
                }
                else
                {
                    Message(@"No new version found", @"You are up-to-date", MessageType.Information, MessageMode.User);
                }
            }
            else
            {
                Message(@"Unable to check for a new version, please try again later", @"Hmm :(",
                    MessageType.Information, MessageMode.User);
            }
        }

        private void giflowSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _screenshotQuality = ScreenshotQuality.Gif;
            giflowSizeToolStripMenuItem.Checked = true;
            jpgmediumToolStripMenuItem.Checked = false;
            pnghugeSizeToolStripMenuItem.Checked = false;
            SetScreenshotQuality();
        }

        private void jpgmediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _screenshotQuality = ScreenshotQuality.Jpg;
            giflowSizeToolStripMenuItem.Checked = false;
            jpgmediumToolStripMenuItem.Checked = true;
            pnghugeSizeToolStripMenuItem.Checked = false;
            SetScreenshotQuality();
        }

        private void pnghugeSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _screenshotQuality = ScreenshotQuality.Png;
            giflowSizeToolStripMenuItem.Checked = false;
            jpgmediumToolStripMenuItem.Checked = false;
            pnghugeSizeToolStripMenuItem.Checked = true;
            SetScreenshotQuality();
        }

        private void SetScreenshotQuality()
        {
            _slot1?.SetScreenshotQuality(_screenshotQuality);
            _slot2?.SetScreenshotQuality(_screenshotQuality);
            _slot3?.SetScreenshotQuality(_screenshotQuality);
        }

        private void debugConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_debugConsole == null)
            {
                _debugConsole = new FormDebugConsole();
            }

            _debugConsole.Show(this);
        }
    }
}