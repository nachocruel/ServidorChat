using System;
using System.Collections.Generic;
using System.Text;

namespace ServidorChat.Essencial.LogChat
{
    public class PrintLog
    {
        private AbstractLog logErro = new LogErro();
        private AbstractLog logInfo = new LogInfo();
        private static PrintLog pLog = new PrintLog();
        private PrintLog() { }

        public static PrintLog getInstance()
        {
            return pLog;
        }

        public void print(object log, LogType logType)
        {
            switch (logType)
            {
                case LogType.ERROR:
                    logErro.Print(log);
                    break;
                case LogType.INFO:
                    logInfo.Print(log);
                    break;
            }
        }
    }
}
