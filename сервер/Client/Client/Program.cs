using System;
using System.IO;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        const int port = 1200;

        /// <summary>
        /// Метод, который отвечает за подключение к серверу, отправку запросов и получение ответов.
        /// </summary>
        static void Main(string[] args)
        {
            using (var client = new TcpClient("localhost", port))
            {
                Console.WriteLine("Hellow! If you want to see files send 1 and then parth, if you wand to download files right 2 and then parth, for exit right Exit.");
                NetworkStream stream = client.GetStream();
                var reader = new StreamReader(stream, System.Text.Encoding.Unicode);
                var writer = new StreamWriter(stream, System.Text.Encoding.Unicode);
                var message = Console.ReadLine();
                while (message != "Exit")
                {
                    writer.WriteLine(message);
                    writer.Flush();
                    string data = reader.ReadLine();
                    Console.WriteLine(data);
                    message = Console.ReadLine();
                }
            }
        }
    }
}
