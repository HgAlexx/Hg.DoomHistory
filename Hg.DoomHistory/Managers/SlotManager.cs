using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Hg.DoomHistory.Comparers;
using Hg.DoomHistory.Controls;
using Hg.DoomHistory.Forms;
using Hg.DoomHistory.Types;
using Hg.DoomHistory.Utilities;

namespace Hg.DoomHistory.Managers
{
    public delegate DialogResult MessageEventHandler(string text, string caption, MessageType type, MessageMode mode);

    public class SlotManager
    {
        #region Fields & Properties

        public event MessageEventHandler OnMessage;
        private readonly BackupManager _backupManager;
        private readonly SettingManager _settingManager;

        private readonly TimeStampComparer _timeStampComparer = new TimeStampComparer(SortOrder.Descending);

        private bool _autoBackup;
        private bool _autoBackupSound;
        private bool _autoBackupSelect = true;
        private bool _includeDeath;

        private List<MapData> _maps = new List<MapData>();

        private bool _restoring;
        private bool _screenshot;

        private SlotControl _slot;

        public string SlotName { get; }

        #endregion

        #region Members

        public SlotManager(SlotControl slot, int id, SettingManager settingManager,
            MessageEventHandler messageEventHandler)
        {
            OnMessage += messageEventHandler;

            _slot = slot;

            _settingManager = settingManager;
            _backupManager = new BackupManager(id, _settingManager);

            SlotName = "Slot " + id;

            UnloadControl();

            BindEvent();

            _backupManager.AutoBackupOccurred += () =>
            {
                if (_slot.InvokeRequired)
                {
                    _slot.Invoke(new Action(LoadControl));
                }
                else
                {
                    LoadControl();
                }

                if (_autoBackupSound)
                {
                    SoundManager.PlaySuccess();
                }
            };

            _backupManager.AutoBackupStatusChanged += () =>
            {
                if (_slot.InvokeRequired)
                {
                    _slot.Invoke(new Action(SetAutoBackupMessage));
                }
                else
                {
                    SetAutoBackupMessage();
                }
            };

            _backupManager.ScanBackupFolder();

            LoadControl();

            SetTimeStampSortOrder();

            _settingManager.TimeStampSortOrderChanged += SetTimeStampSortOrder;
        }

        public bool ActionSaveRestore()
        {
            if (_restoring)
            {
                return false;
            }

            _restoring = true;

            try
            {
                GameDetails gameDetails = GetSelectedGameDetails();
                if (gameDetails != null)
                {
                    if (_backupManager.SaveRestore(gameDetails))
                    {
                        Message("The restoration has been successful", "Restoration Complete", MessageType.Information,
                            MessageMode.User);
                        return true;
                    }
                }
                else
                {
                    Message("Nothing to restore", "Nop!", MessageType.Warning, MessageMode.User);
                    return false;
                }
            }
            finally
            {
                _restoring = false;
            }

            Message("The restoration failed :(", "Hmm :(", MessageType.Error, MessageMode.User);
            return false;
        }

        public void MapSelectNext()
        {
            if (_slot.comboBoxMaps.Items.Count <= 0)
            {
                return;
            }

            int current = _slot.comboBoxMaps.SelectedIndex;
            current++;

            if (current >= 0 && current < _slot.comboBoxMaps.Items.Count)
            {
                _slot.comboBoxMaps.SelectedIndex = current;
                ComboBoxMapsOnSelectionChangeCommitted(null, null);
            }
        }

        public void MapSelectPrevious()
        {
            if (_slot.comboBoxMaps.Items.Count <= 0)
            {
                return;
            }

            int current = _slot.comboBoxMaps.SelectedIndex;
            current--;

            if (current >= 0 && current < _slot.comboBoxMaps.Items.Count)
            {
                _slot.comboBoxMaps.SelectedIndex = current;
                ComboBoxMapsOnSelectionChangeCommitted(null, null);
            }
        }

        public void Release()
        {
            _settingManager.TimeStampSortOrderChanged -= SetTimeStampSortOrder;

            UnbindEvent();
            UnloadControl();

            _slot = null;
        }

