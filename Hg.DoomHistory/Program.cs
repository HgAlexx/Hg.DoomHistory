using System;
using System.Windows.Forms;

namespace Hg.DoomHistory
{
    internal static class Program
    {
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
    }
}