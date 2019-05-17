using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

//сделать так чтобы хоть что-то работало типо запустить сервер и клиент и что-то происходит  (исполнился какой-то запрос)
namespace Сервер2
{
    public class Server
    {
        private int port;
        private TcpListener listener;

        public int NumOfConnectedClients { get; set; } = 0;

        public Server(int port)
        {
            this.port = port;

            listener = new TcpListener(IPAddress.Any, port);
        }

        public void ServerWork(int port) 
        {
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            //сделать цикл бесконечный пока не stopped
            //отдавать здесь работу в отдельный поток модно с таск
            ++NumOfConnectedClients;
            Console.WriteLine("New connection...");

            try
            {
                if (client.Connected)
                {
                    Task task = new Task(() => ClientConnected(client));
                    task.Start();
                }
            }

            catch (AggregateException)
            {
                Console.WriteLine("Exception in task.");
            }
        }

        public void ClientConnected(TcpClient client)
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
            if (!dir.Exists)
            {
                return "-1";
            }
            string request = "";
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
            var ourFile = new FileInfo(responce.Remove(0, 2));
            if (ourFile.Exists)
            {
                using (var str = new StreamReader(responce.Remove(0, 2)))
                {
                    return ourFile.Length + str.ReadToEnd();
                }
            }
            else
            {
                return "-1";
            }
        }
    }
}

