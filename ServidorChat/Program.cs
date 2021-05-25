
using System;

namespace ServidorChat
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerChatTake serverChat = new ServerChatTake();
            serverChat.ConnectServer();
        }
    }
}
