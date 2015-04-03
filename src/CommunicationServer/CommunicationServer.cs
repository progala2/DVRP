using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Messaging;
using System;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class CommunicationServer
    {
        public ServerConfig Config { get; private set; }

        private AsyncTcpServer _tcpServer;
        private MessageProcessor _messageProcessor;

        public CommunicationServer(ServerConfig config)
        {
            Config = config;

            var validator = new Validator();
            var serializer = new MessageSerializer();
            var marshaller = new Marshaller(serializer, validator);

            _messageProcessor = new MessageProcessor(marshaller, config.CommunicationTimeout);
            _tcpServer = new AsyncTcpServer(config, _messageProcessor);
        }

        public void Start()
        {
            Console.WriteLine("Starting Communication Server...");

            ComponentMonitor.Instance.StartMonitoring(Config.CommunicationTimeout);
            _tcpServer.StartListening();

            Console.WriteLine("Communication Server started");
        }

        public void Stop()
        {
            _tcpServer.StopListening();
            ComponentMonitor.Instance.StopMonitoring();
        }

        public void SwitchToPrimaryMode()
        {
            // TODO;
        }
    }
}
