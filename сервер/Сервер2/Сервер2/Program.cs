using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Класс, который отвечает за сервер.
    /// </summary>
    class Program
    {
        const int port = 1200;

        /// <summary>
        /// Метод, который прослушивает подключения и принимает их.
        /// </summary>
        static void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            //Thread thread = new Thread(delegate () { EnqueueThread(myq); });
            Console.WriteLine("New connection...");
            
            try
            {
                while (client.Connected) 
                {
                    Task task = new Task(() => clientConnected(client));
                    task.Start();
                }
            }
            catch (AggregateException)
            {
                Console.WriteLine("Exception in task.");
            }
        }

        public static void clientConnected(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            var reader = new StreamReader(stream, System.Text.Encoding.Unicode);
            string response = reader.ReadLine();
            Console.WriteLine(response);
            var writer = new StreamWriter(stream, System.Text.Encoding.Unicode);
            string request = ProsessingRequest(response);
            writer.WriteLine(request);
            writer.Flush();
        }

        /// <summary>
        /// Метод, который обрабатывает запрос.
        /// </summary>
        public static string ProsessingRequest(string response)
        {
            if (response.Length == 0)
            {
                return "Request is empty";
            }

            string[] requestArray = response.Split();

            if (requestArray[0] == "1")
            {
                return ListResponse(response);
            }
            if (requestArray[0] == "2")
            {
                return GetResponse(response);
            }
            else
            {
                return "String is not in a right format.";
            }
        }

        /// <summary>
        /// Метод, который возвращает список файлов и папок по указанному пути.
        /// </summary>
        public static string ListResponse(string response)
        {
            Console.WriteLine(response);
            int count = 0;
            var dir = new DirectoryInfo(response.Remove(0, 2));
            string request = "============List of files and folders============= ";
            foreach (var item in dir.GetDirectories())
            {
                request += item.Name + " <True>, ";
                ++count;
            }
            foreach (var item in dir.GetFiles())
            {
                request += item.Name + " <False>, ";
                ++count;
            }
            var newRequest = count + " " + request;
            return newRequest;
        } // если папка то True, если файл то false

        /// <summary>
        /// Метод, который возвращает содержимое файла по указанному пути.
        /// </summary>
        public static string GetResponse(string responce)
        {
            using (var str = new StreamReader(responce.Remove(0, 2)))
            {
                string request = "The contanse of our file: " + str.ReadToEnd() ;
                return request;
            }
        }
    }
}

//надо сделать юнит тесты, создать файл и в тестах запускать сервер и смотреть, что он возвращает
//многопоточность