using Newtonsoft.Json;
using ServidorChat.Essencial.Chat;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServidorChat.Essencial
{
    public class HttpHelper
    {
        /// <summary>
        /// Monta o Cabeçario HTTP para um mensagem
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string HttpResponseMountMessage(Message message)
        {
            string strMessage = JsonConvert.SerializeObject(message, Formatting.None);
            return "HTTP/1.1 200 OK" + Environment.NewLine
                                + "Access-Control-Allow-Origin: *" + Environment.NewLine
                                + "Content-Type: application/json; charset=utf-8" + Environment.NewLine
                                + "Date: " + DateTime.Now.ToString("F", System.Globalization.CultureInfo.GetCultureInfo("en"))
                                + Environment.NewLine
                                + "Sec-WebSocket-Key: 75bc4508-d85f-404e-b1c3-197aac0c1764" + Environment.NewLine
                                + "Content-Encoding: gzip, deflate, br" + Environment.NewLine
                                + "Content-Length: " + strMessage.Length + Environment.NewLine
                                + "Connection: Keep-Alive" + Environment.NewLine + Environment.NewLine
                                + strMessage;
        }

        /// <summary>
        /// Monta o cabeçario HTTP para um resposta
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string HttpResponseMountResponse(Response response)
        {
            string strResponse = JsonConvert.SerializeObject(response, Formatting.None);
            return "HTTP/1.1 200 OK" + Environment.NewLine
                                + "Access-Control-Allow-Origin: *" + Environment.NewLine
                                + "Content-Type: application/json; charset=utf-8" + Environment.NewLine
                                + "Date: " + DateTime.Now.ToString("F", System.Globalization.CultureInfo.GetCultureInfo("en"))
                                + Environment.NewLine
                                + "Sec-WebSocket-Key: 75bc4508-d85f-404e-b1c3-197aac0c1764" + Environment.NewLine
                                + "Content-Encoding: gzip, deflate, br" + Environment.NewLine
                                + "Content-Length: " + strResponse.Length + Environment.NewLine
                                + "Connection: Keep-Alive" + Environment.NewLine + Environment.NewLine
                                + strResponse;
        }

        public static string HttpResponseMountRoom(Room room)
        {
            string strRoom = JsonConvert.SerializeObject(room, Formatting.None);
            return "HTTP/1.1 200" + Environment.NewLine
                                + "Access-Control-Allow-Origin: *" + Environment.NewLine
                                + "Content-Type: application/json; charset=utf-8" + Environment.NewLine
                                + "Date: " + DateTime.Now.ToString("F", System.Globalization.CultureInfo.GetCultureInfo("en"))
                                + Environment.NewLine
                                + "Sec-WebSocket-Key: 75bc4508-d85f-404e-b1c3-197aac0c1764" + Environment.NewLine
                                + "Content-Encoding: gzip, deflate, br" + Environment.NewLine
                                + "Content-Length: " + strRoom.Length + Environment.NewLine
                                + "Connection: Keep-Alive" + Environment.NewLine + Environment.NewLine
                                + strRoom;
        }
    }
}
