//
// File imported from my old Hg.Common project
//
using System;

namespace Hg.DoomHistory
{
    public class ExceptionHandler
    {
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Logger.LogException(ex);
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Logger.LogException(ex);
        }
    }
}
