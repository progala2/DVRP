using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class SynchronizationManager
    {
        private readonly MessageProcessor _processor;

        public SynchronizationManager(MessageProcessor processor)
        {
            _processor = processor;
        }

        private void AddSynchronizationMessage(Message message)
        {
            _processor.EnqueueOutputMessage(ComponentType.CommunicationServer, message);
        }
    }
}
