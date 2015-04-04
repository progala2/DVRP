using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class AsyncTcpServerTests
    {
        private AsyncTcpServer _server;
        

        public AsyncTcpServerTests()
        {
            var address = new IPEndPoint(new IPAddress(new byte[]{127,0,0,1}), 9123);

            _server = new AsyncTcpServer(address, new MockProcessor());

            _server.StartListening();


        }

        public class MockProcessor : IDataProcessor<IPEndPoint>
        {
            public void EnqueueDataToProcess(byte[] data, IPEndPoint metadata, ProcessedDataCallback callback)
            {
                callback.Invoke(new byte[] { 1, 2, 3 });
            }
        }

    }
}

