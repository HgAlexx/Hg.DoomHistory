using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using Hg.DoomHistory.Comparers;
using Hg.DoomHistory.Types;
using Hg.DoomHistory.Utilities;

namespace Hg.DoomHistory.Managers
{
    public enum AutoBackupStatus
    {
        None,
        Disabled,
        Enabled,
        Waiting
    }

    public delegate void BackupOccuredEventHandler(bool success);

    public delegate void BackupStatusChangedEventHandler();

    public class BackupManager
    {
        #region Fields & Properties

        public event BackupOccuredEventHandler AutoBackupOccurred;
        public event BackupStatusChangedEventHandler AutoBackupStatusChanged;
        private readonly string _backupSlotPath;
        private readonly int _id;
        private readonly MapComparer _mapComparer = new MapComparer();

        private readonly object _restoreSaveLock = new object();
        private readonly string _savedGameSlotFolder;
        private readonly string _savedGameSlotName;
        private readonly SettingManager _settingManager;

        private AutoBackupStatus _autoBackupEnabled;
        private string _checkpointBuffer = "";

        private DateTime? _checkpointStartTime;
        private bool _exiting;

        private FileSystemWatcher _fileSystemWatcherSavedGameFolder;
        private FileSystemWatcher _fileSystemWatcherSlotFolder;

        private bool _isCheckPoint;
        private bool _isCheckPointAlt;
        private bool _isCheckPointMapStart;
        private bool _isGameDuration;

        private string _screenShotExtension = ".jpg";
        private ImageFormat _screenShotFormat = ImageFormat.Jpeg;

        public AutoBackupStatus AutoBackupStatus
        {
            get => _autoBackupEnabled;
            set
            {
                _autoBackupEnabled = value;
                AutoBackupStatusChanged?.Invoke();
            }
        }

        public bool IncludeDeath { get; set; }

        public GameDetails LastGameDetails { get; set; }

        public List<MapData> Maps
        {
            get
            {
                List<MapData> maps = SavedGameDetails.Select(gameDetails =>
                    new MapData(gameDetails.MapDesc, gameDetails.MapSafe, gameDetails.MapName)).Distinct().ToList();
                maps.Sort((data1, data2) => _mapComparer.Compare(data1, data2));
                return maps;
            }
        }

        public List<GameDetails> SavedGameDetails { get; }

        public bool Screenshot { get; set; }

        #endregion

        #region Members

        public BackupManager(int slotId, SettingManager settingManager)
        {
            _exiting = false;
            SavedGameDetails = new List<GameDetails>();

            _id = slotId;
            _settingManager = settingManager;

            _savedGameSlotName = "GAME-AUTOSAVE" + (_id - 1);
            _savedGameSlotFolder = Path.Combine(_settingManager.SavedGameFolder, _savedGameSlotName);
            _backupSlotPath = Path.Combine(_settingManager.BackupFolder, "Slot" + _id);

            SetScreenshotQuality();

            _settingManager.ScreenshotQualityChanged += SetScreenshotQuality;
        }

        public static IntPtr GetDoomPtr()
        {
            var doomPtr = IntPtr.Zero;
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName == "DOOMx64" || process.ProcessName == "DOOMx64vk")
                {
                    doomPtr = process.MainWindowHandle;
                    break;
                }
            }

