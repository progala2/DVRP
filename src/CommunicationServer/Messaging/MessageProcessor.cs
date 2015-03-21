using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Collections;
using System;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageProcessor
    {
        private InputMessageQueue _inputQueue;
        private OutputMessageQueue _outputQueue;
        private Marshaller _marshaller;

        public MessageProcessor(Marshaller marshaller) 
        {
            _inputQueue = new InputMessageQueue();
            _outputQueue = new OutputMessageQueue();
            _marshaller = marshaller;
        }

        public void EnqeueInputMessage(byte[] rawMsg, AsyncTcpServer.ResponseCallback callback)
        {
            _inputQueue.Enqueue(rawMsg, callback);

            // TODO: try to process asynchronously
        }

        public void EnqueueOutputMessage(Message msg)
        {
            _outputQueue.Enqueue(msg);
        }

        private bool TryProcess()
        {
            byte[] rawMsg;
            AsyncTcpServer.ResponseCallback callback;

            if (_inputQueue.TryDequeue(out rawMsg, out callback))
            {
                Message[] input = _marshaller.Unmarshall(rawMsg);

                if (input.Length != 1)
                    throw new Exception("Received more than one message at once from a component.");

                /*
                 * Decide what to do with the message.
                 */

                return true;
            }
            else
                return false;
        }
    }
}