        public void SaveSelectFirst()
        {
            if (_slot.listViewSavedGames.Items.Count <= 0)
            {
                return;
            }

            _slot.listViewSavedGames.SelectedIndices.Clear();
            _slot.listViewSavedGames.SelectedIndices.Add(0);
            if (_slot.listViewSavedGames.SelectedItems.Count == 1)
            {
                _slot.listViewSavedGames.SelectedItems[0].EnsureVisible();
                _slot.listViewSavedGames.FocusedItem = _slot.listViewSavedGames.SelectedItems[0];
            }

            ListViewSavedGamesOnSelectedIndexChanged(null, null);
        }

        public void SaveSelectLast()
        {
            if (_slot.listViewSavedGames.Items.Count <= 0)
            {
                return;
            }

            int current = _slot.listViewSavedGames.Items.Count - 1;
            if (current >= 0 && current < _slot.listViewSavedGames.Items.Count)
            {
                _slot.listViewSavedGames.SelectedIndices.Clear();
                _slot.listViewSavedGames.SelectedIndices.Add(current);
                if (_slot.listViewSavedGames.SelectedItems.Count == 1)
                {
                    _slot.listViewSavedGames.SelectedItems[0].EnsureVisible();
                    _slot.listViewSavedGames.FocusedItem = _slot.listViewSavedGames.SelectedItems[0];
                }

                ListViewSavedGamesOnSelectedIndexChanged(null, null);
            }
        }

        public void SaveSelectNext()
        {
            if (_slot.listViewSavedGames.Items.Count <= 0)
            {
                return;
            }

            int current = _slot.listViewSavedGames.SelectedIndices.Count > 0
                ? _slot.listViewSavedGames.SelectedIndices[0]
                : -1;
            current++;

            if (current >= _slot.listViewSavedGames.Items.Count)
            {
                current = _slot.listViewSavedGames.Items.Count - 1;
            }

            if (current >= 0 && current < _slot.listViewSavedGames.Items.Count)
            {
                _slot.listViewSavedGames.SelectedIndices.Clear();
                _slot.listViewSavedGames.SelectedIndices.Add(current);
                if (_slot.listViewSavedGames.SelectedItems.Count == 1)
                {
                    _slot.listViewSavedGames.SelectedItems[0].EnsureVisible();
                    _slot.listViewSavedGames.FocusedItem = _slot.listViewSavedGames.SelectedItems[0];
                }

                ListViewSavedGamesOnSelectedIndexChanged(null, null);
            }
        }

        public void SaveSelectPrevious()
        {
            if (_slot.listViewSavedGames.Items.Count <= 0)
            {
                return;
            }

            int current = _slot.listViewSavedGames.SelectedIndices.Count > 0
                ? _slot.listViewSavedGames.SelectedIndices[0]
                : -1;
            current--;

            if (current < 0)
            {
                current = 0;
            }

            if (current >= 0 && current < _slot.listViewSavedGames.Items.Count)
            {
                _slot.listViewSavedGames.SelectedIndices.Clear();
                _slot.listViewSavedGames.SelectedIndices.Add(current);
                if (_slot.listViewSavedGames.SelectedItems.Count == 1)
                {
                    _slot.listViewSavedGames.SelectedItems[0].EnsureVisible();
                    _slot.listViewSavedGames.FocusedItem = _slot.listViewSavedGames.SelectedItems[0];
                }

                ListViewSavedGamesOnSelectedIndexChanged(null, null);
            }
        }

        private void BindEvent()
        {
            _slot.checkBoxAutoBackup.CheckedChanged += CheckBoxAutoBackupOnCheckedChanged;
            _slot.checkBoxIncludeDeath.CheckedChanged += CheckBoxIncludeDeathOnCheckedChanged;
            _slot.checkBoxScreenShot.CheckedChanged += CheckBoxScreenShotOnCheckedChanged;
            _slot.checkBoxSound.CheckStateChanged += CheckBoxSoundOnCheckStateChanged;
            _slot.checkBoxAutoSelect.CheckStateChanged += CheckBoxAutoSelectOnCheckStateChanged;

            _slot.comboBoxMaps.SelectionChangeCommitted += ComboBoxMapsOnSelectionChangeCommitted;

            _slot.listViewSavedGames.SelectedIndexChanged += ListViewSavedGamesOnSelectedIndexChanged;
            _slot.listViewSavedGames.DoubleClick += ListViewSavedGamesOnDoubleClick;

            _slot.contextMenuStripListView.ItemClicked += ContextMenuStripListViewOnItemClicked;

            _slot.buttonBackupNow.Click += ButtonBackupNowOnClick;
            _slot.buttonRestore.Click += ButtonRestoreOnClick;
            _slot.buttonDelete.Click += ButtonDeleteOnClick;

            _slot.propertyGridGameDetails.PropertyValueChanged += PropertyGridGameDetailsOnPropertyValueChanged;
        }

