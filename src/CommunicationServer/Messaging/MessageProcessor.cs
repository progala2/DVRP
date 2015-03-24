using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Collections;
using System;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageProcessor
    {
        private InputMessageQueue _inputQueue;
        private OutputMessageQueue _outputQueue;
        private Marshaller _marshaller;
        private Task _processingThread;

        public MessageProcessor(Marshaller marshaller)
        {
            _inputQueue = new InputMessageQueue();
            _outputQueue = new OutputMessageQueue();
            _marshaller = marshaller;
        }

        public void EnqeueInputMessage(byte[] rawMsg, AsyncTcpServer.ResponseCallback callback)
        {
            _inputQueue.Enqueue(rawMsg, callback);

            if (_processingThread == null)
            {
                _processingThread = new Task(ProcessQueuedMessages);
                _processingThread.Start();
            }
        }

        public void EnqueueOutputMessage(Message msg)
        {
            _outputQueue.Enqueue(msg);
        }

        private void ProcessQueuedMessages()
        {
            while (true)
            {
                byte[] rawMsg;
                AsyncTcpServer.ResponseCallback callback;

                if (_inputQueue.TryDequeue(out rawMsg, out callback))
                {
                    Message[] input = _marshaller.Unmarshall(rawMsg);

                    // This can happen during synchronization process.
                    //if (input.Length != 1)
                    //    throw new Exception("Received more than one message at once from a component.");


                    /*
                     * 1. Get component's type and id.
                     * 2. Check if id is registered.
                     * 3. Dequeue pending messages from the output queue if needed.
                     * 4. Synchronously generate a response if no messages in the output queue.
                     * 5. Marshall response messages.
                     * 6. Call the callback asynchronously.
                     */
                }
                else
                {
                    _processingThread = null;
                    return;
                }
            }
        }

        private void ProcessMessage(Message msg)
        {
            var type = msg.GetType();

            if (type == typeof(RegisterMessage))
            {
                var registerMsg = msg as RegisterMessage;
                   

            }
            else if (type == typeof(StatusMessage))
            {

            }
            else if (type == typeof(SolveRequestMessage))
            {

            }
            else if (type == typeof(SolutionRequestMessage))
            {

            }
            else if (type == typeof(PartialProblemsMessage))
            {

            }
            else if (type == typeof(PartialProblemsMessage))
            {

            }
            else if (type == typeof(SolutionsMessage))
            {

            }
            else
                throw new ArgumentException("Unsupported type received: " + type.FullName);
        }


    }
}
