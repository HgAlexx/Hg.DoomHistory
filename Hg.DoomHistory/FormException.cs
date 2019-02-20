//
// File imported from my old Hg.Common project
//
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Hg.DoomHistory
{
    public partial class FormException : Form
    {
        public FormException()
        {
            InitializeComponent();
        }

        private string _errorDetails = "";
        public string ErrorDetails
        {
            get => _errorDetails;
            set
            {
                _errorDetails = value;
                textBoxDetail.Text = _errorDetails;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/HgAlexx/Hg.DoomHistory/issues/new");
        }
    }
}
