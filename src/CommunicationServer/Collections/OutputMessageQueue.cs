using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Collections
{
    internal class OutputMessageQueue
    {
        /*  
         * Messages below are NOT stored in any of the queues:
         *  - RegisterResponse
         *  - NoOperation 
         *  These shall be generated on-the-go instead.
         */

        private Dictionary<string, ConcurrentQueue<Message>> _msgsForComputationalNode;
        private Dictionary<string, ConcurrentQueue<Message>> _msgsForTaskManager;
        private ConcurrentQueue<Message> _msgsForBackupServer;
        private ConcurrentDictionary<ulong, Message> _finalSolutionMessages;

        public OutputMessageQueue()
        {
            _msgsForComputationalNode = new Dictionary<string, ConcurrentQueue<Message>>();
            _msgsForTaskManager = new Dictionary<string, ConcurrentQueue<Message>>();
            _msgsForBackupServer = new ConcurrentQueue<Message>();
            _finalSolutionMessages = new ConcurrentDictionary<ulong, Message>();
        }

        public Message[] GetTaskManagerMessages(string[] problemType, int max)
        {
            return null;
        }

        public Message[] GetComputationalNodeMessages(string[] problemType, int max)
        {
            return null;
        }

        public Message GetSolution(ulong id)
        {
            return null;
        }

        public void Enqueue(Message message)
        {
            //TODO
        }


    }
}
