using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Messaging;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class CommunicationServer
    {
        private AsyncTcpServer _tcpServer;
        private ComponentMonitor _componentStateMonitor;
        private MessageProcessor _messageProcessor;

        public CommunicationServer(ServerConfig config)
        {
            var inputQueue = new InputMessageQueue();
            var outputQueue = new OutputMessageQueue();
            var marshaller = new Marshaller();

            _tcpServer = new AsyncTcpServer(config, inputQueue);
            _messageProcessor = new MessageProcessor(inputQueue, outputQueue, marshaller);
        }

        public void Start()
        {
            _tcpServer.StartListening();
            _messageProcessor.StartProcessing();
            _componentStateMonitor.StartMonitoring();
        }

        public void Stop()
        {
            _tcpServer.StopListening();
            _messageProcessor.StopProcessing();
            _componentStateMonitor.StopMonitoring();
        }
    }
}
