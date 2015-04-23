using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class TcpServerTests
    {
        private TcpServer _server;
        private IPEndPoint _serverAddress;

        public TcpServerTests()
        {
            _serverAddress = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 9123);
           // _serverAddress = new IPEndPoint(IPAddress.Any, 9123);

            _server = new TcpServer(_serverAddress, new MockProcessor());
            _server.StartListening();
        }

        [TestMethod]
        public void DataIsEnqueued()//change
        {
            byte[] inputData = new byte[]{1,2,3,4,5};
            byte[] outputData = ClientCreateSendAndReceive(inputData);

            Assert.AreEqual(outputData, new byte[] { 5, 4, 3, 2, 1 });
        }




        private byte[] ClientCreateSendAndReceive(byte[] dataToSend)
        {
            int bufferSize = 512;
            byte[] buffer = new byte[bufferSize];

            Socket socket = new Socket(_serverAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(_serverAddress);

            socket.Send(dataToSend);
            socket.Shutdown(SocketShutdown.Send);

            using (MemoryStream memory = new MemoryStream(bufferSize))
            {
                int bytesRec;
                while ((bytesRec = socket.Receive(buffer)) > 0)
                {
                    memory.Write(buffer, 0, bytesRec);
                }

                socket.Shutdown(SocketShutdown.Receive);
                socket.Close();
                buffer = memory.ToArray();
            }
            return buffer;
        }

        public class MockProcessor : IDataProcessor
        {
            public void EnqueueDataToProcess(byte[] data, Metadata metadata, ProcessedDataCallback callback)
            {
                Array.Reverse(data);

                new Task(() =>
                {
                    Thread.Sleep(1000);
                    callback(data);
                }).Start();
            }

            public void StartProcessing() { }
            public void StopProcessing() { }
            public bool IsProcessing { get; set; }
        }
    }
}

