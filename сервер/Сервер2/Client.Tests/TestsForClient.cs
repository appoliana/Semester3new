using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Сервер2;

namespace Server.Tests
{
    [TestClass]
    class TestsForClient
    {
        public void CreateClient(Task obj)
        {
            var client = new Client.Client(1200);
        }

        [TestMethod]
        public void IsServerAsked()
        {
            Sevrer myServer = new Sevrer();
            Task.Delay(3000).ContinueWith(CreateClient);
            myServer.ServerWork(1200);
            Assert.AreNotEqual(0, myServer.NumOfConnectedClients);
        }
    }
}
