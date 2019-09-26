using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// Класс, который отвечает за клиент.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Конструктор класса Client.
        /// </summary>
        public Client(int port)
        {
            this.port = port;
            this.client = new TcpClient("localhost", port);
        }

        private int port;
        private TcpClient client;

        public TcpClient GetClient() => client;

//        /// <summary>
//        /// Метод, который отвечает за связь с сервером.
//        /// </summary>
//        /*public List<string> ClientWork(string path)
//        {
//            using (client)
//            {
//                Console.WriteLine("Hello! If you want to see files send 1 and then path, if you want to download files right 2 and then parth, for exit right Exit.");
//                /*NetworkStream stream = client.GetStream();
//                var reader = new StreamReader(stream, System.Text.Encoding.Unicode);
//                var writer = new StreamWriter(stream, System.Text.Encoding.Unicode);
//                */
//                var message = path;
//                while (message != "Exit")
//                {
//                   /* writer.WriteLine(message);
//                    writer.Flush();
//                    string data = reader.ReadLine();
//                    */
//                    if (message.ElementAt(0) == 1)
//                    {
//                        return ListReturn(message);
//                    }
//                    if (message.ElementAt(0) == 2)
//                    {
//                        return GetReturn(message);
//                    }
//                }
//                return null;
//            }
//        }
//*/
        
        /// <summary>
        /// Метод, который возвращает ответ формата запроса List.
        /// </summary>
        public List<string> ListReturn(string path)
        {
            NetworkStream stream = client.GetStream();
            var reader = new StreamReader(stream, System.Text.Encoding.Unicode);
            var writer = new StreamWriter(stream, System.Text.Encoding.Unicode);
            writer.WriteLine("1 " + path);
            writer.Flush();
            string data = reader.ReadLine();
            List<string> anser = new List<string>();
            anser.Add(data);
            return anser;
        }

        /// <summary>
        /// Метод, который возвращает ответ формата запроса Get.
        /// </summary>
        public List<string> GetReturn(string path)
        {
            NetworkStream stream = client.GetStream();
            var reader = new StreamReader(stream, System.Text.Encoding.Unicode);
            var writer = new StreamWriter(stream, System.Text.Encoding.Unicode);
            writer.WriteLine("2 " + path);
            writer.Flush();
            string data = reader.ReadLine();
            List<string> anser = new List<string>();
            anser.Add(data);
            return anser;
        }

        /// <summary>
        /// Метод, который печатает ответ сервера.
        /// </summary>
        /// <param name="data"></param>
        public static void ConsolePrint(string data)
        {
            Console.WriteLine(data);
        }
    }
}
