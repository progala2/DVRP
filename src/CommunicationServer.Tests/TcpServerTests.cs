using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class TcpServerTests
    {
        private TcpServer _server;


        public TcpServerTests()
        {
            var address = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 9123);

            _server = new TcpServer(address, new MockProcessor());
            _server.StartListening();
        }

        

        public class MockProcessor : IDataProcessor
        {
            public void EnqueueDataToProcess(byte[] data, Metadata metadata, ProcessedDataCallback callback)
            {
                throw new System.NotImplementedException();
            }

            public void StartProcessing()
            {
                throw new System.NotImplementedException();
            }

            public void StopProcessing()
            {
                throw new System.NotImplementedException();
            }

            public bool IsProcessing
            {
                get { throw new System.NotImplementedException(); }
            }
        }

    }
}

