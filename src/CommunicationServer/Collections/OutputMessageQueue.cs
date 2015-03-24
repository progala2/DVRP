using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    internal class OutputMessageQueue
    {
        private Dictionary<string, ConcurrentQueue<Message>> _msgsForComputationalNode;
        private Dictionary<string, ConcurrentQueue<Message>> _msgsForTaskManager;
        private ConcurrentQueue<Message> _msgsForBackupServer;
        private ConcurrentDictionary<ulong, Message> _finalSolutionMsgs;

        public OutputMessageQueue()
        {
            //TODO
        }

        public Message[] Dequeue(ComponentType componentType, string[] problemType, int max)
        {
            //TODO

            switch (componentType)
            {
                case ComponentType.CommunicationServer:

                    break;
            }

            
            return null;
        }

        public void Enqueue(Message message)
        {
            //TODO
        }


    }
}
