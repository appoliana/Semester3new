using System;
using Сервер2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Threading;

namespace Server.Tests
{
    [TestClass]
    public class TestsForServer
    {
        [TestMethod]
        public void CheckMethodListResponse()
        {
            int length = Directory.GetCurrentDirectory().Length;
            string path = Directory.GetCurrentDirectory().Remove(length - 10) + "\\ForTest";
            string testArray = Сервер2.Server.ListResponse(path);
            Assert.AreEqual(testArray, "3 " +
                ".appveyor.yml <False>, 1.jpg <False>, a.txt <False>, ");
        }

        [TestMethod]
        public void ReturnMinusOneIfFilesDidNotExist()
        {
            string path = "\\ForTest";
            Task task = new Task(CreateServer);
            task.Start();
            Thread.Sleep(20000); 
            var client = new Client.Client(1200);
            var testAnser = client.GetReturn("2 " + path);
            Assert.AreEqual("-1", testAnser);
        }

        [TestMethod]
        public void CheckMethodProsessingRequest()
        {
            string path = "";
            string testArray = Сервер2.Server.ProsessingRequest(path);
            Assert.AreEqual(testArray, "Request is empty");
        }

        public void CreateServer()
        {
            var server = new Сервер2.Server();
        }

        public TcpClient CreateClient(Task obj)
        {
            var client = new TcpClient("localhost", 1200);
            return client;
        }

        [TestMethod]
        public void IsClientConnected()
        {
            Сервер2.Server myServer = new Сервер2.Server();
            Task.Delay(3000).ContinueWith(CreateClient);
            myServer.ServerWork(1200);
            Assert.AreNotEqual(0, myServer.NumOfConnectedClients);
        }
    }
}
