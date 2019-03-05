using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Hg.DoomHistory.Managers;
using Hg.DoomHistory.Properties;
using Hg.DoomHistory.Types;
using Hg.DoomHistory.Utilities;

namespace Hg.DoomHistory.Forms
{
    public partial class FormMain : Form
    {
        #region Fields & Properties

        private readonly SettingManager _settingManager;
        private readonly Version _version;

        private FormDebugConsole _debugConsole;

        private HotKeysManager _hotKeysManager;

        private SlotManager _slot1;
        private SlotManager _slot2;
        private SlotManager _slot3;

        #endregion

        #region Members

        public FormMain()
        {
            InitializeComponent();
            _version = Assembly.GetExecutingAssembly().GetName().Version;

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            _settingManager = new SettingManager();
            _settingManager.BackupFolderChanged += () =>
            {
                SetFolderTextBox(textBoxBackupFolder, _settingManager.BackupFolder);
                buttonBackupFolderOpen.Enabled = IsBackupFolderValid();
            };
            _settingManager.SavedGameFolderChanged += () =>
            {
                SetFolderTextBox(textBoxSavedGamesFolder, _settingManager.SavedGameFolder);
            };
            _settingManager.NotificationModeChanged += () =>
            {
                messageBoxToolStripMenuItem.Checked = _settingManager.NotificationMode == MessageMode.MessageBox;
                statusBarToolStripMenuItem.Checked = _settingManager.NotificationMode == MessageMode.Status;
            };
            _settingManager.ScreenshotQualityChanged += () =>
            {
                giflowSizeToolStripMenuItem.Checked = _settingManager.ScreenshotQuality == ScreenshotQuality.Gif;
                jpgmediumToolStripMenuItem.Checked = _settingManager.ScreenshotQuality == ScreenshotQuality.Jpg;
                pnghugeSizeToolStripMenuItem.Checked = _settingManager.ScreenshotQuality == ScreenshotQuality.Png;
            };
            _settingManager.TimeStampSortOrderChanged += () =>
            {
                ascendingToolStripMenuItem.Checked = _settingManager.TimeStampSortOrder == SortOrder.Ascending;
                descendingToolStripMenuItem.Checked = _settingManager.TimeStampSortOrder == SortOrder.Descending;
            };
            _settingManager.HotKeysActiveChanged += () =>
            {
                activeToolStripMenuItem.Checked = _settingManager.HotKeysActive;
                if (_settingManager.HotKeysActive)
                {
                    _hotKeysManager?.Hook();
                }
                else
                {
                    _hotKeysManager?.UnHook();
                }
            };
            _settingManager.HotKeysSoundChanged += () =>
            {
                soundToolStripMenuItem.Checked = _settingManager.HotKeysSound;
            };
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog(this);
        }

        private void activeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingManager.HotKeysActive = !_settingManager.HotKeysActive;
            activeToolStripMenuItem.Checked = _settingManager.HotKeysActive;
        }

