using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Hg.DoomHistory
{
    public partial class SlotControl : UserControl
    {
        public SlotControl()
        {
            InitializeComponent();
        }

        private void pictureBoxScreenshot_DoubleClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(pictureBoxScreenshot.ImageLocation))
            {
                try
                {
                    Process.Start(pictureBoxScreenshot.ImageLocation);
                }
                catch (Exception ex)
                {
                    Logger.Log("pictureBoxScreenshot_DoubleClick: Exception: " + ex.Message, LogLevel.Debug);
                    Logger.LogException(ex);
                }
            }
        }
    }
}