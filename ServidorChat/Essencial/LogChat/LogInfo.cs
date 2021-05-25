using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServidorChat.Essencial.LogChat
{
    public class LogInfo:AbstractLog
    {
        private static readonly object blk = new object();
        protected override void WriteLog(object logObj)
        {
            string chatDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Chat");
            if (!Directory.Exists(chatDir))
                Directory.CreateDirectory(chatDir);
            string logDir = Path.Combine(chatDir, "LogDir");
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            string fileName = "logInfo_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
           string msg = (string)logObj;
            lock (blk)
            {
                using (var sw = new StreamWriter(Path.Combine(logDir, fileName), true))
                {
                    sw.WriteLine(msg);
                }
            }
        }
    }
}
