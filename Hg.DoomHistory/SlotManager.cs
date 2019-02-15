using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Hg.DoomHistory
{
    public enum MessageMode
    {
        None,
        User,
        MessageBox,
        Status
    }

    public delegate DialogResult MessageEventHandler(string text, string caption, MessageType type, MessageMode mode);

    public class SlotManager
    {
        private const string ScreenShotExtension = ".jpg";
        private static readonly ImageFormat ScreenShotFormat = ImageFormat.Jpeg;

        private readonly string _backupFolder;
        private readonly int _id;

        private readonly MapComparer _mapComparer = new MapComparer();

        private readonly List<MapData> _maps = new List<MapData>();
        private readonly string _pathSlot;
        private readonly string _savedGameFolder;
        private readonly TimeStampComparer _timeStampComparer = new TimeStampComparer();

        private bool _autoBackup;

        private FileSystemWatcher _fileSystemWatcher;
        private bool _includeDeath;

        private bool _isCheckPoint;
        private bool _isCheckPointAlt;
        private bool _screenshot;

        private SlotControl _slot;

        private bool _watcherActive;

        public SlotManager(SlotControl slot, int id, string savedGameFolder, string backupFolder,
            MessageEventHandler messageEventHandler)
        {
            OnMessage += messageEventHandler;

            _slot = slot;

            _id = id;
            _backupFolder = backupFolder;
            _savedGameFolder = savedGameFolder;
            _pathSlot = Path.Combine(_backupFolder, "Slot" + _id);

            SlotName = "Slot " + _id;

            UnloadControl();

            BindEvent();

            ScanBackupFolder();

            LoadControl();
        }

        public string SlotName { get; }

        public event MessageEventHandler OnMessage;

        private DialogResult Message(string text, string caption, MessageType type, MessageMode mode)
        {
            string msg = SlotName + ": " + text;
            if (OnMessage != null)
            {
                return OnMessage(msg, caption, type, mode);
            }

            return DialogResult.None;
        }

        public void Release()
        {
            UnbindEvent();
            UnloadControl();
            _maps.Clear();
            _slot = null;
        }

        private void UnbindEvent()
        {
            _slot.checkBoxAutoBackup.CheckedChanged -= CheckBoxAutoBackupOnCheckedChanged;
            _slot.checkBoxIncludeDeath.CheckedChanged -= CheckBoxIncludeDeathOnCheckedChanged;
            _slot.checkBoxScreenShot.CheckedChanged -= CheckBoxScreenShotOnCheckedChanged;

            _slot.comboBoxMaps.SelectionChangeCommitted -= ComboBoxMapsOnSelectionChangeCommitted;

            _slot.listViewSavedGames.SelectedIndexChanged -= ListViewSavedGamesOnSelectedIndexChanged;
            _slot.listViewSavedGames.DoubleClick -= ListViewSavedGamesOnDoubleClick;

            _slot.contextMenuStripListView.ItemClicked -= ContextMenuStripListViewOnItemClicked;

            _slot.buttonBackupNow.Click -= ButtonBackupNowOnClick;
            _slot.buttonRestore.Click -= ButtonRestoreOnClick;
            _slot.buttonDelete.Click -= ButtonDeleteOnClick;

            _slot.propertyGridGameDetails.PropertyValueChanged -= PropertyGridGameDetailsOnPropertyValueChanged;
        }

        private void BindEvent()
        {
            _slot.checkBoxAutoBackup.CheckedChanged += CheckBoxAutoBackupOnCheckedChanged;
            _slot.checkBoxIncludeDeath.CheckedChanged += CheckBoxIncludeDeathOnCheckedChanged;
            _slot.checkBoxScreenShot.CheckedChanged += CheckBoxScreenShotOnCheckedChanged;

            _slot.comboBoxMaps.SelectionChangeCommitted += ComboBoxMapsOnSelectionChangeCommitted;

            _slot.listViewSavedGames.SelectedIndexChanged += ListViewSavedGamesOnSelectedIndexChanged;
            _slot.listViewSavedGames.DoubleClick += ListViewSavedGamesOnDoubleClick;

            _slot.contextMenuStripListView.ItemClicked += ContextMenuStripListViewOnItemClicked;

            _slot.buttonBackupNow.Click += ButtonBackupNowOnClick;
            _slot.buttonRestore.Click += ButtonRestoreOnClick;
            _slot.buttonDelete.Click += ButtonDeleteOnClick;

            _slot.propertyGridGameDetails.PropertyValueChanged += PropertyGridGameDetailsOnPropertyValueChanged;
        }

        private void ListViewSavedGamesOnDoubleClick(object sender, EventArgs e)
        {
            GameDetails gameDetails = GetSelectedGameDetails();
            if (gameDetails != null)
            {
                FormNotes formNotes = new FormNotes {textBoxNotes = {Text = gameDetails.Notes}};
                if (formNotes.ShowDialog(_slot) == DialogResult.OK)
                {
                    gameDetails.Notes = formNotes.textBoxNotes.Text;
                    MapData mapData = GetSelectedMap();
                    if (mapData != null)
                    {
                        RefreshLisView(mapData);
                    }
                }
            }
        }

        private void ContextMenuStripListViewOnItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            GameDetails gameDetails = GetSelectedGameDetails();
            if (gameDetails != null)
            {
                if (e.ClickedItem == _slot.toolStripMenuItemRestore)
                {
                    ButtonRestoreOnClick(sender, null);
                }

                if (e.ClickedItem == _slot.toolStripMenuItemDelete)
                {
                    ButtonDeleteOnClick(sender, null);
                }

                if (e.ClickedItem == _slot.toolStripMenuItemEdit)
                {
                    FormNotes formNotes = new FormNotes {textBoxNotes = {Text = gameDetails.Notes}};
                    if (formNotes.ShowDialog(_slot) == DialogResult.OK)
                    {
                        gameDetails.Notes = formNotes.textBoxNotes.Text;
                        MapData mapData = GetSelectedMap();
                        if (mapData != null)
                        {
                            RefreshLisView(mapData);
                        }
                    }
                }
            }
        }

        private void PropertyGridGameDetailsOnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PropertyDescriptor propertyDescriptor = e.ChangedItem.PropertyDescriptor;
            if (propertyDescriptor != null && propertyDescriptor.Name == "Notes")
            {
                MapData mapData = GetSelectedMap();
                if (mapData != null)
                {
                    RefreshLisView(mapData);
                }
            }
        }

        private GameDetails GetSelectedGameDetails()
        {
            if (_slot.listViewSavedGames.SelectedIndices.Count == 1)
            {
                ListViewItem listViewItem = _slot.listViewSavedGames.SelectedItems[0];
                GameDetails gameDetails = listViewItem.Tag as GameDetails;
                return gameDetails;
            }

            return null;
        }

        private void CheckBoxScreenShotOnCheckedChanged(object sender, EventArgs e)
        {
            _screenshot = _slot.checkBoxScreenShot.Checked;
            SetAutoBackupMessage();
            if (_screenshot)
            {
                IntPtr doomPtr = GetDoomPtr();
                if (!ScreenShots.HasTitlebar(doomPtr))
                {
                    Message("Screenshot only works if Doom is set to Windowed Mode", "Warning!", MessageType.Error,
                        MessageMode.User);
                }
            }
        }

        private void ButtonDeleteOnClick(object sender, EventArgs e)
        {
            if (_slot.listViewSavedGames.Items.Count > 0 && _slot.listViewSavedGames.SelectedIndices.Count >= 1)
            {
                if (!(_slot.comboBoxMaps.SelectedItem is MapData mapData))
                {
                    Message("The deletion has failed", "Hmm :(", MessageType.Error, MessageMode.User);
                    return;
                }

                int error = 0;
                int ok = 0;

                for (var i = _slot.listViewSavedGames.SelectedItems.Count - 1; i >= 0; i--)
                {
                    ListViewItem selectedItem = _slot.listViewSavedGames.SelectedItems[i];
                    if (selectedItem.Tag is GameDetails gameDetails)
                    {
                        string pathMap = Path.Combine(_pathSlot, gameDetails.MapSafe);
                        string timeStampFolderName = gameDetails.DateTimeSafe;
                        string pathTimeStamp = Path.Combine(pathMap, timeStampFolderName);

                        if (DeleteSave(pathTimeStamp))
                        {
                            if (mapData.Games.Contains(gameDetails))
                                mapData.Games.Remove(gameDetails);
                            ok++;
                        }
                        else
                        {
                            error++;
                        }
                    }
                }

                if (error > 0)
                {
                    Message("The deletion has failed for " + error + " items :(", "Hmm :(", MessageType.Error,
                        MessageMode.User);
                }
                else
                {
                    RefreshLisView(mapData);
                    Message("The deletion has been successful, " + ok + " items deleted", "Deletion complete",
                        MessageType.Information, MessageMode.User);
                }
            }
        }

        private void ButtonRestoreOnClick(object sender, EventArgs e)
        {
            if (_slot.listViewSavedGames.Items.Count > 0 && _slot.listViewSavedGames.SelectedIndices.Count == 1)
            {
                ListViewItem listViewItem = _slot.listViewSavedGames.SelectedItems[0];

                if (listViewItem.Tag is GameDetails gameDetails)
                {
                    string pathMap = Path.Combine(_pathSlot, gameDetails.MapSafe);
                    string timeStampFolderName = gameDetails.DateTimeSafe;
                    string pathTimeStamp = Path.Combine(pathMap, timeStampFolderName);
                    string pathGameAutoGame = Path.Combine(pathTimeStamp, "GAME-AUTOSAVE" + (_id - 1));

                    if (RestoreSave(pathGameAutoGame))
                    {
                        Message("The restoration has been successful", "Restoration Complete", MessageType.Information,
                            MessageMode.User);
                    }
                    else
                    {
                        Message("The restoration failed :(", "Hmm :(", MessageType.Error, MessageMode.User);
                    }
                }
            }
        }

        private void ButtonBackupNowOnClick(object sender, EventArgs e)
        {
            bool screenshotStatus = _screenshot;
            try
            {
                _screenshot = false;
                if (BackupSave(false))
                {
                    LoadControl();
                    Message("The backup has been successful", "Backup Successful", MessageType.Information,
                        MessageMode.User);
                }
                else
                    Message("The folder seams to be missing :(", "Hmm :(", MessageType.Error, MessageMode.User);
            }
            finally
            {
                _screenshot = screenshotStatus;
            }
        }

        private void ListViewSavedGamesOnSelectedIndexChanged(object sender, EventArgs e)
        {
            _slot.buttonRestore.Enabled = false;
            _slot.buttonDelete.Enabled = false;
            _slot.pictureBoxScreenshot.ImageLocation = null;

            _slot.contextMenuStripListView.Enabled = false;

            _slot.toolStripMenuItemRestore.Enabled = false;
            _slot.toolStripMenuItemEdit.Enabled = false;
            _slot.toolStripMenuItemDelete.Enabled = false;

            if (_slot.listViewSavedGames.Items.Count > 0)
            {
                _slot.contextMenuStripListView.Enabled = true;

                if (_slot.listViewSavedGames.SelectedIndices.Count == 1)
                {
                    _slot.buttonRestore.Enabled = true;
                    _slot.toolStripMenuItemRestore.Enabled = true;
                    _slot.toolStripMenuItemEdit.Enabled = true;
                    _slot.toolStripMenuItemDelete.Enabled = true;

                    ListViewItem listViewItem = _slot.listViewSavedGames.SelectedItems[0];
                    GameDetails gameDetails = listViewItem.Tag as GameDetails;

                    _slot.propertyGridGameDetails.SelectedObject = gameDetails;

                    if (gameDetails != null && gameDetails.HasScreenshots)
                        _slot.pictureBoxScreenshot.ImageLocation = gameDetails.ScreenshotsPath;
                }
                else
                    _slot.propertyGridGameDetails.SelectedObject = null;

                if (_slot.listViewSavedGames.SelectedIndices.Count >= 1)
                {
                    _slot.buttonDelete.Enabled = true;
                    _slot.toolStripMenuItemDelete.Enabled = true;
                }
            }
        }

        private MapData GetSelectedMap()
        {
            return _slot.comboBoxMaps.SelectedItem as MapData;
        }

        private void ComboBoxMapsOnSelectionChangeCommitted(object sender, EventArgs e)
        {
            MapData mapData = GetSelectedMap();
            if (mapData != null)
            {
                RefreshLisView(mapData);
            }
        }

        private void RefreshLisView(MapData mapData)
        {
            _slot.listViewSavedGames.BeginUpdate();

            int selected = -1;
            if (_slot.listViewSavedGames.SelectedIndices.Count == 1)
                selected = _slot.listViewSavedGames.SelectedIndices[0];

            _slot.listViewSavedGames.Items.Clear();

            foreach (GameDetails gameDetails in mapData.Games)
            {
                ListViewItem listViewItem = new ListViewItem {Text = gameDetails.DateTimeString};
                listViewItem.SubItems.Add(gameDetails.DiffString);
                listViewItem.SubItems.Add(gameDetails.PlayedTime.ToString("g"));
                listViewItem.SubItems.Add(gameDetails.IsDeath ? "🕱" : "");
                listViewItem.SubItems.Add(gameDetails.Notes);

                listViewItem.Tag = gameDetails;

                _slot.listViewSavedGames.Items.Add(listViewItem);
            }

            if (selected > -1)
                _slot.listViewSavedGames.SelectedIndices.Add(selected);

            _slot.listViewSavedGames.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            _slot.listViewSavedGames.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            _slot.listViewSavedGames.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize);
            _slot.listViewSavedGames.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent);

            if (_slot.listViewSavedGames.SelectedIndices.Count == 0 && _slot.listViewSavedGames.Items.Count > 0)
                _slot.listViewSavedGames.SelectedIndices.Add(0);

            ListViewSavedGamesOnSelectedIndexChanged(null, null);

            _slot.listViewSavedGames.EndUpdate();
        }

        private void CheckBoxAutoBackupOnCheckedChanged(object sender, EventArgs e)
        {
            if (_autoBackup != _slot.checkBoxAutoBackup.Checked)
            {
                _autoBackup = _slot.checkBoxAutoBackup.Checked;

                _slot.checkBoxIncludeDeath.Enabled = _slot.checkBoxAutoBackup.Checked;
                _slot.checkBoxScreenShot.Enabled = _slot.checkBoxAutoBackup.Checked;

                if (SetWatcher(_autoBackup))
                {
                    SetAutoBackupMessage();
                }
                else
                {
                    Message("Auto backup failed to start", "Hmm :(", MessageType.Information, MessageMode.User);
                }
            }
        }

        private void CheckBoxIncludeDeathOnCheckedChanged(object sender, EventArgs e)
        {
            _includeDeath = _slot.checkBoxIncludeDeath.Checked;
            SetAutoBackupMessage();
        }

        private void SetAutoBackupMessage()
        {
            string message;
            if (_watcherActive)
            {
                message = "Auto backup enabled";
            }
            else
            {
                message = "Auto backup disabled";
            }

            if (_watcherActive && _includeDeath)
                message += ", including death checkpoint";

            if (_watcherActive && _screenshot)
                message += ", with screenshots";

            Message(message, "", MessageType.Information, MessageMode.Status);
        }

        private void UnloadControl()
        {
            _slot.buttonBackupNow.Enabled = false;
            _slot.buttonRestore.Enabled = false;
            _slot.buttonDelete.Enabled = false;

            _slot.checkBoxAutoBackup.Checked = false;
            _slot.checkBoxIncludeDeath.Checked = false;
            _slot.checkBoxScreenShot.Checked = false;

            _slot.checkBoxAutoBackup.Enabled = false;
            _slot.checkBoxIncludeDeath.Enabled = false;
            _slot.checkBoxScreenShot.Enabled = false;

            _slot.comboBoxMaps.Items.Clear();
            _slot.comboBoxMaps.Enabled = false;

            _slot.listViewSavedGames.Items.Clear();

            _slot.propertyGridGameDetails.SelectedObject = null;
        }

        private void LoadControl()
        {
            if (_slot == null)
                return;

            _slot.buttonBackupNow.Enabled = true;
            _slot.checkBoxAutoBackup.Enabled = true;
            _slot.comboBoxMaps.Enabled = true;

            // Map list
            _slot.comboBoxMaps.BeginUpdate();

            MapData selection = _slot.comboBoxMaps.SelectedItem as MapData;

            int currentCount = _slot.comboBoxMaps.Items.Count;

            _slot.comboBoxMaps.Items.Clear();

            foreach (MapData mapData in _maps)
            {
                _slot.comboBoxMaps.Items.Add(mapData);
            }

            if (selection != null && _slot.comboBoxMaps.Items.Contains(selection))
                _slot.comboBoxMaps.SelectedItem = selection;
            else
            {
                if (_slot.comboBoxMaps.Items.Count > 0)
                    _slot.comboBoxMaps.SelectedIndex = _slot.comboBoxMaps.Items.Count - 1; // Select last map
            }

            // TODO: improve that
            if (currentCount < _slot.comboBoxMaps.Items.Count)
                _slot.comboBoxMaps.SelectedIndex = _slot.comboBoxMaps.Items.Count - 1; // Select new map

            _slot.comboBoxMaps.EndUpdate();
            ComboBoxMapsOnSelectionChangeCommitted(null, null);

            _slot.checkBoxAutoBackup.Checked = _autoBackup;
            _slot.checkBoxIncludeDeath.Checked = _includeDeath;
        }

        private MapData CheckAndGetMapData(string mapName)
        {
            MapData mapData = _maps.FirstOrDefault(o => o.NameSafe == mapName || o.Name == mapName);

            if (mapData == null)
            {
                mapData = new MapData(_pathSlot, mapName);
                _maps.Add(mapData);
                _maps.Sort((data1, data2) => _mapComparer.Compare(data1.Name, data2.Name));
            }

            return mapData;
        }

        private void CheckAndUpdateGameDetails(MapData mapData, string timeStamp, GameDetails inGameDetails)
        {
            GameDetails gameDetails = CheckAndGetGameDetails(mapData, timeStamp);
            if (gameDetails != null)
            {
                mapData.Games.Remove(gameDetails);
            }

            mapData.Games.Add(inGameDetails);
            mapData.Games.Sort((data1, data2) =>
                _timeStampComparer.Compare(data1.DateTimeString, data2.DateTimeString));
        }

        private GameDetails CheckAndGetGameDetails(MapData mapData, string timeStamp)
        {
            return mapData.Games.FirstOrDefault(o => o.DateTimeSafe == timeStamp);
        }

        private void ScanBackupFolder()
        {
            if (!Directory.Exists(_backupFolder))
                return;

            DirectoryInfo directoryInfo;

            if (!Directory.Exists(_pathSlot))
                directoryInfo = Directory.CreateDirectory(_pathSlot);
            else
                directoryInfo = new DirectoryInfo(_pathSlot);

            _maps.Clear();

            // loop through maps
            foreach (DirectoryInfo directoryMap in directoryInfo.GetDirectories())
            {
                string mapNameSafe = directoryMap.Name;
                MapData mapData = CheckAndGetMapData(mapNameSafe);

                // loop through timestamps
                foreach (DirectoryInfo directoryTimeStamp in directoryMap.GetDirectories())
                {
                    string timeStampPath = directoryTimeStamp.FullName;
                    string pathGameAutoGame = Path.Combine(timeStampPath, "GAME-AUTOSAVE" + (_id - 1));

                    if (Directory.Exists(pathGameAutoGame))
                    {
                        string gameDetailsFilePath = Path.Combine(pathGameAutoGame, "game.details");
                        if (File.Exists(gameDetailsFilePath))
                        {
                            string content = File.ReadAllText(gameDetailsFilePath);

                            GameDetails gameDetails = new GameDetails(_id, content);
                            gameDetails.SetPath(timeStampPath);

                            string markerFile = Path.Combine(timeStampPath, ".hg.death");
                            if (File.Exists(markerFile))
                                gameDetails.IsDeath = true;

                            string screenshotsFile = Path.Combine(timeStampPath,
                                directoryTimeStamp.Name + ScreenShotExtension);
                            if (File.Exists(screenshotsFile))
                            {
                                gameDetails.HasScreenshots = true;
                                gameDetails.ScreenshotsPath = screenshotsFile;
                            }

                            CheckAndUpdateGameDetails(mapData, directoryTimeStamp.Name, gameDetails);
                        }
                    }
                }
            }
        }

        private bool BackupSave(bool isDeath)
        {
            string pathSave = Path.Combine(_savedGameFolder, "GAME-AUTOSAVE" + (_id - 1));
            if (!Directory.Exists(pathSave))
            {
                return false;
            }

            string gameDetailsFilePath = Path.Combine(pathSave, "game.details");
            if (!File.Exists(gameDetailsFilePath))
                return false;

            string gameDetailsContent = File.ReadAllText(gameDetailsFilePath);
            GameDetails gameDetails = new GameDetails(_id, gameDetailsContent) {IsDeath = isDeath};

            string pathSlot = Path.Combine(_backupFolder, "Slot" + _id);
            string pathMap = Path.Combine(pathSlot, gameDetails.MapSafe);
            Directory.CreateDirectory(pathMap);

            string timeStampFolderName = gameDetails.DateTimeSafe;
            string pathTimeStamp = Path.Combine(pathMap, timeStampFolderName);
            Directory.CreateDirectory(pathTimeStamp);

            gameDetails.SetPath(pathTimeStamp);

            string pathGameAutoGame = Path.Combine(pathTimeStamp, "GAME-AUTOSAVE" + (_id - 1));
            Directory.CreateDirectory(pathGameAutoGame);

            DirectoryInfo source = new DirectoryInfo(pathSave);
            DirectoryInfo target = new DirectoryInfo(pathGameAutoGame);

            foreach (FileInfo fileInfo in source.GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
            }

            if (isDeath)
            {
                string markerFile = Path.Combine(pathTimeStamp, ".hg.death");
                File.WriteAllText(markerFile, @"甘き死よ、来たれ");
            }

            IntPtr doomPtr = GetDoomPtr();
            if (doomPtr != IntPtr.Zero)
            {
                try
                {
                    string screenshotsFile = Path.Combine(pathTimeStamp, timeStampFolderName + ScreenShotExtension);
                    if (ScreenShots.HasTitlebar(doomPtr))
                    {
                        Thread.Sleep(250);
                        Bitmap bitmap = ScreenShots.Take(doomPtr);
                        bitmap.Save(screenshotsFile, ScreenShotFormat);
                        gameDetails.HasScreenshots = true;
                        gameDetails.ScreenshotsPath = screenshotsFile;
                    }
                }
                catch (Exception)
                {
                    gameDetails.HasScreenshots = false;
                    gameDetails.ScreenshotsPath = "";
                }
            }

            MapData mapData = CheckAndGetMapData(gameDetails.MapSafe);
            CheckAndUpdateGameDetails(mapData, timeStampFolderName, gameDetails);

            return true;
        }

        private static IntPtr GetDoomPtr()
        {
            IntPtr doomPtr = IntPtr.Zero;
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.StartsWith("DOOM"))
                {
                    doomPtr = process.MainWindowHandle;
                    break;
                }
            }

            return doomPtr;
        }

        private bool DeleteSave(string sourcePath)
        {
            try
            {
                Directory.Delete(sourcePath, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool RestoreSave(string sourcePath)
        {
            string pathSave = Path.Combine(_savedGameFolder, "GAME-AUTOSAVE" + (_id - 1));
            if (!Directory.Exists(pathSave))
            {
                return false;
            }

            DirectoryInfo source = new DirectoryInfo(sourcePath);
            DirectoryInfo target = new DirectoryInfo(pathSave);
            bool canRestore = false;

            if (_autoBackup)
            {
                SetWatcher(false);
                Thread.Sleep(250);
            }

            try
            {
                foreach (FileInfo fileInfo in target.GetFiles())
                {
                    fileInfo.MoveTo(fileInfo.FullName + ".hg.bak");
                }

                Thread.Sleep(250);

                canRestore = true;

                foreach (FileInfo fileInfo in source.GetFiles())
                {
                    fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
                }

                Thread.Sleep(250);

                foreach (FileInfo fileInfo in target.GetFiles())
                {
                    if (fileInfo.Name.EndsWith(".hg.bak"))
                    {
                        fileInfo.Delete();
                        canRestore = false;
                    }
                }
            }
            catch (Exception)
            {
                if (canRestore)
                {
                    foreach (FileInfo fileInfo in target.GetFiles())
                    {
                        if (!fileInfo.Name.EndsWith(".hg.bak"))
                        {
                            fileInfo.Delete();
                        }

                        if (fileInfo.Name.EndsWith(".hg.bak"))
                        {
                            fileInfo.MoveTo(fileInfo.FullName.Replace(".hg.bak", ""));
                        }
                    }
                }

                return false;
            }

            if (_autoBackup)
            {
                SetWatcher(true);
            }

            return true;
        }

        private void SaveWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                // Mostly in case of death
                if (e.Name == "checkpoint_alt.dat")
                    _isCheckPointAlt = true;
                // Proper checkpoint
                if (e.Name == "checkpoint.dat")
                    _isCheckPoint = true;

                // End of save event
                if (e.Name == "game.details.verify")
                {
                    if (_isCheckPoint)
                    {
                        // Normal checkpoint (usually)
                        if (BackupSave(false))
                        {
                            _slot.Invoke(new Action(LoadControl));
                        }
                    }
                    else if (!_isCheckPoint && _isCheckPointAlt && _includeDeath)
                    {
                        // Death checkpoint (usually)
                        if (BackupSave(true))
                        {
                            _slot.Invoke(new Action(LoadControl));
                        }
                    }

                    _isCheckPoint = false;
                    _isCheckPointAlt = false;
                }
            }
            catch (Exception)
            {
            }
        }

        private bool SetWatcher(bool activate)
        {
            _watcherActive = false;
            string pathSave = Path.Combine(_savedGameFolder, "GAME-AUTOSAVE" + (_id - 1));
            if (!Directory.Exists(pathSave))
            {
                return false;
            }

            try
            {
                if (_fileSystemWatcher == null)
                {
                    _fileSystemWatcher = new FileSystemWatcher
                    {
                        Path = pathSave,
                        Filter = "*",
                        EnableRaisingEvents = false,
                        NotifyFilter = NotifyFilters.FileName
                    };
                    _fileSystemWatcher.Renamed += SaveWatcherOnRenamed;
                }

                if (activate)
                {
                    _isCheckPoint = false;
                    _isCheckPointAlt = false;
                    _fileSystemWatcher.EnableRaisingEvents = true;
                    _watcherActive = true;
                }
                else
                {
                    _fileSystemWatcher.EnableRaisingEvents = false;
                    _isCheckPoint = false;
                    _isCheckPointAlt = false;
                    _watcherActive = false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}