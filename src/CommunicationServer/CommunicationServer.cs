using Dvrp.Ucc.CommunicationServer.Components;
using Dvrp.Ucc.CommunicationServer.Components.Base;
using Dvrp.Ucc.CommunicationServer.Messaging;
using Dvrp.Ucc.CommunicationServer.Messaging.Base;
using Dvrp.Ucc.CommunicationServer.WorkManagement;
using Dvrp.Ucc.CommunicationServer.WorkManagement.Base;

namespace Dvrp.Ucc.CommunicationServer
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

        /// <summary>
        /// Creates CommunicationServer instance.
        /// </summary>
        /// <param name="config">Server configuration.</param>
        public CommunicationServer(ServerConfig config)
        {
            // TODO Communication server should be a backup by default. Some work is needed here.

            Config = config;

            _componentOverseer = new ComponentOverseer(Config.CommunicationTimeout, ComponentOverseerCheckInterval);
            IWorkManager workManager = new WorkManager(_componentOverseer);
            _msgProcessor = new MessageProcessor(_componentOverseer, workManager);
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