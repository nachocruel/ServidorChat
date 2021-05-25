using System;
using System.Collections.Generic;
using System.Text;

namespace ServidorChat.Essencial
{
    public enum LogType {ERROR, WARNING, INFO}

    public enum UserStatus { ONLINE, OFFLINE, BUSY}
    public enum MessageType { PUBLIC, PUBLICUSER, PRIVATE }
    public enum UserProfile { ADMIN, NORMAL, OPERADOR }
}