        private void ascendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingManager.TimeStampSortOrder = SortOrder.Ascending;
            ascendingToolStripMenuItem.Checked = true;
            descendingToolStripMenuItem.Checked = false;
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
            {
                Message(@"No folder selected", @"Auto Detect", MessageType.Information, MessageMode.User);
            }
        }

        private void ButtonBackupBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_settingManager.BackupFolder))
            {
                folderBrowserDialogBackup.SelectedPath = _settingManager.BackupFolder;
            }

            if (folderBrowserDialogBackup.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialogBackup.SelectedPath))
                {
                    _settingManager.BackupFolder = folderBrowserDialogBackup.SelectedPath;
                    SetFolderTextBox(textBoxBackupFolder, _settingManager.BackupFolder);

                    Message("", "", MessageType.None, MessageMode.Status);

                    Settings.Default.BackupFolder = _settingManager.BackupFolder;
                    Settings.Default.Save();

                    CreateSlots();
                }
            }

            buttonBackupFolderOpen.Enabled = IsBackupFolderValid();
        }

        private void ButtonBackupFolderOpen_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_settingManager.BackupFolder))
            {
                return;
            }

            if (!Directory.Exists(_settingManager.BackupFolder))
            {
                return;
            }

            try
            {
                Process.Start(_settingManager.BackupFolder);
            }
            catch (Exception ex)
            {
                Logger.Log("ButtonBackupFolderOpen_Click: Exception: " + ex.Message, LogLevel.Debug);
                Logger.LogException(ex);
            }
        }

        private void ButtonSavedGamesBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_settingManager.SavedGameFolder))
            {
                folderBrowserDialogSavedGames.SelectedPath = _settingManager.SavedGameFolder;
            }

            if (folderBrowserDialogSavedGames.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialogSavedGames.SelectedPath))
                {
                    SetSavedGameFolder(folderBrowserDialogSavedGames.SelectedPath);
                }
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string responseString = null;
            string versionFormatted = $"{_version.Major}.{_version.Minor}.{_version.Build}";
            try
            {
                // We'll get all release for now and change this to latest when a proper release is made
                HttpWebRequest request =
                    (HttpWebRequest) WebRequest.Create("https://api.github.com/repos/HgAlexx/Hg.DoomHistory/releases");
                request.UserAgent = "Hg.DoomHistory/" + versionFormatted + " (" + Environment.OSVersion + ") " +
                                    "By: HgAlexx";

                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        responseString = new StreamReader(stream).ReadToEnd();
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(responseString))
            {
                Version maxVersion = _version;
                try
                {
                    // Fast and ugly way to parse the json response from the github api.. just to not add one dependency to the project
                    Regex regex = new Regex(@"""tag_name"":""v([0-9.]+)"",");
                    Match match = regex.Match(responseString);

                    while (match.Success)
                    {
                        Version v = new Version(match.Groups[1].Value);
                        if (v > _version)
                        {
                            if (v > maxVersion)
                            {
                                maxVersion = v;
                            }
                        }

                        match = match.NextMatch();
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                }

                if (maxVersion > _version)
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

        private void clearSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Message("Are you sure you want to reset all global settings?", "Reset global settings?",
                    MessageType.Question, MessageMode.MessageBox) == DialogResult.Yes)
            {
                Release();
                _settingManager.ResetSettings();
                _settingManager.SaveSettings();
                Init();
            }
        }

        private void CreateHotKeysHook()
        {
            _hotKeysManager = new HotKeysManager();
            _hotKeysManager.KeyDown += OnKeyDown;
            _hotKeysManager.KeyUp += OnKeyUp;
        }

        private void CreateSlots()
        {
            ReleaseSlots();

            if (!Directory.Exists(_settingManager.SavedGameFolder) || !Directory.Exists(_settingManager.BackupFolder))
            {
                Message("Folders not set", "", MessageType.Information, MessageMode.Status);
                return;
            }

            // Load stuff
            _slot1 = new SlotManager(slotControl1, 1, _settingManager, Message);
            _slot2 = new SlotManager(slotControl2, 2, _settingManager, Message);
            _slot3 = new SlotManager(slotControl3, 3, _settingManager, Message);
        }

        private void debugConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_debugConsole == null)
            {
                _debugConsole = new FormDebugConsole();
                _debugConsole.Show(this);
            }

            if (!_debugConsole.Visible)
            {
                _debugConsole.Visible = true;
            }
        }

        private void descendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingManager.TimeStampSortOrder = SortOrder.Descending;
            ascendingToolStripMenuItem.Checked = false;
            descendingToolStripMenuItem.Checked = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Release();
            SaveSettings();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string versionFormatted = $"v{_version.Major}.{_version.Minor}.{_version.Build}";
            Text += @" " + versionFormatted;

            CreateHotKeysHook();

            LoadSettings();

            Init();

            SoundManager.PreLoad();
        }

        private void giflowSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingManager.ScreenshotQuality = ScreenshotQuality.Gif;
            giflowSizeToolStripMenuItem.Checked = true;
            jpgmediumToolStripMenuItem.Checked = false;
            pnghugeSizeToolStripMenuItem.Checked = false;
        }

        private void importExistingBackupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormImport import = new FormImport(_settingManager.BackupFolder);
            import.ShowDialog(this);

            if (import.Imported > 0)
            {
                Release();
                Init();
            }
        }

        private void Init()
        {
            Message("Ready", "", MessageType.Information, MessageMode.Status);

            CreateSlots();

            InitHotKeys();
        }

        private void InitHotKeys()
        {
            if (_hotKeysManager == null || _settingManager == null)
            {
                return;
            }

            _hotKeysManager.HotKeys.Clear();
            foreach (HotKeyToAction hotKeyToAction in _settingManager.HotKeyToActions)
            {
                _hotKeysManager.HotKeys.Add(hotKeyToAction);
            }
        }

        private bool IsBackupFolderValid()
        {
            return !string.IsNullOrEmpty(_settingManager.BackupFolder) &&
                   Directory.Exists(_settingManager.BackupFolder);
        }

        private void jpgmediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingManager.ScreenshotQuality = ScreenshotQuality.Jpg;
            giflowSizeToolStripMenuItem.Checked = false;
            jpgmediumToolStripMenuItem.Checked = true;
            pnghugeSizeToolStripMenuItem.Checked = false;
        }

        private void LoadSettings()
        {
            _settingManager.LoadSettings();
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
                if (mode == MessageMode.User && _settingManager.NotificationMode == MessageMode.MessageBox ||
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
                if (mode == MessageMode.User && _settingManager.NotificationMode == MessageMode.Status ||
                    mode == MessageMode.Status)
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

        private void messageBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingManager.NotificationMode = MessageMode.MessageBox;
            messageBoxToolStripMenuItem.Checked = _settingManager.NotificationMode == MessageMode.MessageBox;
            statusBarToolStripMenuItem.Checked = _settingManager.NotificationMode == MessageMode.Status;
        }

        private void OnKeyDown(object sender, KeyEventArgs e, HotKeyToAction hotKeyToAction)
        {
            Logger.Log(_hotKeysManager.GetCurrentStates() + ", " + e.KeyCode, LogLevel.Debug);

            if (!_settingManager.HotKeysActive)
            {
                return;
            }

            SlotManager slotManager = null;
            if (tabControl1.SelectedTab == tabPageSlot1)
            {
                slotManager = _slot1;
            }

            if (tabControl1.SelectedTab == tabPageSlot2)
            {
                slotManager = _slot2;
            }

            if (tabControl1.SelectedTab == tabPageSlot3)
            {
                slotManager = _slot3;
            }

            if (slotManager != null)
            {
                switch (hotKeyToAction.Action)
                {
                    case HotKeyAction.MapPrevious:
                        slotManager.MapSelectPrevious();
                        e.Handled = true;
                        break;
                    case HotKeyAction.MapNext:
                        slotManager.MapSelectNext();
                        e.Handled = true;
                        break;
                    case HotKeyAction.SaveFirst:
                        slotManager.SaveSelectFirst();
                        e.Handled = true;
                        break;
                    case HotKeyAction.SaveLast:
                        slotManager.SaveSelectLast();
                        e.Handled = true;
                        break;
                    case HotKeyAction.SavePrevious:
                        slotManager.SaveSelectPrevious();
                        e.Handled = true;
                        break;
                    case HotKeyAction.SaveNext:
                        slotManager.SaveSelectNext();
                        e.Handled = true;
                        break;
                    case HotKeyAction.SaveRestore:
                        if (slotManager.ActionSaveRestore())
                        {
                            if (_settingManager.HotKeysSound)
                            {
                                SoundManager.PlaySuccess();
                            }
                        }
                        else
                        {
                            if (_settingManager.HotKeysSound)
                            {
                                SoundManager.PlayError();
                            }
                        }

                        e.Handled = true;
                        break;
                }
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e, HotKeyToAction hotKeyToAction)
        {
            switch (hotKeyToAction.Action)
            {
                case HotKeyAction.MapPrevious:
                case HotKeyAction.MapNext:
                case HotKeyAction.SaveFirst:
                case HotKeyAction.SaveLast:
                case HotKeyAction.SavePrevious:
                case HotKeyAction.SaveNext:
                case HotKeyAction.SaveRestore:
                    e.Handled = true;
                    break;
            }
        }

        private void pnghugeSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingManager.ScreenshotQuality = ScreenshotQuality.Png;
            giflowSizeToolStripMenuItem.Checked = false;
            jpgmediumToolStripMenuItem.Checked = false;
            pnghugeSizeToolStripMenuItem.Checked = true;
        }

        private void Release()
        {
            _hotKeysManager?.UnHook();
            _hotKeysManager?.Dispose();
            _hotKeysManager = null;

            ReleaseSlots();
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

        private void SaveSettings()
        {
            _settingManager.SaveSettings();
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

        private void setKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettingsHotKeys formSettingsHotKeys = new FormSettingsHotKeys(_settingManager.HotKeyToActions);
            if (formSettingsHotKeys.ShowDialog(this) == DialogResult.OK)
            {
                InitHotKeys();
            }
        }

        private void SetSavedGameFolder(string path)
        {
            _settingManager.SavedGameFolder = path;
            SetFolderTextBox(textBoxSavedGamesFolder, _settingManager.SavedGameFolder);

            CreateSlots();
        }

        private void soundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingManager.HotKeysSound = !_settingManager.HotKeysSound;
            soundToolStripMenuItem.Checked = _settingManager.HotKeysSound;
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingManager.NotificationMode = MessageMode.Status;
            messageBoxToolStripMenuItem.Checked = _settingManager.NotificationMode == MessageMode.MessageBox;
            statusBarToolStripMenuItem.Checked = _settingManager.NotificationMode == MessageMode.Status;
        }

        #endregion
    }
}