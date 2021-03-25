using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace TcpListenForms {
    public static class Logger {
        static object locker = new object();
        static string _environmentVars = null;
        public static string CompIp;
        public static string CompNameWin;
        public static string CompNameDns;
        public static string UserNameWin;

        public static void WriteLog(string str) {
            string fullDir = GetTempDir() + @"Logs\";
            if (!Directory.Exists(fullDir)) {
                try {
                    Directory.CreateDirectory(fullDir);
                }
                finally { }
            }
            string s = DateTime.Now.ToLongTimeString() + "." + String.Format("{0,3:000}", DateTime.Now.Millisecond) +
                       ", ThrID=" + Thread.CurrentThread.GetHashCode() + " " + str + System.Environment.NewLine;
            lock (locker) {
                try {
                    File.AppendAllText(GetLogFileName(), s, Encoding.GetEncoding("windows-1251"));
                }
                finally { }
            }
        }

        public static void WriteError(Exception ex, string msgPrefix = "", bool writeToDb = true) {
            string msg;
            if (string.IsNullOrEmpty(msgPrefix)) msg = Environment.NewLine + GetExcetionAsString(ex);
            else msg = Environment.NewLine + msgPrefix + Environment.NewLine + GetExcetionAsString(ex);
            WriteLog(msg);
        }

        private static string GetExcetionAsString(Exception ex) {
            if (ex != null) {
                //if (ex.Message != null) s1 = ex.Message;
                string s1 = ex.Message ?? String.Empty;
                string s2 = ex.Source ?? String.Empty;
                string s3 = ex.StackTrace ?? String.Empty;
                return "-----------------1)Exception.Message:---------------------- \r\n" + s1 + Environment.NewLine +
                       "-----------------2)Exception.Source:---------------------- \r\n" + s2 + Environment.NewLine +
                       "-----------------3)Exception.StackTrace:---------------------- \r\n" + s3;
            }
            else
                return String.Empty;
        }

        public static string GetLogFileName(string baseFileName = null) {
            string dt = Regex.Replace(DateTime.Now.ToShortDateString(), @"[^\w\.-]", "-");
            string fullDir = GetTempDir() + @"Logs\";
            if (baseFileName == null)
                return fullDir + "AppForTesting-" + dt + ".log";
            else
                return fullDir + baseFileName + dt + ".log";
        }

        public static string GetDateTimeForName() {
            return Regex.Replace(DateTime.Now.ToShortDateString(), @"[^\w\.-]", "-") + "_" +
                   Regex.Replace(DateTime.Now.ToShortTimeString(), @"[^\w\.-]", "-");
        }

        public static string GetTimeForName() {
            return Regex.Replace(DateTime.Now.ToShortTimeString(), @"[^\w\.-]", "-");
        }


        public static string GetTempDir() {
            return GetSystemDrive() + @"\Temp\";
        }

        public static string GetSystemDrive() {
            return Environment.GetEnvironmentVariable("systemdrive");
        }


    }
}