            return doomPtr;
        }

        public void Release()
        {
            _exiting = true;

            AutoBackupOccurred = null;

            if (_fileSystemWatcherSavedGameFolder != null)
            {
                _fileSystemWatcherSavedGameFolder.EnableRaisingEvents = false;
            }

            _fileSystemWatcherSavedGameFolder = null;
            if (_fileSystemWatcherSlotFolder != null)
            {
                _fileSystemWatcherSlotFolder.EnableRaisingEvents = false;
            }

            _fileSystemWatcherSlotFolder = null;
        }

        public bool SaveBackup(bool isDeath)
        {
            Logger.Log("Slot " + _id + ", BackupSave: Enter", LogLevel.Debug);

            if (!Directory.Exists(_savedGameSlotFolder))
            {
                Logger.Log("Slot " + _id + ", BackupSave: _savedGameSlotFolder does not exist", LogLevel.Debug);
                return false;
            }

            var gameDetailsFilePath = Path.Combine(_savedGameSlotFolder, "game.details");
            if (!File.Exists(gameDetailsFilePath))
            {
                Logger.Log("Slot " + _id + ", BackupSave: gameDetailsFilePath does not exist", LogLevel.Debug);
                return false;
            }

            var gameDetailsContent = File.ReadAllText(gameDetailsFilePath);
            var gameDetails = new GameDetails(gameDetailsContent) {IsDeath = isDeath};

            var mapPath = Path.Combine(_backupSlotPath, gameDetails.MapSafe);
            Directory.CreateDirectory(mapPath);

            var timeStampFolderName = gameDetails.DateTimeSafe;
            var timestampPath = Path.Combine(mapPath, timeStampFolderName);
            Directory.CreateDirectory(timestampPath);

            gameDetails.SetPath(timestampPath);

            var savedGamePath = Path.Combine(timestampPath, _savedGameSlotName);
            Directory.CreateDirectory(savedGamePath);

            var source = new DirectoryInfo(_savedGameSlotFolder);
            var target = new DirectoryInfo(savedGamePath);

            int tries = 0;
            while (tries < 10)
            {
                bool needToWait = false;
                foreach (var fileInfo in source.GetFiles())
                {
                    if (fileInfo.Name.EndsWith(".temp") || fileInfo.Name.EndsWith(".temp.verify"))
                    {
                        Logger.Log("Slot " + _id + ", BackupSave: a temp file is still present, wait a bit",
                            LogLevel.Debug);
                        needToWait = true;
                        break;
                    }
                }

                if (needToWait)
                {
                    tries++;
                    Thread.Sleep(100);
                }
                else
                {
                    break;
                }
            }

            try
            {
                foreach (var fileInfo in source.GetFiles())
                {
                    if (!fileInfo.Name.EndsWith(".temp") && !fileInfo.Name.EndsWith(".temp.verify"))
                    {
                        fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("Slot " + _id + ", BackupSave: " + exception.Message, LogLevel.Error);
                return false;
            }

            if (isDeath)
            {
                var markerFile = Path.Combine(timestampPath, ".hg.death");
                File.WriteAllText(markerFile, @"甘き死よ、来たれ");
            }

            if (Screenshot)
            {
                var doomPtr = GetDoomPtr();
                if (doomPtr != IntPtr.Zero)
                {
                    Logger.Log("Slot " + _id + ", BackupSave: Doom is running", LogLevel.Debug);
                    try
                    {
                        var screenshotsFile = Path.Combine(timestampPath, timeStampFolderName + _screenShotExtension);
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

            CheckAndUpdateGameDetails(gameDetails);
            LastGameDetails = gameDetails;

            Logger.Log("Slot " + _id + ", BackupSave: Exit, OK", LogLevel.Debug);
            return true;
        }

        public bool SaveDelete(GameDetails gameDetails)
        {
            if (gameDetails == null)
            {
                return false;
            }

            try
            {
                string timeStampPath = gameDetails.Path;
                if (Directory.Exists(timeStampPath))
                {
                    Directory.Delete(timeStampPath, true);
                }

                if (SavedGameDetails.Contains(gameDetails))
                {
                    SavedGameDetails.Remove(gameDetails);
                }

                if (LastGameDetails == gameDetails)
                {
                    LastGameDetails = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Slot " + _id + ", DeleteSave: Exception: " + ex.Message, LogLevel.Debug);
                Logger.LogException(ex);
                return false;
            }
        }

        public bool SaveRestore(GameDetails gameDetails)
        {
            var sourcePath = Path.Combine(gameDetails.Path, _savedGameSlotName);
            var targetPath = Path.Combine(_settingManager.SavedGameFolder, _savedGameSlotName);

            lock (_restoreSaveLock)
            {
                if (!Directory.Exists(targetPath))
                {
                    return false;
                }

                if (!Directory.Exists(sourcePath))
                {
                    return false;
                }

                var source = new DirectoryInfo(sourcePath);
                var target = new DirectoryInfo(targetPath);
                var canRestore = false;

                if (_autoBackupEnabled == AutoBackupStatus.Enabled || _autoBackupEnabled == AutoBackupStatus.Waiting)
                {
                    SetWatchers(false);
                    Thread.Sleep(250);
                }

                try
                {
                    foreach (var fileInfo in target.GetFiles())
                    {
                        fileInfo.MoveTo(fileInfo.FullName + ".hg.bak");
                    }

                    Thread.Sleep(250);

                    canRestore = true;

                    foreach (var fileInfo in source.GetFiles())
                    {
                        fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
                    }

                    Thread.Sleep(250);

                    foreach (var fileInfo in target.GetFiles())
                    {
                        if (fileInfo.Name.EndsWith(".hg.bak"))
                        {
                            fileInfo.Delete();
                            canRestore = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (canRestore)
                    {
                        // Clean up
                        foreach (var fileInfo in target.GetFiles())
                        {
                            if (!fileInfo.Name.EndsWith(".hg.bak"))
                            {
                                fileInfo.Delete();
                            }
                        }

                        // Restore backup
                        foreach (var fileInfo in target.GetFiles())
                        {
                            if (fileInfo.Name.EndsWith(".hg.bak"))
                            {
                                fileInfo.MoveTo(fileInfo.FullName.Replace(".hg.bak", ""));
                            }
                        }
                    }

                    Logger.LogException(ex);
                    return false;
                }

                if (_autoBackupEnabled == AutoBackupStatus.Enabled || _autoBackupEnabled == AutoBackupStatus.Waiting)
                {
                    SetWatchers(true);
                    Thread.Sleep(250);
                }

                return true;
            }
        }

        public void ScanBackupFolder()
        {
            if (!Directory.Exists(_settingManager.BackupFolder))
            {
                return;
            }

            DirectoryInfo directoryInfo;

            if (!Directory.Exists(_backupSlotPath))
            {
                directoryInfo = Directory.CreateDirectory(_backupSlotPath);
            }
            else
            {
                directoryInfo = new DirectoryInfo(_backupSlotPath);
            }

            // loop through maps
            foreach (var directoryMap in directoryInfo.GetDirectories())
            {
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

                            var gameDetails = new GameDetails(content);
                            gameDetails.SetPath(timeStampPath);

                            var markerFile = Path.Combine(timeStampPath, ".hg.death");
                            if (File.Exists(markerFile))
                            {
                                gameDetails.IsDeath = true;
                            }

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

                            CheckAndUpdateGameDetails(gameDetails);
                        }
                    }
                }
            }
        }

        public AutoBackupStatus SetAutoBackup(bool enable)
        {
            _autoBackupEnabled = SetWatchers(enable);
            return _autoBackupEnabled;
        }

        private void CheckAndUpdateGameDetails(GameDetails inGameDetails)
        {
            var gameDetails = SavedGameDetails.FirstOrDefault(o => o.DateTimeSafe == inGameDetails.DateTimeSafe);
            if (gameDetails != null)
            {
                SavedGameDetails.Remove(gameDetails);
            }

            SavedGameDetails.Add(inGameDetails);
        }

        private void FileSystemWatcherSavedGameFolderOnCreated(object sender, FileSystemEventArgs e)
        {
            Logger.Log("Slot " + _id + ", FileSystemWatcherSavedGameFolderOnCreated: Slot folder created",
                LogLevel.Debug);

            if (_exiting)
            {
                return;
            }

            if (_autoBackupEnabled == AutoBackupStatus.Waiting)
            {
                MakeSlotWatcher();
                SetSlotWatcher(true);
                AutoBackupStatus = AutoBackupStatus.Enabled;
            }
        }

        private void FileSystemWatcherSavedGameFolderOnDeleted(object sender, FileSystemEventArgs e)
        {
            Logger.Log("Slot " + _id + ", FileSystemWatcherSavedGameFolderOnCreated: Slot folder deleted",
                LogLevel.Debug);

            if (_exiting)
            {
                return;
            }

            if (_fileSystemWatcherSlotFolder != null)
            {
                if (_autoBackupEnabled == AutoBackupStatus.Enabled)
                {
                    SetSlotWatcher(false);
                }

                _fileSystemWatcherSlotFolder = null;
            }

            if (_autoBackupEnabled == AutoBackupStatus.Enabled)
            {
                AutoBackupStatus = AutoBackupStatus.Waiting;
            }
        }

        private void FileSystemWatcherSlotFolderOnRenamed(object sender, RenamedEventArgs e)
        {
            if (_exiting)
            {
                return;
            }

            try
            {
                // Fail-safe: check if Doom is running for auto-backup
                var doomPtr = GetDoomPtr();
                if (doomPtr == IntPtr.Zero)
                {
                    Logger.Log(
                        "Slot " + _id +
                        ", FileSystemWatcherSlotFolderOnRenamed: Change detected but Doom is not running :(",
                        LogLevel.Debug);
                    return;
                }

                if (_checkpointStartTime != null)
                {
                    TimeSpan timeSpan = DateTime.UtcNow.Subtract(_checkpointStartTime.Value);
                    if (timeSpan.TotalSeconds >= 3)
                    {
                        Logger.Log(
                            "Slot " + _id +
                            ", FileSystemWatcherSlotFolderOnRenamed: Too much time since last event, _checkpointBuffer was " +
                            _checkpointBuffer, LogLevel.Debug);

                        // Too much time since last event, reset states
                        _checkpointBuffer = "";
                        _checkpointStartTime = null;
                        _isCheckPoint = false;
                        _isCheckPointAlt = false;
                        _isGameDuration = false;
                        _isCheckPointMapStart = false;
                    }
                }

                if (!_isGameDuration && e.Name == "game_duration.dat")
                {
                    _isGameDuration = true;
                    _checkpointBuffer += "G";
                }

                if (!_isCheckPointMapStart && e.Name == "checkpoint_mapstart.dat")
                {
                    _isCheckPointMapStart = true;
                    _checkpointBuffer += "M";
                }

                // Mostly in case of death
                if (!_isCheckPointAlt && e.Name == "checkpoint_alt.dat")
                {
                    _isCheckPointAlt = true;
                    _checkpointBuffer += "A";
                }

                // Proper checkpoint
                if (!_isCheckPoint && e.Name == "checkpoint.dat")
                {
                    _isCheckPoint = true;
                    _checkpointBuffer += "C";
                }

                // On first event, save time
                if (_checkpointStartTime == null && !string.IsNullOrEmpty(_checkpointBuffer))
                {
                    _checkpointStartTime = DateTime.UtcNow;
                }

                // End of save event
                if (e.Name == "game.details.verify")
                {
                    Logger.Log(
                        "Slot " + _id +
                        ", FileSystemWatcherSlotFolderOnRenamed: game.details.verify event, _checkpointBuffer = " +
                        _checkpointBuffer, LogLevel.Debug);

                    if (_checkpointBuffer.StartsWith("CGMA"))
                    {
                        Logger.Log("Slot " + _id + ", FileSystemWatcherSlotFolderOnRenamed: map change",
                            LogLevel.Debug);
                        // Map change checkpoint (usually)
                        // do nothing
                    }
                    else if (_checkpointBuffer.StartsWith("CMGA"))
                    {
                        Logger.Log("Slot " + _id + ", FileSystemWatcherSlotFolderOnRenamed: normal checkpoint",
                            LogLevel.Debug);

                        // Normal checkpoint (usually)
                        if (SaveBackup(false))
                        {
                            AutoBackupOccurred?.Invoke(true);
                        }
                        else
                        {
                            AutoBackupOccurred?.Invoke(false);
                        }
                    }
                    else if (_checkpointBuffer == "GA" && IncludeDeath)
                    {
                        Logger.Log("Slot " + _id + ", FileSystemWatcherSlotFolderOnRenamed: death checkpoint",
                            LogLevel.Debug);

                        // Death checkpoint (usually)
                        if (SaveBackup(true))
                        {
                            AutoBackupOccurred?.Invoke(true);
                        }
                        else
                        {
                            AutoBackupOccurred?.Invoke(false);
                        }
                    }

                    _isCheckPoint = false;
                    _isCheckPointAlt = false;
                    _isGameDuration = false;
                    _isCheckPointMapStart = false;

                    _checkpointBuffer = "";
                    _checkpointStartTime = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Slot " + _id + ", FileSystemWatcherSlotFolderOnRenamed: Exception: " + ex.Message,
                    LogLevel.Debug);
                Logger.LogException(ex);
            }
        }

        private void MakeSlotWatcher()
        {
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

        private void SetScreenshotQuality()
        {
            switch (_settingManager.ScreenshotQuality)
            {
                case ScreenshotQuality.Gif:
                    _screenShotExtension = ".gif";
                    _screenShotFormat = ImageFormat.Gif;
                    break;
                case ScreenshotQuality.None:
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

        private void SetSlotWatcher(bool enable)
        {
            if (_fileSystemWatcherSlotFolder == null)
            {
                return;
            }

            _isCheckPoint = false;
            _isCheckPointAlt = false;
            _isGameDuration = false;
            _isCheckPointMapStart = false;

            if (enable)
            {
                Logger.Log("Slot " + _id + ", SetWatcher: activating", LogLevel.Debug);
                _fileSystemWatcherSlotFolder.EnableRaisingEvents = true;
                Logger.Log("Slot " + _id + ", SetWatcher: activated", LogLevel.Debug);
            }
            else
            {
                Logger.Log("Slot " + _id + ", SetWatcher: deactivating", LogLevel.Debug);
                _fileSystemWatcherSlotFolder.EnableRaisingEvents = false;
                Logger.Log("Slot " + _id + ", SetWatcher: deactivated", LogLevel.Debug);
            }
        }

        private AutoBackupStatus SetWatchers(bool enable)
        {
            if (enable)
            {
                try
                {
                    if (_fileSystemWatcherSavedGameFolder == null)
                    {
                        Logger.Log("Slot " + _id + ", SetWatchers: _fileSystemWatcherSavedGameFolder is null",
                            LogLevel.Debug);
                        _fileSystemWatcherSavedGameFolder = new FileSystemWatcher
                        {
                            Path = _settingManager.SavedGameFolder,
                            Filter = _savedGameSlotName,
                            EnableRaisingEvents = false,
                            NotifyFilter = NotifyFilters.DirectoryName
                        };
                        _fileSystemWatcherSavedGameFolder.Created += FileSystemWatcherSavedGameFolderOnCreated;
                        _fileSystemWatcherSavedGameFolder.Deleted += FileSystemWatcherSavedGameFolderOnDeleted;
                        Logger.Log("Slot " + _id + ", SetWatchers: _fileSystemWatcherSavedGameFolder created",
                            LogLevel.Debug);
                    }

                    _fileSystemWatcherSavedGameFolder.EnableRaisingEvents = true;

                    if (!Directory.Exists(_savedGameSlotFolder))
                    {
                        if (_fileSystemWatcherSlotFolder != null)
                        {
                            _fileSystemWatcherSlotFolder.EnableRaisingEvents = false;
                        }

                        _fileSystemWatcherSlotFolder = null;

                        Logger.Log("Slot " + _id + ", SetWatchers: waiting on slot folder to be created",
                            LogLevel.Debug);
                        return AutoBackupStatus.Waiting;
                    }

                    if (_fileSystemWatcherSlotFolder == null)
                    {
                        MakeSlotWatcher();
                    }

                    SetSlotWatcher(true);

                    return AutoBackupStatus.Enabled;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }

                return AutoBackupStatus.Disabled;
            }

            if (_fileSystemWatcherSlotFolder != null)
            {
                _fileSystemWatcherSlotFolder.EnableRaisingEvents = false;
            }

            if (_fileSystemWatcherSavedGameFolder != null)
            {
                _fileSystemWatcherSavedGameFolder.EnableRaisingEvents = false;
            }

            return AutoBackupStatus.Disabled;
        }

        #endregion
    }
}