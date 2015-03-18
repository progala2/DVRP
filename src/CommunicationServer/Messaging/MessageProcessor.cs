using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using System;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageProcessor : IDataProcessor
    {
        private IUnmarshaller<Message> _unmarshaller;

        public MessageProcessor(IUnmarshaller<Message> unmarshaller) 
        {
            _unmarshaller = unmarshaller;
        }

        public byte[] ProcessData(byte[] data)
        {
            Message[] messages = _unmarshaller.Unmarshall(data);

            /*
             * 1. Wrzucić wiadomości do InputMessageQueue
             * 
             * 
             * 
             * 
             */ 

            throw new NotImplementedException();
        }
    }
}
