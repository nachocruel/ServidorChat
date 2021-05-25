using System;
using System.Collections.Generic;
using System.Text;

namespace ServidorChat.Essencial.LogChat
{
    public abstract class AbstractLog
    {
        public void Print(object obj)
        {
            WriteLog(obj);
        }

        protected abstract void WriteLog(object logObj);
    }
}
