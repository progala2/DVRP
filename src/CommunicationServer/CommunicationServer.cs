using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using _15pl04.Ucc.CommunicationServer.Messaging;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;

namespace _15pl04.Ucc.CommunicationServer
{
    /// <summary>
    /// Main communication server class.
    /// </summary>
    internal class CommunicationServer
    {
        private const uint ComponentOverseerCheckInterval = 1;
        private readonly IComponentOverseer _componentOverseer;
        private readonly IDataProcessor _msgProcessor;
        private readonly TcpServer _tcpServer;
        private readonly IWorkManager _workManager;

        /// <summary>
        /// Creates CommunicationServer instance.
        /// </summary>
        /// <param name="config">Server configuration.</param>
        public CommunicationServer(ServerConfig config)
        {
            // TODO Communication server should be a backup by default. Some work is needed here.

            Config = config;

            _componentOverseer = new ComponentOverseer(Config.CommunicationTimeout, ComponentOverseerCheckInterval);
            _workManager = new WorkManager(_componentOverseer);
            _msgProcessor = new MessageProcessor(_componentOverseer, _workManager);
            _tcpServer = new TcpServer(Config.Address, _msgProcessor);
        }

        /// <summary>
        /// Server configuration.
        /// </summary>
        public ServerConfig Config { get; }

        /// <summary>
        /// Starts all modules of the server.
        /// </summary>
        public void Start()
        {
            _tcpServer.StartListening();
            _msgProcessor.StartProcessing();
            _componentOverseer.StartMonitoring();
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            _tcpServer.StopListening();
            _componentOverseer.StopMonitoring();
            _msgProcessor.StopProcessing();
        }

        /// <summary>
        /// Takes over the primary server functions.
        /// </summary>
        public void SwitchToPrimaryMode()
        {
        }
    }
}