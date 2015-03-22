using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Messaging;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class CommunicationServer
    {
        public ServerConfig Config { get; private set; }

        private AsyncTcpServer _tcpServer;
        private MessageQueuer _messageProcessor;


        public CommunicationServer(ServerConfig config)
        {
            Config = config;

            var marshaller = new Marshaller();

            _messageProcessor = new MessageQueuer(marshaller);
            _tcpServer = new AsyncTcpServer(config, _messageProcessor);
            
        }

        public void Start()
        {
            _tcpServer.StartListening();
            ComponentMonitor.Instance.StartMonitoring(Config.Timeout);
        }

        public void Stop()
        {
            _tcpServer.StopListening();
            ComponentMonitor.Instance.StopMonitoring();
        }
    }
}
