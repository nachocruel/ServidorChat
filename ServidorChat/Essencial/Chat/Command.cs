using System;
using System.Collections.Generic;
using System.Text;

namespace ServidorChat.Essencial.Chat
{
    public class Command
    {
        public string cmd { get; set; }
        public Commands commandType { get; set; }
    }
}
