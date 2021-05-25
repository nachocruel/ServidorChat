using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace ServidorChat.Essencial.Chat
{
    public class User
    {
        public string nickName { get; set; }
        public UserStatus status { get; set; }
        public UserProfile profile;
        private string password { get; set; }
               
        private TcpClient userConnection { get; set; }

        private Room atualRoom = null;

        public TcpClient GetUserConnection()
        {
            return userConnection;
        }

        public void SetUserConnection(TcpClient client)
        {
            userConnection = client;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }

        public string getPassword()
        {
            return this.password;
        }

        public void JoinRoom(Room room)
        {
            atualRoom = room;
            atualRoom.AddUser(this);
        }

        public void Part()
        {
            atualRoom = null;
        }

        public void sendMessage(Message message, MessageType messageType)
        {
            switch(messageType)
            {
                case MessageType.PUBLIC:
                    atualRoom.notifyPublic(message);
                    break;
                case MessageType.PUBLICUSER:
                    atualRoom.notifyUserPublic(message);
                    break;
                case MessageType.PRIVATE:
                    atualRoom.notifyPrivate(message);
                    break;
            }
        }
    }
}
