using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Collections;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageProcessor
    {
        private InputMessageQueue _inputQueue;
        private OutputMessageQueue _outputQueue;
        private Marshaller _marshaller;

        public MessageProcessor(InputMessageQueue inputQueue, OutputMessageQueue outputQueue, Marshaller marshaller) 
        {
            _inputQueue = inputQueue;
            _outputQueue = outputQueue;
            _marshaller = marshaller;
        }

        public void StartProcessing()
        {
            /*
             * Przetwarzanie z użyciem ThredPool.
             */ 
        }

        public void StopProcessing()
        {

        }

    }
}
