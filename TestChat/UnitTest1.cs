using NUnit.Framework;

namespace TestChat
{
    using Newtonsoft.Json;
    using ServidorChat;
    using ServidorChat.Essencial.Chat;

    public class Tests
    {
        Server serverProcess;

        [SetUp]
        public void Setup()
        {
            serverProcess = Server.getInstance();
            Room newRoom = new Room();
            newRoom.roomName = "sala-teste";
            serverProcess.AddRoom(newRoom);
        }

        [Test]
        public void TestAdmin()
        {   
            // Testa o retorno de informação do ADMIN
            var ret = serverProcess.ListAdministrator("EDVAN");
            Assert.IsTrue(((Response)ret).success);
        }

        [Test]
        public void TestAdminNull()
        {
            // Teste Admin não existente
            var ret = serverProcess.ListAdministrator("Mario");
            Assert.IsFalse(!ret.success);
        }

       [Test]
        public void RetornoRooms()
        {
            var ret = serverProcess.ListRooms();
            Assert.IsTrue(ret.success);
        }

        [Test]
        public void NickAndJoinUserAndPartRoom()
        {
            // Usuário ainda não existe
            var resp = serverProcess.Nick("Cassio");
            Assert.IsTrue(resp.success);
            var resp2 = serverProcess.Nick("Cassio");

            // Usuário já está no server, ret deve ser falso
            Assert.IsFalse(resp2.success);

            var resp3 = serverProcess.Nick("Roger");
            serverProcess.JoinRoom("Roger", "sala-teste");

            User user = JsonConvert.DeserializeObject<User>(resp.message);
            Response resp4 = serverProcess.JoinRoom(user.nickName, "sala-teste");

            Assert.IsTrue(resp4.success);

            // Sair da sala
            Response res5 = serverProcess.LeaveRoom(user.nickName, "sala-teste");
            Assert.IsTrue(res5.success);
        }
    }
}