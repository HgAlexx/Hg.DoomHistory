using System;
using System.Windows.Forms;
using Hg.DoomHistory.Forms;
using Hg.DoomHistory.Utilities;

namespace Hg.DoomHistory
{
    internal static class Program
    {
        #region Members

        [STAThread]
        private static void Main()
        {
            Logger.ExceptionMode = LogMode.Dialog;
            Logger.Level = LogLevel.Debug;

            // Exception Handlers
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.CurrentDomain_UnhandledException;
            Application.ThreadException += ExceptionHandler.Application_ThreadException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        #endregion
    }
}