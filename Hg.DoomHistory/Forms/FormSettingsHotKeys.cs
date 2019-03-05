using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hg.DoomHistory.Types;

namespace Hg.DoomHistory.Forms
{
    public partial class FormSettingsHotKeys : Form
    {
        #region Fields & Properties

        private readonly List<HotKeyToAction> _hotKeyToActions;

        #endregion

        #region Members

        public FormSettingsHotKeys(List<HotKeyToAction> hotKeyToActions)
        {
            InitializeComponent();

            _hotKeyToActions = hotKeyToActions;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
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
                }
            }
        }

        private void FormSettingsHotKeys_Load(object sender, EventArgs e)
        {
            // Load hot keys
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
                }
            }
        }

        #endregion
    }
}