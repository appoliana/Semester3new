using System;
using Сервер2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server.Tests
{
    [TestClass]
    public class TestsForServer
    {
        [TestMethod]
        public void CheckMethodListResponse()
        {
            string path = "D:\\gitNew\\Semester3new\\сервер\\Сервер2\\Server.Tests\\ForTest";
            string testArray = Sevrer.ListResponse(path);
            Assert.AreEqual(testArray, "3 ============List of files and folders============= " +
                ".appveyor.yml <False>, 1.jpg <False>, a.txt <False>, ");
        }

        [TestMethod]
        public void CheckMethodGetResponse()
        {
            string path = "D:\\gitNew\\Semester3new\\сервер\\Сервер2\\Server.Tests\\ForTest\\a.txt";
            string testArray = Сервер2.Sevrer.GetResponse(path);
            Assert.AreEqual(testArray, "The contanse of our file: work");
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
