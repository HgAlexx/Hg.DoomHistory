//
// File imported from my old Hg.Common project
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Hg.DoomHistory.Types;

namespace Hg.DoomHistory.Forms
{
    public partial class FormException : Form
    {
        #region Fields & Properties

        private List<Error> _errorDetails = new List<Error>();

        public List<Error> ErrorDetails
        {
            get => _errorDetails;
            private set => _errorDetails = value;
        }

        #endregion

        #region Members

        public FormException()
        {
            InitializeComponent();
        }

        public void LoadCombobox()
        {
            comboBoxErrors.Items.Clear();

            for (var i = 0; i < _errorDetails.Count; i++)
            {
                var error = _errorDetails[i];
                comboBoxErrors.Items.Add(error.Title);
            }

            textBoxErrorCount.Text = _errorDetails.Count.ToString();
        }

        private void ButtonContinue_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ButtonSaveErrorsToFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                string content = "";
                foreach (Error errorDetail in ErrorDetails)
                {
                    content += errorDetail.Title + Environment.NewLine;
                    content += errorDetail.Content + Environment.NewLine;
                    content += "----------------------------------------" + Environment.NewLine;
                }

                string filePath = Path.Combine(folderBrowserDialog.SelectedPath, "Hg.DoomHistory.Errors.log");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.WriteAllText(filePath, content);
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/HgAlexx/Hg.DoomHistory/issues/new");
        }

        private void ComboBoxErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxErrors.SelectedIndex >= 0)
            {
                var error = _errorDetails[comboBoxErrors.SelectedIndex];
                textBoxDetail.Text = error.Content;
            }
        }

        #endregion
    }
}