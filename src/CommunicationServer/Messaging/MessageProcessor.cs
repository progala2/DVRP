using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Collections;
using System;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageProcessor
    {
        public delegate void MessageReceptionEventHandler(object sender, MessageReceptionEventArgs e);
        public event MessageReceptionEventHandler MessageReception;

        private InputMessageQueue _inputQueue;
        private Marshaller _marshaller;
        private Task _processingThread;

        public MessageProcessor(Marshaller marshaller)
        {
            _inputQueue = new InputMessageQueue();
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

        public void EnqueueOutputMessage(ComponentType addresseeType, Message msg)
        {
            // legacy (nie zmieniać dopóki Rad nie skończy pracy nad synchronizacją)
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

                    if (input.Length > 1)
                        throw new Exception("More than one message received at once during a single connection.");

                    foreach (var message in input)
                    {
                        if (message is IIdentifiableBySender || message is RegisterMessage || message is SolutionRequestMessage || message is SolveRequestMessage)
                        {
                            var responseMsgs = GetResponseMessages(message);

                            var rawResponse = _marshaller.Marshall(responseMsgs);
                            new Task(() => { callback(rawResponse); }).Start();
                        }
                        else
                            throw new Exception("Wrong type of Message received.");
                    }
                }
                else
                {
                    _processingThread = null;
                    return;
                }
            }
        }

        private Message[] GetResponseMessages(Message msg)
        {
            switch(msg.MessageType)
            {
                case Message.MessageClassType.Register:
                    var registerMsg = msg as RegisterMessage;
                    //var id = ComponentMonitor.Instance.Register(registerMsg.Type.
                    //var registerResponseMessage = new RegisterResponseMessage()
                   //     .

                    break;
                case Message.MessageClassType.Status:

                    break;
                case Message.MessageClassType.SolveRequest:

                    break;
                case Message.MessageClassType.SolutionRequest:

                    break;
                case Message.MessageClassType.PartialProblems:

                    break;
                case Message.MessageClassType.Solutions:

                    break;
                default:
                    throw new Exception("Unsupported type received: " + msg.MessageType.ToString());
                    break;
            }



            return null;
        }

        // TODO additional MessageProcessor for backup CS

    }
}
