using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using System;
using System.Net;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{

    internal class MessageProcessor : IDataProcessor<IPEndPoint>
    {
        public void EnqueueDataToProcess(byte[] data, IPEndPoint metadata, ProcessedDataCallback callback)
        {
            
        }

        public MessageProcessor()
        {

        }
    }
}
