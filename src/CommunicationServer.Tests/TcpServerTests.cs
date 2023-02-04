﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Dvrp.Ucc.CommunicationServer.Messaging;
using Dvrp.Ucc.CommunicationServer.Messaging.Base;
using Xunit;

namespace Dvrp.Ucc.CommunicationServer.Tests
{
    public class TcpServerTests
    {
        private readonly TcpServer _server;
        private readonly IPEndPoint _serverAddress;

        public TcpServerTests()
        {
            _serverAddress = new IPEndPoint(new IPAddress(new byte[] {127, 0, 0, 1}), 9123);
            // _serverAddress = new IPEndPoint(IPAddress.Any, 9123);

            _server = new TcpServer(_serverAddress, new MockProcessor());
            _server.StartListening();
        }

        [Fact]
        public void DataIsEnqueued() //change
        {
            byte[] inputData = {1, 2, 3, 4, 5};
            var outputData = ClientCreateSendAndReceive(inputData);
            var expectedData = new byte[] {5, 4, 3, 2, 1};

            Assert.Equal(outputData.Length, expectedData.Length);
            for (var i = 0; i < expectedData.Length; i++)
            {
                Assert.Equal(outputData[i], expectedData[i]);
            }
        }

        private byte[] ClientCreateSendAndReceive(byte[] dataToSend)
        {
            var bufferSize = 512;
            var buffer = new byte[bufferSize];

            var socket = new Socket(_serverAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(_serverAddress);

            socket.Send(dataToSend);
            socket.Shutdown(SocketShutdown.Send);

            using var memory = new MemoryStream(bufferSize);
            int bytesRec;
            while ((bytesRec = socket.Receive(buffer)) > 0)
            {
	            memory.Write(buffer, 0, bytesRec);
            }

            socket.Shutdown(SocketShutdown.Receive);
            socket.Close();
            buffer = memory.ToArray();
            return buffer;
        }

        public class MockProcessor : IDataProcessor
        {
            public void EnqueueDataToProcess(byte[] data, Metadata metadata, ProcessedDataCallback callback)
            {
                Array.Reverse(data);

                new Task(() => { callback(data); }).Start();
            }

            public void StartProcessing()
            {
            }

            public void StopProcessing()
            {
            }

            public bool IsProcessing { get; set; }
        }
    }
}