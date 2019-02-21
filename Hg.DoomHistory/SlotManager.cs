using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Hg.DoomHistory
{
    public delegate DialogResult MessageEventHandler(string text, string caption, MessageType type, MessageMode mode);

    public class SlotManager
    {
        private readonly string _backupFolder;
        private readonly int _id;

        private readonly MapComparer _mapComparer = new MapComparer();

        private readonly List<MapData> _maps = new List<MapData>();
        private readonly string _pathSlot;
        private readonly string _savedGameFolder;
        private readonly string _savedGameSlotFolder;
        private readonly string _savedGameSlotName;
        private readonly TimeStampComparer _timeStampComparer = new TimeStampComparer();

        private bool _autoBackup;

        private FileSystemWatcher _fileSystemWatcherDoomFolder;
        private FileSystemWatcher _fileSystemWatcherSlotFolder;
        private bool _includeDeath;

        private bool _isCheckPoint;
        private bool _isCheckPointAlt;
        private bool _screenshot;
        private string _screenShotExtension = ".jpg";
        private ImageFormat _screenShotFormat = ImageFormat.Jpeg;

        private SlotControl _slot;

        private bool _waitingOnSlotFolder;

        private bool _watcherActive;

        public SlotManager(SlotControl slot, int id, string savedGameFolder, string backupFolder,
            MessageEventHandler messageEventHandler)
        {
            OnMessage += messageEventHandler;

            _slot = slot;

            _id = id;
            _backupFolder = backupFolder;
            _savedGameFolder = savedGameFolder;
            _savedGameSlotName = "GAME-AUTOSAVE" + (_id - 1);
            _savedGameSlotFolder = Path.Combine(_savedGameFolder, _savedGameSlotName);
            _pathSlot = Path.Combine(_backupFolder, "Slot" + _id);

            SlotName = "Slot " + _id;

            // Default to jpg
            SetScreenshotQuality(ScreenshotQuality.Jpg);

            UnloadControl();

            BindEvent();

            ScanBackupFolder();

            LoadControl();
        }

        public string SlotName { get; }

        private bool BackupSave(bool isDeath)
        {
            Logger.Log("Slot " + _id + ", BackupSave: Enter", LogLevel.Debug);

            var pathSave = Path.Combine(_savedGameFolder, _savedGameSlotName);
            if (!Directory.Exists(pathSave))
            {
                Logger.Log("Slot " + _id + ", BackupSave: pathSave does not exist", LogLevel.Debug);
                return false;
            }

            var gameDetailsFilePath = Path.Combine(pathSave, "game.details");
            if (!File.Exists(gameDetailsFilePath))
            {
                Logger.Log("Slot " + _id + ", BackupSave: gameDetailsFilePath does not exist", LogLevel.Debug);
                return false;
            }

            var gameDetailsContent = File.ReadAllText(gameDetailsFilePath);
            var gameDetails = new GameDetails(_id, gameDetailsContent) {IsDeath = isDeath};

            var pathSlot = Path.Combine(_backupFolder, "Slot" + _id);
            var pathMap = Path.Combine(pathSlot, gameDetails.MapSafe);
            Directory.CreateDirectory(pathMap);

            var timeStampFolderName = gameDetails.DateTimeSafe;
            var pathTimeStamp = Path.Combine(pathMap, timeStampFolderName);
            Directory.CreateDirectory(pathTimeStamp);

            gameDetails.SetPath(pathTimeStamp);

            var pathGameAutoGame = Path.Combine(pathTimeStamp, _savedGameSlotName);
            Directory.CreateDirectory(pathGameAutoGame);

            var source = new DirectoryInfo(pathSave);
            var target = new DirectoryInfo(pathGameAutoGame);

            foreach (var fileInfo in source.GetFiles()) fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);

            if (isDeath)
            {
                var markerFile = Path.Combine(pathTimeStamp, ".hg.death");
                File.WriteAllText(markerFile, @"甘き死よ、来たれ");
            }

            if (_screenshot)
            {
                var doomPtr = GetDoomPtr();
                if (doomPtr != IntPtr.Zero)
                {
                    Logger.Log("Slot " + _id + ", BackupSave: Doom is running", LogLevel.Debug);
                    try
                    {
                        var screenshotsFile =
                            Path.Combine(pathTimeStamp, timeStampFolderName + _screenShotExtension);
                        if (ScreenShots.HasTitlebar(doomPtr))
                        {
                            Thread.Sleep(250);
                            var bitmap = ScreenShots.Take(doomPtr);
                            bitmap.Save(screenshotsFile, _screenShotFormat);
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
            }

            var mapData = CheckAndGetMapData(gameDetails.MapDesc, gameDetails.MapSafe);
            if (string.IsNullOrEmpty(mapData.NameInternal))
                mapData.NameInternal = gameDetails.MapName;

            CheckAndUpdateGameDetails(mapData, timeStampFolderName, gameDetails);

            Logger.Log("Slot " + _id + ", BackupSave: Exit", LogLevel.Debug);
            return true;
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

        private void ButtonBackupNowOnClick(object sender, EventArgs e)
        {
            var screenshotStatus = _screenshot;
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
                {
                    Message("The folder seams to be missing :(", "Hmm :(", MessageType.Error, MessageMode.User);
                }
            }
            finally
            {
                _screenshot = screenshotStatus;
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

                var error = 0;
                var ok = 0;

                for (var i = _slot.listViewSavedGames.SelectedItems.Count - 1; i >= 0; i--)
                {
                    var selectedItem = _slot.listViewSavedGames.SelectedItems[i];
                    if (selectedItem.Tag is GameDetails gameDetails)
                    {
                        var pathTimeStamp = gameDetails.Path;
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
                    Message("The deletion has failed for " + error + " items :(", "Hmm :(", MessageType.Error,
                        MessageMode.User);
                else
                    Message("The deletion has been successful, " + ok + " items deleted", "Deletion complete",
                        MessageType.Information, MessageMode.User);

                // Refresh listview
                RefreshLisView(mapData);
            }
        }

        private void ButtonRestoreOnClick(object sender, EventArgs e)
        {
            if (_slot.listViewSavedGames.Items.Count > 0 && _slot.listViewSavedGames.SelectedIndices.Count == 1)
            {
                var listViewItem = _slot.listViewSavedGames.SelectedItems[0];

                if (listViewItem.Tag is GameDetails gameDetails)
                {
                    var pathMap = Path.Combine(_pathSlot, gameDetails.MapSafe);
                    var timeStampFolderName = gameDetails.DateTimeSafe;
                    var pathTimeStamp = Path.Combine(pathMap, timeStampFolderName);
                    var pathGameAutoGame = Path.Combine(pathTimeStamp, _savedGameSlotName);

                    if (RestoreSave(pathGameAutoGame))
                        Message("The restoration has been successful", "Restoration Complete", MessageType.Information,
                            MessageMode.User);
                    else
                        Message("The restoration failed :(", "Hmm :(", MessageType.Error, MessageMode.User);
                }
            }
        }

        private GameDetails CheckAndGetGameDetails(MapData mapData, string timeStamp)
        {
            return mapData.Games.FirstOrDefault(o => o.DateTimeSafe == timeStamp);
        }

        private MapData CheckAndGetMapData(string mapName, string mapNameSafe)
        {
            var mapData = _maps.FirstOrDefault(o => o.NameSafe == mapNameSafe || o.Name == mapName);

            if (mapData == null)
            {
                mapData = new MapData(_pathSlot, mapName, mapNameSafe);
                _maps.Add(mapData);
            }

            return mapData;
        }

        private void CheckAndUpdateGameDetails(MapData mapData, string timeStamp, GameDetails inGameDetails)
        {
            var gameDetails = CheckAndGetGameDetails(mapData, timeStamp);
            if (gameDetails != null) mapData.Games.Remove(gameDetails);

            mapData.Games.Add(inGameDetails);
            mapData.Games.Sort((data1, data2) =>
                _timeStampComparer.Compare(data1, data2));
        }

        private void CheckBoxAutoBackupOnCheckedChanged(object sender, EventArgs e)
        {
            if (_autoBackup != _slot.checkBoxAutoBackup.Checked)
            {
                _autoBackup = _slot.checkBoxAutoBackup.Checked;

                _slot.checkBoxIncludeDeath.Enabled = _slot.checkBoxAutoBackup.Checked;
                _slot.checkBoxScreenShot.Enabled = _slot.checkBoxAutoBackup.Checked;

                if (SetWatcher(_autoBackup))
                    SetAutoBackupMessageSuccess();
                else
                    SetAutoBackupMessageFailure();
            }
        }

        private void CheckBoxIncludeDeathOnCheckedChanged(object sender, EventArgs e)
        {
            _includeDeath = _slot.checkBoxIncludeDeath.Checked;
            SetAutoBackupMessageSuccess();
        }

        private void CheckBoxScreenShotOnCheckedChanged(object sender, EventArgs e)
        {
            _screenshot = _slot.checkBoxScreenShot.Checked;
            SetAutoBackupMessageSuccess();
            if (_screenshot)
            {
                var doomPtr = GetDoomPtr();
                if (!ScreenShots.HasTitlebar(doomPtr))
                    Message("Screenshot only works if Doom is set to Windowed Mode", "Warning!", MessageType.Error,
                        MessageMode.User);
            }
        }

        private void ComboBoxMapsOnSelectionChangeCommitted(object sender, EventArgs e)
        {
            var mapData = GetSelectedMap();
            if (mapData != null) RefreshLisView(mapData);
        }

        private void ContextMenuStripListViewOnItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _slot.contextMenuStripListView.Hide();
            if (e.ClickedItem == _slot.toolStripMenuItemRestore) ButtonRestoreOnClick(sender, null);

            if (e.ClickedItem == _slot.toolStripMenuItemDelete) ButtonDeleteOnClick(sender, null);

            if (e.ClickedItem == _slot.toolStripMenuItemEdit)
            {
                var gameDetails = GetSelectedGameDetails();
                if (gameDetails != null)
                {
                    var formNotes = new FormNotes {textBoxNotes = {Text = gameDetails.Notes}};
                    if (formNotes.ShowDialog(_slot) == DialogResult.OK)
                    {
                        gameDetails.Notes = formNotes.textBoxNotes.Text;
                        var mapData = GetSelectedMap();
                        if (mapData != null) RefreshLisView(mapData);
                    }
                }
            }
        }

        private bool DeleteSave(string sourcePath)
        {
            try
            {
                if (Directory.Exists(sourcePath)) Directory.Delete(sourcePath, true);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Slot " + _id + ", DeleteSave: Exception: " + ex.Message, LogLevel.Debug);
                Logger.LogException(ex);
                return false;
            }
        }

        private void FileSystemWatcherDoomFolderOnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.Name == _savedGameSlotName && Directory.Exists(_savedGameSlotFolder))
            {
                _waitingOnSlotFolder = false;

                // Create or recreate the slot watcher
                MakeSlotWatcher();

                // Enable or disable accordingly
                if (SetWatcher(_autoBackup))
                    SetAutoBackupMessageSuccess();
                else
                    SetAutoBackupMessageFailure();
            }
        }

        private void FileSystemWatcherDoomFolderOnDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.Name == _savedGameSlotName && !Directory.Exists(_savedGameSlotFolder))
            {
                if (_fileSystemWatcherSlotFolder != null) _fileSystemWatcherSlotFolder.EnableRaisingEvents = false;
                _fileSystemWatcherSlotFolder = null;
                _waitingOnSlotFolder = true;
            }
        }

        private static IntPtr GetDoomPtr()
        {
            var doomPtr = IntPtr.Zero;
            foreach (var process in Process.GetProcesses())
                if (process.ProcessName == "DOOMx64" || process.ProcessName == "DOOMx64vk")
                {
                    doomPtr = process.MainWindowHandle;
                    break;
                }

            return doomPtr;
        }

        private GameDetails GetSelectedGameDetails()
        {
            if (_slot.listViewSavedGames.SelectedIndices.Count == 1)
            {
                var listViewItem = _slot.listViewSavedGames.SelectedItems[0];
                var gameDetails = listViewItem.Tag as GameDetails;
                return gameDetails;
            }

            return null;
        }

        private MapData GetSelectedMap()
        {
            return _slot.comboBoxMaps.SelectedItem as MapData;
        }

        private void ListViewSavedGamesOnDoubleClick(object sender, EventArgs e)
        {
            var gameDetails = GetSelectedGameDetails();
            if (gameDetails != null)
            {
                var formNotes = new FormNotes {textBoxNotes = {Text = gameDetails.Notes}};
                if (formNotes.ShowDialog(_slot) == DialogResult.OK)
                {
                    gameDetails.Notes = formNotes.textBoxNotes.Text;
                    var mapData = GetSelectedMap();
                    if (mapData != null) RefreshLisView(mapData);
                }
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

                    var listViewItem = _slot.listViewSavedGames.SelectedItems[0];
                    var gameDetails = listViewItem.Tag as GameDetails;

                    _slot.propertyGridGameDetails.SelectedObject = gameDetails;

                    if (gameDetails != null && gameDetails.HasScreenshots)
                        _slot.pictureBoxScreenshot.ImageLocation = gameDetails.ScreenshotsPath;
                }
                else
                {
                    _slot.propertyGridGameDetails.SelectedObject = null;
                }

                if (_slot.listViewSavedGames.SelectedIndices.Count >= 1)
                {
                    _slot.buttonDelete.Enabled = true;
                    _slot.toolStripMenuItemDelete.Enabled = true;
                }
            }
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

            var selection = _slot.comboBoxMaps.SelectedItem as MapData;

            var currentCount = _slot.comboBoxMaps.Items.Count;

            _slot.comboBoxMaps.Items.Clear();

            _maps.Sort((data1, data2) => _mapComparer.Compare(data1, data2));

            foreach (var mapData in _maps) _slot.comboBoxMaps.Items.Add(mapData);

            if (selection != null && _slot.comboBoxMaps.Items.Contains(selection))
            {
                _slot.comboBoxMaps.SelectedItem = selection;
            }
            else
            {
                if (_slot.comboBoxMaps.Items.Count > 0)
                    _slot.comboBoxMaps.SelectedIndex = _slot.comboBoxMaps.Items.Count - 1; // Select last map
            }

            // TODO: improve that
            if (currentCount < _slot.comboBoxMaps.Items.Count)
                _slot.comboBoxMaps.SelectedIndex = _slot.comboBoxMaps.Items.Count - 1; // Select new map

            _slot.comboBoxMaps.EndUpdate();

            ComboBoxMapsOnSelectionChangeCommitted(_slot.comboBoxMaps, null);

            _slot.checkBoxAutoBackup.Checked = _autoBackup;
            _slot.checkBoxIncludeDeath.Checked = _includeDeath;
        }

        private void MakeSlotWatcher()
        {
            // Clear previous
            _watcherActive = false;
            if (_fileSystemWatcherSlotFolder != null) _fileSystemWatcherSlotFolder.EnableRaisingEvents = false;
            _fileSystemWatcherSlotFolder = null;

            // Create new
            _fileSystemWatcherSlotFolder = new FileSystemWatcher
            {
                Path = _savedGameSlotFolder,
                Filter = "*",
                EnableRaisingEvents = false,
                NotifyFilter = NotifyFilters.FileName
            };
            _fileSystemWatcherSlotFolder.Renamed += FileSystemWatcherSlotFolderOnRenamed;
            Logger.Log("Slot " + _id + ", SetWatcher: _fileSystemWatcherSlotFolder created", LogLevel.Debug);
        }

        private DialogResult Message(string text, string caption, MessageType type, MessageMode mode)
        {
            var msg = SlotName + ": " + text;
            Logger.Log(text, LogLevel.Debug);
            if (OnMessage != null) return OnMessage(msg, caption, type, mode);

            return DialogResult.None;
        }

        public event MessageEventHandler OnMessage;

        private void PropertyGridGameDetailsOnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var propertyDescriptor = e.ChangedItem.PropertyDescriptor;
            if (propertyDescriptor != null && propertyDescriptor.Name == "Notes")
            {
                var mapData = GetSelectedMap();
                if (mapData != null) RefreshLisView(mapData);
            }
        }

        private void RefreshLisView(MapData mapData)
        {
            _slot.listViewSavedGames.BeginUpdate();

            var selected = -1;
            if (_slot.listViewSavedGames.SelectedIndices.Count == 1)
                selected = _slot.listViewSavedGames.SelectedIndices[0];

            _slot.listViewSavedGames.Items.Clear();

            foreach (var gameDetails in mapData.Games)
            {
                var listViewItem = new ListViewItem {Text = gameDetails.DateTimeString};
                listViewItem.SubItems.Add(gameDetails.DiffString);
                listViewItem.SubItems.Add(gameDetails.PlayedTime.ToString("g"));
                listViewItem.SubItems.Add(gameDetails.IsDeath ? "🕱" : "");
                listViewItem.SubItems.Add(gameDetails.Notes);

                listViewItem.Tag = gameDetails;

                _slot.listViewSavedGames.Items.Add(listViewItem);
            }

            if (selected > -1 && selected < _slot.listViewSavedGames.Items.Count)
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

        public void Release()
        {
            UnbindEvent();
            UnloadControl();
            _maps.Clear();
            _slot = null;
        }

        private bool RestoreSave(string sourcePath)
        {
            var pathSave = Path.Combine(_savedGameFolder, _savedGameSlotName);
            if (!Directory.Exists(pathSave)) return false;

            if (!Directory.Exists(sourcePath)) return false;

            var source = new DirectoryInfo(sourcePath);
            var target = new DirectoryInfo(pathSave);
            var canRestore = false;

            if (_autoBackup)
            {
                SetWatcher(false);
                Thread.Sleep(250);
            }

            try
            {
                foreach (var fileInfo in target.GetFiles()) fileInfo.MoveTo(fileInfo.FullName + ".hg.bak");

                Thread.Sleep(250);

                canRestore = true;

                foreach (var fileInfo in source.GetFiles()) fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);

                Thread.Sleep(250);

                foreach (var fileInfo in target.GetFiles())
                    if (fileInfo.Name.EndsWith(".hg.bak"))
                    {
                        fileInfo.Delete();
                        canRestore = false;
                    }
            }
            catch (Exception)
            {
                if (canRestore)
                    foreach (var fileInfo in target.GetFiles())
                    {
                        if (!fileInfo.Name.EndsWith(".hg.bak")) fileInfo.Delete();

                        if (fileInfo.Name.EndsWith(".hg.bak")) fileInfo.MoveTo(fileInfo.FullName.Replace(".hg.bak", ""));
                    }

                return false;
            }

            if (_autoBackup) SetWatcher(true);

            return true;
        }

        private void FileSystemWatcherSlotFolderOnRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                // Fail-safe: check if Doom is running for auto-backup
                var doomPtr = GetDoomPtr();
                if (doomPtr == IntPtr.Zero)
                {
                    Logger.Log("Slot " + _id + ", FileSystemWatcherSlotFolderOnRenamed: Change detected but Doom is not running :(", LogLevel.Debug);
                    return;
                }

                // Mostly in case of death
                if (e.Name == "checkpoint_alt.dat")
                    _isCheckPointAlt = true;

                // Proper checkpoint
                if (e.Name == "checkpoint.dat")
                    _isCheckPoint = true;

                // End of save event
                if (e.Name == "game.details.verify")
                {
                    Logger.Log("Slot " + _id + ", FileSystemWatcherSlotFolderOnRenamed: game.details.verify event", LogLevel.Debug);
                    if (_isCheckPoint)
                    {
                        // Normal checkpoint (usually)
                        if (BackupSave(false)) _slot.Invoke(new Action(LoadControl));
                    }
                    else if (!_isCheckPoint && _isCheckPointAlt && _includeDeath)
                    {
                        // Death checkpoint (usually)
                        if (BackupSave(true)) _slot.Invoke(new Action(LoadControl));
                    }

                    _isCheckPoint = false;
                    _isCheckPointAlt = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Slot " + _id + ", FileSystemWatcherSlotFolderOnRenamed: Exception: " + ex.Message, LogLevel.Debug);
                Logger.LogException(ex);
            }
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
            foreach (var directoryMap in directoryInfo.GetDirectories())
            // loop through timestamps
            foreach (var directoryTimeStamp in directoryMap.GetDirectories())
            {
                var timeStampPath = directoryTimeStamp.FullName;
                var pathGameAutoGame = Path.Combine(timeStampPath, _savedGameSlotName);

                if (Directory.Exists(pathGameAutoGame))
                {
                    var gameDetailsFilePath = Path.Combine(pathGameAutoGame, "game.details");
                    if (File.Exists(gameDetailsFilePath))
                    {
                        var content = File.ReadAllText(gameDetailsFilePath);

                        var gameDetails = new GameDetails(_id, content);
                        gameDetails.SetPath(timeStampPath);

                        var mapData = CheckAndGetMapData(gameDetails.MapDesc, gameDetails.MapSafe);

                        if (string.IsNullOrEmpty(mapData.NameInternal))
                            mapData.NameInternal = gameDetails.MapName;

                        var markerFile = Path.Combine(timeStampPath, ".hg.death");
                        if (File.Exists(markerFile))
                            gameDetails.IsDeath = true;

                        var screenshotsFile = Path.Combine(timeStampPath, directoryTimeStamp.Name + ".jpg");
                        if (File.Exists(screenshotsFile))
                        {
                            gameDetails.HasScreenshots = true;
                            gameDetails.ScreenshotsPath = screenshotsFile;
                        }
                        else if (!gameDetails.HasScreenshots)
                        {
                            screenshotsFile = Path.Combine(timeStampPath, directoryTimeStamp.Name + ".png");
                            if (File.Exists(screenshotsFile))
                            {
                                gameDetails.HasScreenshots = true;
                                gameDetails.ScreenshotsPath = screenshotsFile;
                            }
                        }
                        else if (!gameDetails.HasScreenshots)
                        {
                            screenshotsFile = Path.Combine(timeStampPath, directoryTimeStamp.Name + ".gif");
                            if (File.Exists(screenshotsFile))
                            {
                                gameDetails.HasScreenshots = true;
                                gameDetails.ScreenshotsPath = screenshotsFile;
                            }
                        }

                        CheckAndUpdateGameDetails(mapData, directoryTimeStamp.Name, gameDetails);
                    }
                }
            }
        }

        private void SetAutoBackupMessageFailure()
        {
            if (_autoBackup)
            {
                if (_waitingOnSlotFolder)
                {
                    Message("Auto backup will start after the first checkpoint", "", MessageType.Warning, MessageMode.User);
                }
                else
                {
                    _autoBackup = false;
                    _slot.checkBoxAutoBackup.Checked = false;
                    Message("Auto backup failed to start", "Hmm :(", MessageType.Error, MessageMode.User);
                }
            }
            else
            {
                _autoBackup = true;
                _slot.checkBoxAutoBackup.Checked = true;
                Message("Auto backup failed to stop", "Hmm :(", MessageType.Error, MessageMode.User);
            }
        }

        private void SetAutoBackupMessageSuccess()
        {
            string message;
            if (_watcherActive)
                message = "Auto backup enabled";
            else
                message = "Auto backup disabled";

            if (_watcherActive && _includeDeath)
                message += ", including death checkpoint";

            if (_watcherActive && _screenshot)
                message += ", with screenshots";

            Message(message, "", MessageType.Information, MessageMode.Status);
        }

        public void SetScreenshotQuality(ScreenshotQuality screenshotQuality)
        {
            switch (screenshotQuality)
            {
                case ScreenshotQuality.Gif:
                    _screenShotExtension = ".gif";
                    _screenShotFormat = ImageFormat.Gif;
                    break;
                case ScreenshotQuality.Jpg:
                    _screenShotExtension = ".jpg";
                    _screenShotFormat = ImageFormat.Jpeg;
                    break;
                case ScreenshotQuality.Png:
                    _screenShotExtension = ".png";
                    _screenShotFormat = ImageFormat.Png;
                    break;
            }
        }

        private bool SetWatcher(bool activate)
        {
            Logger.Log("Slot " + _id + ", SetWatcher: Enter", LogLevel.Debug);

            try
            {
                if (_fileSystemWatcherDoomFolder == null)
                {
                    Logger.Log("Slot " + _id + ", SetWatcher: _fileSystemWatcherDoomFolder is null", LogLevel.Debug);
                    _fileSystemWatcherDoomFolder = new FileSystemWatcher
                    {
                        Path = _savedGameFolder,
                        Filter = _savedGameSlotName,
                        EnableRaisingEvents = false,
                        NotifyFilter = NotifyFilters.DirectoryName
                    };
                    _fileSystemWatcherDoomFolder.Created += FileSystemWatcherDoomFolderOnCreated;
                    _fileSystemWatcherDoomFolder.Deleted += FileSystemWatcherDoomFolderOnDeleted;
                    Logger.Log("Slot " + _id + ", SetWatcher: _fileSystemWatcherDoomFolder created", LogLevel.Debug);

                }
                _fileSystemWatcherDoomFolder.EnableRaisingEvents = activate;

                if (_fileSystemWatcherSlotFolder == null || !Directory.Exists(_savedGameSlotFolder))
                {
                    _waitingOnSlotFolder = activate;
                    Logger.Log("Slot " + _id + ", SetWatcher: waiting on slot folder to be created", LogLevel.Debug);
                    return false;
                }

                if (activate)
                {
                    Logger.Log("Slot " + _id + ", SetWatcher: activating", LogLevel.Debug);
                    _isCheckPoint = false;
                    _isCheckPointAlt = false;
                    _fileSystemWatcherSlotFolder.EnableRaisingEvents = true;
                    _watcherActive = true;
                    Logger.Log("Slot " + _id + ", SetWatcher: activated", LogLevel.Debug);
                }
                else
                {
                    Logger.Log("Slot " + _id + ", SetWatcher: deactivating", LogLevel.Debug);
                    _fileSystemWatcherSlotFolder.EnableRaisingEvents = false;
                    _isCheckPoint = false;
                    _isCheckPointAlt = false;
                    _watcherActive = false;
                    Logger.Log("Slot " + _id + ", SetWatcher: deactivated", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                _watcherActive = false;
                _fileSystemWatcherSlotFolder = null;
                _fileSystemWatcherDoomFolder = null;
                Logger.Log("Slot " + _id + ", SetWatcher: Exception: " + ex.Message, LogLevel.Debug);
                Logger.LogException(ex);
                return false;
            }

            Logger.Log("Slot " + _id + ", SetWatcher: Exit Success", LogLevel.Debug);
            return true;
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
    }
}