        private void CheckBoxAutoSelectOnCheckStateChanged(object sender, EventArgs e)
        {
            _autoBackupSelect = _slot.checkBoxAutoSelect.Checked;
        }

        private void ButtonBackupNowOnClick(object sender, EventArgs e)
        {
            var screenshotStatus = _backupManager.Screenshot;
            try
            {
                _backupManager.Screenshot = false;
                if (_backupManager.SaveBackup(false))
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
                _backupManager.Screenshot = screenshotStatus;
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
                        if (_backupManager.SaveDelete(gameDetails))
                        {
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
                    Message("The deletion has been successful, " + ok + " items deleted", "Deletion complete",
                        MessageType.Information, MessageMode.User);
                }

                // Refresh list view
                RefreshListView(mapData);
            }
        }

        private void ButtonRestoreOnClick(object sender, EventArgs e)
        {
            ActionSaveRestore();
        }

        private void CheckBoxAutoBackupOnCheckedChanged(object sender, EventArgs e)
        {
            if (_autoBackup != _slot.checkBoxAutoBackup.Checked)
            {
                _autoBackup = _slot.checkBoxAutoBackup.Checked;

                _backupManager.SetAutoBackup(_autoBackup);

                SetAutoBackupMessage();
            }
        }

        private void CheckBoxIncludeDeathOnCheckedChanged(object sender, EventArgs e)
        {
            _includeDeath = _slot.checkBoxIncludeDeath.Checked;
            _backupManager.IncludeDeath = _includeDeath;
            SetAutoBackupMessage();
        }

        private void CheckBoxScreenShotOnCheckedChanged(object sender, EventArgs e)
        {
            _screenshot = _slot.checkBoxScreenShot.Checked;
            _backupManager.Screenshot = _screenshot;
            SetAutoBackupMessage();
            if (_screenshot)
            {
                var doomPtr = BackupManager.GetDoomPtr();
                if (!ScreenShots.HasTitlebar(doomPtr))
                {
                    Message("Screenshot only works if Doom is set to Windowed Mode", "Warning!", MessageType.Error,
                        MessageMode.User);
                }
            }
        }

        private void CheckBoxSoundOnCheckStateChanged(object sender, EventArgs e)
        {
            _autoBackupSound = _slot.checkBoxSound.Checked;
        }

        private void ComboBoxMapsOnSelectionChangeCommitted(object sender, EventArgs e)
        {
            var mapData = GetSelectedMap();
            if (mapData != null)
            {
                RefreshListView(mapData);
            }
        }

        private void ContextMenuStripListViewOnItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _slot.contextMenuStripListView.Hide();
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
                var gameDetails = GetSelectedGameDetails();
                if (gameDetails != null)
                {
                    var formNotes = new FormNotes {textBoxNotes = {Text = gameDetails.Notes}};
                    if (formNotes.ShowDialog(_slot) == DialogResult.OK)
                    {
                        gameDetails.Notes = formNotes.textBoxNotes.Text;
                        var mapData = GetSelectedMap();
                        if (mapData != null)
                        {
                            RefreshListView(mapData);
                        }
                    }
                }
            }
        }

        private List<GameDetails> GetSavedGamePerMap(MapData mapData)
        {
            return _backupManager.SavedGameDetails.FindAll(x => x.MapName == mapData.NameInternal);
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
                    if (mapData != null)
                    {
                        RefreshListView(mapData);
                    }
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
                    {
                        _slot.pictureBoxScreenshot.ImageLocation = gameDetails.ScreenshotsPath;
                    }
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
            {
                return;
            }

            _slot.buttonBackupNow.Enabled = true;
            _slot.checkBoxAutoBackup.Enabled = true;
            _slot.comboBoxMaps.Enabled = true;

            // Map list
            _slot.comboBoxMaps.BeginUpdate();

            var selection = _slot.comboBoxMaps.SelectedItem as MapData;

            _slot.comboBoxMaps.Items.Clear();

            _maps = _backupManager.Maps;
            foreach (var mapData in _maps)
            {
                _slot.comboBoxMaps.Items.Add(mapData);
            }

            if (selection != null && _slot.comboBoxMaps.Items.Contains(selection))
            {
                _slot.comboBoxMaps.SelectedItem = selection;
            }
            else
            {
                if (_slot.comboBoxMaps.Items.Count > 0 && _slot.checkBoxAutoSelect.Checked)
                {
                    _slot.comboBoxMaps.SelectedIndex = _slot.comboBoxMaps.Items.Count - 1; // Select last map
                }
            }

            if (_backupManager.LastGameDetails != null && _slot.checkBoxAutoSelect.Checked)
            {
                var gameDetails = _backupManager.LastGameDetails;
                selection = _maps.FirstOrDefault(o => o.NameInternal == gameDetails.MapName);
                if (selection != null && _slot.comboBoxMaps.Items.Contains(selection))
                {
                    _slot.comboBoxMaps.SelectedItem = selection;
                }
            }

            _slot.comboBoxMaps.EndUpdate();

            ComboBoxMapsOnSelectionChangeCommitted(_slot.comboBoxMaps, null);

            _slot.checkBoxAutoBackup.Checked = _autoBackup;
            _slot.checkBoxIncludeDeath.Checked = _includeDeath;
            _slot.checkBoxSound.Checked = _autoBackupSound;
            _slot.checkBoxAutoSelect.Checked = _autoBackupSelect;
        }

        private DialogResult Message(string text, string caption, MessageType type, MessageMode mode)
        {
            var msg = SlotName + ": " + text;
            Logger.Log(text, LogLevel.Debug);
            if (OnMessage != null)
            {
                return OnMessage(msg, caption, type, mode);
            }

            return DialogResult.None;
        }

        private void PropertyGridGameDetailsOnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var propertyDescriptor = e.ChangedItem.PropertyDescriptor;
            if (propertyDescriptor != null && propertyDescriptor.Name == "Notes")
            {
                var mapData = GetSelectedMap();
                if (mapData != null)
                {
                    RefreshListView(mapData);
                }
            }
        }

        private void RefreshListView(MapData mapData)
        {
            _slot.listViewSavedGames.BeginUpdate();

            List<GameDetails> savedGames = GetSavedGamePerMap(mapData);
            savedGames.Sort((data1, data2) => _timeStampComparer.Compare(data1, data2));

            var selected = -1;
            if (_slot.listViewSavedGames.SelectedItems.Count > 0)
            {
                selected = _slot.listViewSavedGames.SelectedIndices[0];
            }

            _slot.listViewSavedGames.Items.Clear();

            foreach (var gameDetails in savedGames)
            {
                var listViewItem = new ListViewItem {Text = gameDetails.DateTimeString};
                listViewItem.SubItems.Add(gameDetails.DiffString);
                listViewItem.SubItems.Add(gameDetails.PlayedTime.ToString("g"));
                listViewItem.SubItems.Add(gameDetails.IsDeath ? "🕱" : "");
                listViewItem.SubItems.Add(gameDetails.Notes);

                listViewItem.Tag = gameDetails;

                _slot.listViewSavedGames.Items.Add(listViewItem);
            }

            _slot.listViewSavedGames.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            _slot.listViewSavedGames.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            _slot.listViewSavedGames.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize);
            _slot.listViewSavedGames.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent);

            if (_backupManager.LastGameDetails != null && _slot.checkBoxAutoSelect.Checked)
            {
                var lastGameDetails = _backupManager.LastGameDetails;
                foreach (ListViewItem item in _slot.listViewSavedGames.Items)
                {
                    if (item.Tag == lastGameDetails)
                    {
                        _slot.listViewSavedGames.SelectedIndices.Clear();
                        item.Selected = true;
                        item.Focused = true;
                        _backupManager.LastGameDetails = null;
                        break;
                    }
                }
            }

            if (_slot.listViewSavedGames.Items.Count > 0 && _slot.listViewSavedGames.SelectedItems.Count == 0)
            {
                if (selected < 0)
                {
                    selected = 0;
                }

                if (selected >= _slot.listViewSavedGames.Items.Count)
                {
                    selected = _slot.listViewSavedGames.Items.Count - 1;
                }

                _slot.listViewSavedGames.SelectedIndices.Add(selected);
            }

            if (_slot.listViewSavedGames.SelectedItems.Count == 1)
            {
                _slot.listViewSavedGames.SelectedItems[0].EnsureVisible();
            }

            _slot.listViewSavedGames.EndUpdate();

            ListViewSavedGamesOnSelectedIndexChanged(null, null);
        }

        private void SetAutoBackupMessage()
        {
            if (_backupManager.AutoBackupStatus == AutoBackupStatus.Enabled)
            {
                SetAutoBackupMessageSuccess();
            }
            else
            {
                SetAutoBackupMessageFailure();
            }

            _slot.checkBoxIncludeDeath.Enabled = _slot.checkBoxAutoBackup.Checked;
            _slot.checkBoxScreenShot.Enabled = _slot.checkBoxAutoBackup.Checked;
            _slot.checkBoxSound.Enabled = _slot.checkBoxAutoBackup.Checked;
            _slot.checkBoxAutoSelect.Enabled = _slot.checkBoxAutoBackup.Checked;
        }

        private void SetAutoBackupMessageFailure()
        {
            if (_autoBackup)
            {
                if (_backupManager.AutoBackupStatus == AutoBackupStatus.Waiting)
                {
                    Message("Auto backup will start after the first checkpoint", "", MessageType.Warning,
                        MessageMode.User);
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
                if (_backupManager.AutoBackupStatus == AutoBackupStatus.Disabled)
                {
                    Message("Auto backup disabled", "", MessageType.Information, MessageMode.Status);
                }
                else
                {
                    _autoBackup = true;
                    _slot.checkBoxAutoBackup.Checked = true;
                    Message("Auto backup failed to stop", "Hmm :(", MessageType.Error, MessageMode.User);
                }
            }
        }

        private void SetAutoBackupMessageSuccess()
        {
            bool enabled = _backupManager.AutoBackupStatus == AutoBackupStatus.Enabled;
            string message;
            if (enabled)
            {
                message = "Auto backup enabled";
            }
            else
            {
                message = "Auto backup disabled";
            }

            if (enabled && _includeDeath)
            {
                message += ", including death checkpoint";
            }

            if (enabled && _screenshot)
            {
                message += ", with screenshots";
            }

            Message(message, "", MessageType.Information, MessageMode.Status);
        }

        private void SetTimeStampSortOrder()
        {
            _timeStampComparer.SortAscending = _settingManager.TimeStampSortOrder;
            ComboBoxMapsOnSelectionChangeCommitted(null, null);
        }

        private void UnbindEvent()
        {
            _slot.checkBoxAutoBackup.CheckedChanged -= CheckBoxAutoBackupOnCheckedChanged;
            _slot.checkBoxIncludeDeath.CheckedChanged -= CheckBoxIncludeDeathOnCheckedChanged;
            _slot.checkBoxScreenShot.CheckedChanged -= CheckBoxScreenShotOnCheckedChanged;
            _slot.checkBoxSound.CheckStateChanged -= CheckBoxSoundOnCheckStateChanged;

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
            _slot.checkBoxSound.Checked = false;
            _slot.checkBoxAutoSelect.Checked = true;

            _slot.checkBoxAutoBackup.Enabled = false;
            _slot.checkBoxIncludeDeath.Enabled = false;
            _slot.checkBoxScreenShot.Enabled = false;
            _slot.checkBoxSound.Enabled = false;
            _slot.checkBoxAutoSelect.Enabled = false;

            _slot.comboBoxMaps.Items.Clear();
            _slot.comboBoxMaps.Enabled = false;

            _slot.listViewSavedGames.Items.Clear();

            _slot.propertyGridGameDetails.SelectedObject = null;
        }

        #endregion
    }
}