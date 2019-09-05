using System;
using System.IO;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        /// <summary>
        /// Метод, который отвечает за подключение к серверу, отправку запросов и получение ответов.
        /// </summary>
        static void Main(string[] args)
        {
            var port = 1200;
            var client = new Client(port);
            while (true)
            {
                var input = Console.ReadKey();
                if (Equals('1', input))
                {
                    var list = client.ListReturn("ServerExample");
                    if (list != null)
                    {
                        Console.WriteLine(list.Count);
                        Console.WriteLine();
                        for (var i = 0; i < list.Count; i++)
                        {
                            Console.WriteLine(list[i]);
                            Console.WriteLine(list[i]);
                        }
                    }
                }
                else
                {
                    var list = client.GetReturn();

                }

                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
