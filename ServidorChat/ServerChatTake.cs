using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using ServidorChat.Essencial.Chat;
using ServidorChat.Essencial;
using Newtonsoft.Json;
using ServidorChat.Essencial.LogChat;

namespace ServidorChat
{
    class ServerChatTake
    {
        private static int port = 5000;
        PrintLog logPrint = PrintLog.getInstance();
        Server serverProcess = Server.getInstance();

        public void ConnectServer()
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine(String.Format("Servidor executando no endereco: {0}:{1}", server.LocalEndpoint.ToString(), port), Environment.NewLine);

            try
            {

                string data = String.Empty;
                while (true)
                {
                    Console.WriteLine("Aguardando conexão...");
                    TcpClient client = server.AcceptTcpClient();
                    string msgClientConnect = String.Format("Conexão do cliente: {0}",
                        client.Client.RemoteEndPoint.ToString());
                    Console.WriteLine(msgClientConnect);
                    logPrint.print(msgClientConnect, LogType.INFO);
                    data = String.Empty;

                    Byte[] bytes = new Byte[4000];
                    Thread t = new Thread(() =>
                    {
                        NetworkStream stream = client.GetStream();
                        int i;
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            string content = data.Split('\n')[data.Split('\n').Length - 1];
                            Console.WriteLine(data);
                            Command cmd = JsonConvert.DeserializeObject<Command>(content);

                            string textRet = String.Empty;
                            switch (cmd.commandType)
                            {
                                case Commands.ADMIN:
                                    string target = cmd.cmd.Trim().Split(' ')[1];
                                    Response resAdm = serverProcess.ListAdministrator(target);
                                    textRet = HttpHelper.HttpResponseMountResponse(resAdm);
                                    break;
                                case Commands.CNOTICE:
                                    int tamC = cmd.cmd.Trim().Split(' ').Length;
                                    if(tamC == 5)
                                    {
                                        string nickPubPri = cmd.cmd.Trim().Split(' ')[1];
                                        string nickDestPubPri = cmd.cmd.Trim().Split(' ')[2];
                                        string roomPubPri = cmd.cmd.Trim().Split(' ')[3];
                                        string msgPubPri = cmd.cmd.Trim().Split(' ')[4];
                                        serverProcess.SendMessageParticular(nickPubPri, nickDestPubPri, roomPubPri, msgPubPri.Replace("#", " "), MessageType.PUBLICUSER);
                                        textRet = HttpHelper.HttpResponseMountResponse(new Response()
                                        { message = " ", success = true });
                                    }
                                    else
                                    {
                                        textRet = HttpHelper.HttpResponseMountResponse(new Response()
                                        { message = "Erro! Comando faltando parametros. Digite 'HELP CNOTICE' para ajuda.", success = false });
                                    }
                                    break;
                                case Commands.JOIN:
                                    string nickJoin = cmd.cmd.Trim().Split(' ')[1];
                                    string roomJoin = String.Empty;
                                    if (cmd.cmd.Trim().Split(' ').Length == 3)
                                        roomJoin = cmd.cmd.Trim().Split(' ')[2];
                                    if (!String.IsNullOrEmpty(roomJoin))
                                    {
                                        Response retJoin = serverProcess.JoinRoom(nickJoin, roomJoin);
                                        textRet = HttpHelper.HttpResponseMountResponse(retJoin);
                                    }
                                    else
                                        textRet = HttpHelper.HttpResponseMountResponse(new Response()
                                        { message = "Erro. Por favor informe a sala.", success = false });
                                    break;
                                case Commands.KICK:
                                    break;
                                case Commands.LIST:
                                    Response respResRooms = serverProcess.ListRooms();
                                    textRet = HttpHelper.HttpResponseMountResponse(respResRooms);
                                    break;
                                case Commands.NICK:
                                    string nickUser = cmd.cmd.Trim().Split(' ')[1];
                                    Response resNick = serverProcess.Nick(nickUser);
                                    textRet = HttpHelper.HttpResponseMountResponse(resNick);
                                    break;
                                case Commands.OPER:
                                    break;
                                case Commands.PART:
                                    string nickLeave = cmd.cmd.Trim().Split(' ')[1];
                                    string roomLeave = cmd.cmd.Trim().Split(' ')[2];
                                    Response respPart = serverProcess.LeaveRoom(nickLeave, roomLeave);
                                    textRet = HttpHelper.HttpResponseMountResponse(respPart);
                                    break;
                                case Commands.PASS:
                                    break;
                                case Commands.PRIVMSG:
                                    int tamP = cmd.cmd.Trim().Split(' ').Length;
                                    if(tamP == 5)
                                    {
                                        string nickSenderPri = cmd.cmd.Trim().Split(' ')[1];
                                        string nickDest = cmd.cmd.Trim().Split(' ')[2];
                                        string roomPri = cmd.cmd.Trim().Split(' ')[3];
                                        string msgPri = cmd.cmd.Trim().Split(' ')[4];
                                        serverProcess.SendMessageParticular(nickSenderPri, nickDest, roomPri, msgPri.Replace("#", " "), MessageType.PRIVATE);
                                        textRet = HttpHelper.HttpResponseMountResponse(new Response()
                                        { message = " ", success = true });
                                    }
                                    else
                                    {
                                        textRet = HttpHelper.HttpResponseMountResponse(new Response()
                                        { message = "Erro. Comando faltando parametros. Digite 'HELP PRIVMSG' para ajuda.", success = false });
                                    }

                                    break;
                                case Commands.PUBMSG:
                                    int tam = cmd.cmd.Trim().Split(' ').Length;
                                    if (tam == 4)
                                    {
                                        string roomPub = cmd.cmd.Trim().Split(' ')[1];
                                        string msgPub = cmd.cmd.Trim().Split(' ')[2];
                                        string nickSender = cmd.cmd.Trim().Split(' ')[3];
                                        serverProcess.SendMessagePublic(nickSender, roomPub, msgPub.Replace("#", " "));
                                        textRet = HttpHelper.HttpResponseMountResponse(new Response()
                                        { message = " ", success = true });
                                    }
                                    else
                                    {
                                        textRet = HttpHelper.HttpResponseMountResponse(new Response()
                                        { message = "Erro. Comando faltando parâmetroa. Digite 'HELP PUBMSG' para ajuda.", success = false });
                                    }
                                    break;
                                case Commands.CREATEROOM:
                                    Response responseCRoom = serverProcess.createRoom(cmd.cmd.Trim().Split(' ')[1]);
                                    textRet = HttpHelper.HttpResponseMountResponse(responseCRoom);
                                    break;
                                case Commands.QUIT:
                                    string nickLieveserver = cmd.cmd.Trim().Split(' ')[1];
                                    serverProcess.LeaveServer(nickLieveserver);
                                    textRet = HttpHelper.HttpResponseMountResponse(new Response() { 
                                    message = "Usuário desconectado do servidor!", success = true });
                                    break;
                                case Commands.WAITMESSAGE:
                                    string nicWait = cmd.cmd.Trim().Split(' ')[1];
                                    serverProcess.WaitMessage(nicWait, client);
                                    break;
                            }

                            if (!String.IsNullOrEmpty(textRet))
                            {
                                Console.WriteLine(textRet);
                                byte[] response = new byte[4000];

                                Array.Copy(Encoding.UTF8.GetBytes(textRet), 0, response, 0, Encoding.UTF8.GetBytes(textRet).Length);
                                stream.Write(response, 0, response.Length);
                            }
                        }
                    });
                    t.Start();
                }
            }
            catch (SocketException se)
            {
                logPrint.print(se, LogType.ERROR);
            }
            finally
            {
                server.Stop();
                logPrint.print("O servidor parou", LogType.INFO);
            }
        }
    }
}
