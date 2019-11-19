using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using Hg.DoomHistory.Controls;
using Hg.DoomHistory.Types;
using Hg.DoomHistory.Utilities;

namespace Hg.DoomHistory.Forms
{
    public partial class FormSettingsHotKeys : Form
    {
        #region Fields & Properties

        private readonly List<HotKeyToAction> _hotKeyToActions;
        private readonly List<Keys> _keys = new List<Keys>();

        #endregion

        #region Members

        public FormSettingsHotKeys(List<HotKeyToAction> hotKeyToActions)
        {
            InitializeComponent();

            _hotKeyToActions = hotKeyToActions;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            _keys.Clear();
            
            foreach (HotKeyControl hotKeyControl in FormHelper.FindControls<HotKeyControl>(this))
            {
                if (!_keys.Contains(hotKeyControl.Key))
                {
                    _keys.Add(hotKeyControl.Key);
                }
                else
                {
                    MessageBox.Show(string.Format("Duplicate hot keys are not allowed, {0} found twice.", hotKeyControl.Key), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            foreach (HotKeyToAction hotKeyToAction in _hotKeyToActions)
            {
                switch (hotKeyToAction.Action)
                {
                    case HotKeyAction.MapPrevious:
                        hkcMapPrevious.ToHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.MapNext:
                        hkcMapNext.ToHotKeyToAction(hotKeyToAction);
                        break;

                    case HotKeyAction.SaveFirst:
                        hkcSaveFirst.ToHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SaveLast:
                        hkcSaveLast.ToHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SavePrevious:
                        hkcSavePrevious.ToHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SaveNext:
                        hkcSaveNext.ToHotKeyToAction(hotKeyToAction);
                        break;

                    case HotKeyAction.SaveRestore:
                        hkcSaveRestore.ToHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SaveBackup:
                        hkcSaveBackup.ToHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SaveDelete:
                        hkcSaveDelete.ToHotKeyToAction(hotKeyToAction);
                        break;

                    case HotKeyAction.SettingSwitchAutoBackup:
                        hkcSwitchAutoBackup.ToHotKeyToAction(hotKeyToAction);
                        break;
                }
            }
        }

        private void FormSettingsHotKeys_Load(object sender, EventArgs e)
        {
            // Load hot keys into control
            foreach (HotKeyToAction hotKeyToAction in _hotKeyToActions)
            {
                switch (hotKeyToAction.Action)
                {
                    case HotKeyAction.MapPrevious:
                        hkcMapPrevious.FromHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.MapNext:
                        hkcMapNext.FromHotKeyToAction(hotKeyToAction);
                        break;

                    case HotKeyAction.SaveFirst:
                        hkcSaveFirst.FromHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SaveLast:
                        hkcSaveLast.FromHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SavePrevious:
                        hkcSavePrevious.FromHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SaveNext:
                        hkcSaveNext.FromHotKeyToAction(hotKeyToAction);
                        break;

                    case HotKeyAction.SaveRestore:
                        hkcSaveRestore.FromHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SaveBackup:
                        hkcSaveBackup.FromHotKeyToAction(hotKeyToAction);
                        break;
                    case HotKeyAction.SaveDelete:
                        hkcSaveDelete.FromHotKeyToAction(hotKeyToAction);
                        break;

                    case HotKeyAction.SettingSwitchAutoBackup:
                        hkcSwitchAutoBackup.FromHotKeyToAction(hotKeyToAction);
                        break;
                }
            }
        }

        #endregion
    }
}
