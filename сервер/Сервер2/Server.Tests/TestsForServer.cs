using System;
using Сервер2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

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
            string testArray = Sevrer.ListResponse(path);
            Assert.AreEqual(testArray, "3 " +
                ".appveyor.yml <False>, 1.jpg <False>, a.txt <False>, ");
        }

        [TestMethod]
        public void CheckMethodProsessingRequest()
        {
            string path = "";
            string testArray = Сервер2.Sevrer.ProsessingRequest(path);
            Assert.AreEqual(testArray, "Request is empty");
        }

        public void CreateClient(Task obj)
        {
            var client = new TcpClient("localhost", 1200);
        }

        [TestMethod]
        public void IsClientConnected()
        {
            Sevrer myServer = new Sevrer();
            Task.Delay(3000).ContinueWith(CreateClient);
            myServer.ServerWork(1200);
            Assert.AreNotEqual(0, myServer.NumOfConnectedClients);
        }
    }
}
