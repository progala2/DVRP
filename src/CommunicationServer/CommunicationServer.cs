using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Messaging;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class CommunicationServer
    {
        private AsyncTcpServer _tcpServer;
        private ComponentStateMonitor _componentStateMonitor;
        private AsyncMessageProcessor _messageProcessor;

        public CommunicationServer(ServerConfig config)
        {
            var inputQueue = new InputMessageQueue();
            var outputQueue = new OutputMessageQueue();
            var marshaller = new Marshaller();

            _tcpServer = new AsyncTcpServer(config, inputQueue);
            _messageProcessor = new AsyncMessageProcessor(inputQueue, outputQueue, marshaller);
            _componentStateMonitor = new ComponentStateMonitor();
        }

        public void Start()
        {
            _tcpServer.StartListening();
            _messageProcessor.StartProcessing();
            _componentStateMonitor.Start();
        }

        public void Stop()
        {
            _tcpServer.StopListening();
            _messageProcessor.StopProcessing();
            _componentStateMonitor.Stop();
        }
    }
}
