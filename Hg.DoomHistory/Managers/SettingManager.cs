﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Hg.DoomHistory.Properties;
using Hg.DoomHistory.Types;

namespace Hg.DoomHistory.Managers
{
    public delegate void SettingEventHandler();

    public class SettingManager
    {
        #region Fields & Properties

        public event SettingEventHandler BackupFolderChanged;
        public event SettingEventHandler HotKeysActiveChanged;
        public event SettingEventHandler HotKeysSoundChanged;
        public event SettingEventHandler NotificationModeChanged;
        public event SettingEventHandler SavedGameFolderChanged;
        public event SettingEventHandler ScreenshotQualityChanged;
        public event SettingEventHandler TimeStampSortOrderChanged;

        private string _backupFolder;
        private bool _hotKeysActive;
        private bool _hotKeysSound;
        private MessageMode _notificationMode;
        private string _savedGameFolder;
        private ScreenshotQuality _screenshotQuality;
        private SortOrder _timeStampSortOrder;

        public string BackupFolder
        {
            get => _backupFolder;
            set
            {
                _backupFolder = value;
                BackupFolderChanged?.Invoke();
            }
        }

        public bool HotKeysActive
        {
            get => _hotKeysActive;
            set
            {
                _hotKeysActive = value;
                HotKeysActiveChanged?.Invoke();
            }
        }

        public bool HotKeysSound
        {
            get => _hotKeysSound;
            set
            {
                _hotKeysSound = value;
                HotKeysSoundChanged?.Invoke();
            }
        }

        public List<HotKeyToAction> HotKeyToActions { get; set; }

        public MessageMode NotificationMode
        {
            get => _notificationMode;
            set
            {
                _notificationMode = value;
                NotificationModeChanged?.Invoke();
            }
        }

        public string SavedGameFolder
        {
            get => _savedGameFolder;
            set
            {
                _savedGameFolder = value;
                SavedGameFolderChanged?.Invoke();
            }
        }

        public ScreenshotQuality ScreenshotQuality
        {
            get => _screenshotQuality;
            set
            {
                _screenshotQuality = value;
                ScreenshotQualityChanged?.Invoke();
            }
        }

        public SortOrder TimeStampSortOrder
        {
            get => _timeStampSortOrder;
            set
            {
                _timeStampSortOrder = value;
                TimeStampSortOrderChanged?.Invoke();
            }
        }

        #endregion

        #region Members

        public SettingManager()
        {
            HotKeyToActions = new List<HotKeyToAction>();
            ResetSettings();
        }

        public void LoadSettings()
        {
            BackupFolder = Settings.Default.BackupFolder;
            SavedGameFolder = Settings.Default.SavedGameFolder;
            NotificationMode = (MessageMode) Settings.Default.NotificationMode;
            ScreenshotQuality = (ScreenshotQuality) Settings.Default.ScreenshotQuality;
            TimeStampSortOrder = (SortOrder) Settings.Default.TimeStampSortOrder;
            HotKeysActive = Settings.Default.HotKeysActive;
            HotKeysSound = Settings.Default.HotKeysSound;

            if (NotificationMode == MessageMode.None)
            {
                NotificationMode = MessageMode.MessageBox;
            }

            if (ScreenshotQuality == ScreenshotQuality.None)
            {
                ScreenshotQuality = ScreenshotQuality.Jpg;
            }

            if (TimeStampSortOrder == SortOrder.None)
            {
                TimeStampSortOrder = SortOrder.Ascending;
            }

            if (!Directory.Exists(SavedGameFolder))
            {
                SavedGameFolder = "";
            }

            if (!Directory.Exists(BackupFolder))
            {
                BackupFolder = "";
            }

            string allHotKeys = Settings.Default.HotKeysToActions;
            List<string> hotKeys = allHotKeys.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (hotKeys.Count > 0)
            {
                HotKeyToActions.Clear();
                foreach (string hotKey in hotKeys)
                {
                    HotKeyToActions.Add(HotKeyToAction.DeserializeFromString(hotKey));
                }
            }
        }

        public void ResetSettings()
        {
            BackupFolder = "";
            SavedGameFolder = "";
            NotificationMode = MessageMode.MessageBox;
            ScreenshotQuality = ScreenshotQuality.Jpg;
            TimeStampSortOrder = SortOrder.Descending;
            HotKeysActive = false;
            HotKeysSound = false;

            HotKeyToActions.Clear();
            HotKeyToActions.Add(new HotKeyToAction
                {Enabled = true, Action = HotKeyAction.SavePrevious, HotKey = new HotKey(Keys.Up, true, true, false)});
            HotKeyToActions.Add(new HotKeyToAction
                {Enabled = true, Action = HotKeyAction.SaveNext, HotKey = new HotKey(Keys.Down, true, true, false)});
            HotKeyToActions.Add(new HotKeyToAction
                {Enabled = true, Action = HotKeyAction.MapPrevious, HotKey = new HotKey(Keys.Left, true, true, false)});
            HotKeyToActions.Add(new HotKeyToAction
                {Enabled = true, Action = HotKeyAction.MapNext, HotKey = new HotKey(Keys.Right, true, true, false)});
            HotKeyToActions.Add(new HotKeyToAction
                {Enabled = true, Action = HotKeyAction.SaveFirst, HotKey = new HotKey(Keys.PageUp, true, true, false)});
            HotKeyToActions.Add(new HotKeyToAction
            {
                Enabled = true, Action = HotKeyAction.SaveLast, HotKey = new HotKey(Keys.PageDown, true, true, false)
            });
            HotKeyToActions.Add(new HotKeyToAction
            {
                Enabled = true, Action = HotKeyAction.SaveRestore, HotKey = new HotKey(Keys.Insert, true, true, false)
            });
        }

        public void SaveSettings()
        {
            Settings.Default.BackupFolder = BackupFolder;
            Settings.Default.SavedGameFolder = SavedGameFolder;
            Settings.Default.NotificationMode = (int) NotificationMode;
            Settings.Default.ScreenshotQuality = (int) ScreenshotQuality;
            Settings.Default.TimeStampSortOrder = (int) TimeStampSortOrder;
            Settings.Default.HotKeysActive = HotKeysActive;
            Settings.Default.HotKeysSound = HotKeysSound;

            List<string> hotKeys = new List<string>();
            foreach (HotKeyToAction hotKeyToAction in HotKeyToActions)
            {
                hotKeys.Add(HotKeyToAction.SerializeToString(hotKeyToAction));
            }

            string allHotKeys = string.Join("|", hotKeys);
            Settings.Default.HotKeysToActions = allHotKeys;

            Settings.Default.Save();
        }

        #endregion
    }
}