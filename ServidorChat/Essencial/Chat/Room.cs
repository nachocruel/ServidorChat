using ServidorChat.Essencial.LogChat;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServidorChat.Essencial.Chat
{
    public class Room
    {
        public string roomName { get; set; }
        public List<User> users = new List<User>();
        public List<Message> roomMessages = new List<Message>();
        private PrintLog logPrint = PrintLog.getInstance();

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public User GetUser(string nickName)
        {
            return users.Find(u => u.nickName == nickName);
        }

        public List<User> GetAllUsers()
        {
            return users;
        }

        /// <summary>
        /// Notificação publica
        /// </summary>
        /// <param name="message"></param>
        public void notifyPublic(Message message)
        {
            try
            {
                string httpMsg = HttpHelper.HttpResponseMountMessage(message);
                byte[] response = new byte[4000];
                Array.Copy(Encoding.UTF8.GetBytes(httpMsg), 0, response, 0, Encoding.UTF8.GetBytes(httpMsg).Length);
                Console.WriteLine(httpMsg);
                foreach (var user in users)
                {
                    if (user.GetUserConnection() != null)
                    {
                        NetworkStream stream = user.GetUserConnection().GetStream();
                        stream.Write(response, 0, response.Length);
                    }
                }
                roomMessages.Add(message);
            }
            catch (Exception ex)
            {
                logPrint.print(ex, LogType.ERROR);
            }
        }

        /// <summary>
        /// Notifica usuário específico publicamente
        /// </summary>
        /// <param name="message"></param>
        public void notifyUserPublic(Message message)
        {
            try
            {
                string httpMsg = HttpHelper.HttpResponseMountMessage(message);
                byte[] response = new byte[4000];
                Array.Copy(Encoding.UTF8.GetBytes(httpMsg), 0, response, 0, Encoding.UTF8.GetBytes(httpMsg).Length);
                Console.WriteLine(httpMsg);
                foreach (var user in users)
                {
                    if (user.GetUserConnection() != null)
                    {
                        NetworkStream stream = user.GetUserConnection().GetStream();
                        stream.Write(response, 0, response.Length);
                    }
                }
                roomMessages.Add(message);
            }
            catch (Exception ex)
            {
                logPrint.print(ex, LogType.ERROR);
            }
        }

        /// <summary>
        /// Notifica usuário privadamente
        /// </summary>
        /// <param name="message"></param>
        public void notifyPrivate(Message message)
        {
            

            try
            {
                User uAuthor = users.Find(u => u.nickName == message.userAuthor.nickName);
                User uDest = users.Find(u => u.nickName == message.userDest.nickName);
                string httpMsg = HttpHelper.HttpResponseMountMessage(message);
                byte[] response = new byte[4000];
                Array.Copy(Encoding.UTF8.GetBytes(httpMsg), 0, response, 0, Encoding.UTF8.GetBytes(httpMsg).Length);
                Console.WriteLine(httpMsg);
                foreach (var user in users)
                {
                    if (user.nickName == uAuthor.nickName || user.nickName == uDest.nickName)
                    {
                        if (user.GetUserConnection() != null)
                        {
                            NetworkStream stream = user.GetUserConnection().GetStream();
                            stream.Write(response, 0, response.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logPrint.print(ex, LogType.ERROR);
            }
        }
    }
}
