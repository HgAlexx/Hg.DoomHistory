using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Hg.DoomHistory.Utilities;

namespace Hg.DoomHistory.Forms
{
    public partial class FormDebugConsole : Form
    {
        #region Fields & Properties

        private List<string> _logs;

        #endregion

        #region Members

        public FormDebugConsole()
        {
            InitializeComponent();
            comboBoxLevel.SelectedIndex = 3; // Debug by default
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Logger.Enabled = false;
            listViewLog.Items.Clear();
            Logger.ClearLogs();
            Logger.Enabled = checkBoxEnable.Checked;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filePath = Path.Combine(folderBrowserDialog.SelectedPath, "Hg.DoomHistory.log");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.WriteAllLines(filePath, _logs);
            }
        }

        private void checkBoxEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxLevel_SelectedIndexChanged(null, null);

            Logger.Enabled = checkBoxEnable.Checked;
            if (Logger.Enabled)
            {
                Logger.OnLog += OnLogEvent;
            }
            else
            {
                Logger.OnLog -= OnLogEvent;
            }
        }

        private void comboBoxLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxLevel.SelectedIndex)
            {
                case 0: // Error
                    Logger.Level = LogLevel.Error;
                    break;
                case 1: // Warning
                    Logger.Level = LogLevel.Warning;
                    break;
                case 2: // Info
                    Logger.Level = LogLevel.Information;
                    break;
                default: // debug
                    Logger.Level = LogLevel.Debug;
                    break;
            }
        }

        private void FormDebugConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Enabled = false;
            Logger.OnLog -= OnLogEvent;

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Visible = false;
            }
        }

        private void LoadListView()
        {
            listViewLog.BeginUpdate();
            int currentCount = listViewLog.Items.Count;
            for (int i = currentCount; i < _logs.Count; i++)
            {
                listViewLog.Items.Add(_logs[i]);
            }

            listViewLog.EndUpdate();
        }

        private void OnLogEvent()
        {
            _logs = Logger.GetLogs();
            if (InvokeRequired)
            {
                Invoke(new Action(LoadListView));
            }
            else
            {
                LoadListView();
            }
        }

        #endregion
    }
}