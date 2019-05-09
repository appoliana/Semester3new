using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        const int port = 1200;
        public static void ClientWork(int port)
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
                    ConsolePrint(data);
                    message = Console.ReadLine();
                }
            }
        }

        public static void ConsolePrint(string data)
        {
            Console.WriteLine(data);
        }
    }
}
