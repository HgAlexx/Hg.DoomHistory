//
// File imported from my old Hg.Common project
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Hg.DoomHistory.Forms;
using Hg.DoomHistory.Types;

namespace Hg.DoomHistory.Utilities
{
    public delegate void LogEvent();

    public class Logger
    {
        #region

        public static LogMode ExceptionMode = LogMode.Debug;
        public static LogLevel Level = LogLevel.Error;
        private static readonly List<string> LogEntries = new List<string>();

        private static readonly object LogEntriesLock = new object();
        private static FormException _formException;
            
        public static bool Enabled { get; set; }

        #endregion

        #region

        public static void ClearLogs()
        {
            lock (LogEntriesLock)
            {
                LogEntries.Clear();
            }

            OnLog?.Invoke();
        }

        public static List<string> GetLogs()
        {
            lock (LogEntriesLock)
            {
                return LogEntries.ToList();
            }
        }

        public static void Log(string message, LogLevel level)
        {
            if (!Enabled)
            {
                return;
            }

            string fullMessage = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + level + ": " + message;
            Debug.WriteLine(fullMessage);

            if (level <= Level)
            {
                lock (LogEntriesLock)
                {
                    LogEntries.Add(fullMessage);
                }

                OnLog?.Invoke();
            }
        }

        public static void LogException(Exception exception)
        {
            switch (ExceptionMode)
            {
                case LogMode.Debug:
                    LogExceptionDebug(exception);
                    break;
                case LogMode.Dialog:
                    LogExceptionDebug(exception);
                    LogExceptionDialog(exception);
                    break;
            }
        }

        public static void LogExceptionDebug(Exception exception)
        {
            Debug.WriteLine("");
            Debug.WriteLine(exception.ToString(), "Exception");
            Debug.WriteLine("");
            if (exception.InnerException != null)
            {
                LogExceptionDebug(exception.InnerException);
            }
        }

        public static void LogExceptionDialog(Exception exception)
        {
            string content = "";
            content = exception.ToString();

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                content += Environment.NewLine;
                content += exception.ToString();
            }

            if (_formException == null)
            {
                _formException = new FormException();
            }

            _formException.ErrorDetails.Add(new Error(){ Title = exception.Message, Content = content});
            _formException.LoadCombobox();

            if (!_formException.Visible)
                _formException.Show();
        }

        public static event LogEvent OnLog;

        #endregion
    }

    public enum LogLevel
    {
        None,
        Error,
        Warning,
        Information,
        Debug
    }

    public enum LogMode
    {
        None,
        Debug,
        File,
        Dialog
    }
}
