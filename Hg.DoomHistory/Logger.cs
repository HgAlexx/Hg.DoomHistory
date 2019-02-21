//
// File imported from my old Hg.Common project
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Hg.DoomHistory
{
    public delegate void LogEvent();

    public class Logger
    {
        public static LogMode ExceptionMode = LogMode.Debug;
        public static LogLevel Level = LogLevel.Error;

        private static readonly object LogEntriesLock = new object();
        private static readonly List<string> LogEntries = new List<string>();

        public static bool Enabled { get; set; }

        public static event LogEvent OnLog;

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
                return;

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

        public static void LogExceptionDialog(Exception exception)
        {
            FormException formException = new FormException {ErrorDetails = exception.ToString()};
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                formException.ErrorDetails += Environment.NewLine;
                formException.ErrorDetails += exception.ToString();
            }

            if (formException.ShowDialog() == DialogResult.Cancel)
            {
                Application.Exit(new CancelEventArgs());
            }
        }

        public static void LogExceptionDebug(Exception exception)
        {
            Debug.WriteLine("");
            Debug.WriteLine(exception.ToString(), "Exception");
            Debug.WriteLine("");
            if (exception.InnerException != null)
                LogExceptionDebug(exception.InnerException);
        }

        public static void ClearLogs()
        {
            lock (LogEntriesLock)
            {
                LogEntries.Clear();
            }

            OnLog?.Invoke();
        }
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