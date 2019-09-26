using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Сервер2
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 1200;
            var server = new Server(port);
            server.ServerWork(port);
        }
    }
}

//надо сделать юнит тесты, создать файл и в тестах запускать сервер и смотреть, что он возвращает
//многопоточность