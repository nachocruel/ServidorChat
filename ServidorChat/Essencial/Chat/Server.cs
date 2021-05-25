using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServidorChat.Essencial.Chat
{
    public class Server
    {
        private static Server instance = new Server();
        private string serverName { get; set; }

        private List<User> usersOut = new List<User>();
        private List<User> allUsers = new List<User>();

        private Server()
        {
            serverName = "ServerAPP";
            // COMO NESTE PROJETO NÃO UTILIZA BD O USUÁRIO ADMIN INICIAL É CONFIGURADO DINÂMICO
            User firstUser = new User()
            {
                nickName = "EDVAN",
                status = UserStatus.OFFLINE,
                profile = UserProfile.ADMIN
            };

            User secondUser = new User()
            {
                nickName = "JOAO",
                status = UserStatus.OFFLINE,
                profile = UserProfile.ADMIN
            };
            administrators.Add(secondUser);
            allUsers.Add(secondUser);
            allUsers.Add(firstUser);
            administrators.Add(firstUser);

            Room defaultRoom = new Room();
            defaultRoom.roomName = "default";
            rooms.Add(defaultRoom);
        }

        public List<User> administrators = new List<User>();
        public List<Room> rooms = new List<Room>();


        public static Server getInstance()
        {
            return instance;
        }


        /// <summary>
        /// Lista os adiministradores do servidor
        /// </summary>
        /// <returns></returns>
        public Response ListAdministrator(string target)
        {
            if (target.ToLower() == serverName.ToLower())
            {
                return new Response() { message = JsonConvert.SerializeObject(administrators), success = true };
            }
            else
            {
                User user = administrators.Find(admin => admin.nickName.ToLower() == target);
                if (user != null)
                    return new Response() { message = JsonConvert.SerializeObject(user), success = true };
            }
            return new Response() { message = "User not found!", success = true };
        }

        /// <summary>
        /// Adiciona um sala ao servidor
        /// </summary>
        /// <param name="room"></param>
        public void AddRoom(Room room)
        {
            rooms.Add(room);
        }

        /// <summary>
        /// Remove uma Sala do servidor
        /// </summary>
        /// <param name="room"></param>
        public void RemoveRoom(Room room)
        {
            rooms = rooms.FindAll(r => r.roomName != room.roomName);
        }

        /// <summary>
        /// Lista todas as salas do servidor
        /// </summary>
        /// <returns></returns>
        public Response ListRooms()
        {
            return new Response() { message = JsonConvert.SerializeObject(rooms), success = true };
        }

        /// <summary>
        /// Cria um nova sala
        /// </summary>
        /// <param name="roomName"></param>
        public Response createRoom(string roomName)
        {
            if (rooms.Find(r => r.roomName.ToUpper() == roomName.ToUpper()) == null)
            {
                rooms.Add(new Room() { roomName = roomName });
                return new Response() { message = "A sala '" + roomName + "' foi criada com sucesso!", success = true };
            }
            return new Response() { message = "Erro: Já existe um sala com o nome "+ roomName + ". Selecione outro nome!", success = false };
        }

        public void WaitMessage(string nickName, TcpClient client)
        {
            User user = allUsers.Find(u => u.nickName == nickName);
            if(user != null)
            {
                user.SetUserConnection(client);
            }
        }

        #region COMANDOS DO USUARIO
        public void SendMessageParticular(string nickSander, string nickDest, string roomName, string text, MessageType msgType = MessageType.PRIVATE)
        {
            var room = rooms.Find(r => r.roomName == roomName);
            if (room != null)
            {
                var userSender = room.GetUser(nickSander);
                var userDest = room.GetUser(nickDest);
                if (userSender != null && userDest != null)
                {
                    Message message = new Message(userSender, userDest);
                    message.text = text;
                    message.viewed = false;
                    message.sendDate = DateTime.Now;
                    userSender.sendMessage(message, msgType);
                }
            }
        }

        public void SendMessagePublic(string nickSander, string roomName, string text)
        {
            var room = rooms.Find(r => r.roomName == roomName);
            if (room != null)
            {
                var userSender = room.GetUser(nickSander);
                if (userSender != null)
                {
                    Message message = new Message(userSender, null);
                    message.viewed = false;
                    message.text = text;
                    message.sendDate = DateTime.Now;
                    userSender.sendMessage(message, MessageType.PUBLIC);
                }
            }
        }

        public Response JoinRoom(string nickName, string roomName)
        {
            var room = rooms.Find(r => r.roomName == roomName);
            if (room != null)
            {
                var user = usersOut.Find(u => u.nickName == nickName);
                if (user != null)
                {
                    user.JoinRoom(room);
                    usersOut.Remove(user);
                    SendMessagePublic(user.nickName, room.roomName, user.nickName + " entrou na sala!");
                    return new Response() { message = JsonConvert.SerializeObject(room), success = true };
                }
                return new Response() { message = "Apalido não encontrado!", success = false };
            }
            return new Response() { message = "Sala não encontrada!", success = false };
        }

        public Response LeaveRoom(string nickName, string roomName)
        {
            var room = rooms.Find(r => r.roomName == roomName);
            if (room != null)
            {
                var user = room.GetUser(nickName);
                if (user != null)
                {
                    SendMessagePublic(user.nickName, room.roomName, user.nickName + " saiu da sala!");
                    usersOut.Add(user);
                    room.GetAllUsers().Remove(user);
                    user.Part();
                    return new Response() { message = "Concluído", success = true };
                }
                return new Response { message = "Usuário não econtrado" };
            }
            return new Response() { message = "A sala não existe.", success = false };
        }

        public Response Nick(string nickName)
        {
            if (nickAvailable(nickName))
            {
                User user = new User();
                user.nickName = nickName;
                user.status = UserStatus.ONLINE;
                user.profile = UserProfile.NORMAL;

                usersOut.Add(user);
                allUsers.Add(user);
                return new Response() { message = JsonConvert.SerializeObject(user), success = true };
            }
            return new Response() { message = "Apelido indisponível!", success = false };
        }

        public void LeaveServer(string nickName)
        {
            User user = allUsers.Find(u => u.nickName == nickName);
            if (user != null)
            {
                if (usersOut.Contains(user))
                    usersOut.Remove(user);
                foreach(var room in rooms)
                {
                    if(room.GetAllUsers().Find(u=>u.nickName==nickName) != null)
                    {
                        room.GetAllUsers().Remove(user);
                    }
                }
                allUsers.Remove(user);
            }
        }

        private bool nickAvailable(string nickName)
        {
            bool available = true;
            foreach (var user in allUsers)
                if (user.nickName == nickName)
                    available = false;
            return available;
        }
        #endregion
    }
}
