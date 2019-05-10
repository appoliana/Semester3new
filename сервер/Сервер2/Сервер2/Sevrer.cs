using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Сервер2
{
    public class Sevrer
    {
        private const int port = 1200;

        public int NumOfConnectedClients { get; set; } = 0;

        public void ServerWork(int port) 
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            ++NumOfConnectedClients;
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
         //   IsClientConnected = false;
        }

        public void clientConnected(TcpClient client)
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
            FileInfo ourfile = new System.IO.FileInfo(responce.Remove(0, 2));
            if (ourfile.Exists)
            {
                using (var str = new StreamReader(responce.Remove(0, 2)))
                {
                    return ourfile.Length + str.ReadToEnd();
                }
            }
            else
            {
                return "-1";
            }
        }
    }
}

