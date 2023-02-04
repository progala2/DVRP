using System.Net;

namespace Dvrp.Ucc.CommunicationServer
{
    /// <summary>
    /// Server configuration.
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// Server mode: primary/backup.
        /// </summary>
        public enum ServerMode
        {
            /// <summary>
            /// 
            /// </summary>
            Primary = 1,
            /// <summary>
            /// 
            /// </summary>
            Backup = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="address"></param>
        /// <param name="communicationTimeout"></param>
        public ServerConfig(ServerMode mode, IPEndPoint address, uint communicationTimeout)
        {
	        Mode = mode;
	        Address = address;
	        CommunicationTimeout = communicationTimeout;
        }

        /// <summary>
        /// Current server mode.
        /// </summary>
        public ServerMode Mode { get; set; }
        /// <summary>
        /// Server's address.
        /// </summary>
        public IPEndPoint Address { get; set; }
        /// <summary>
        /// Timeout for communication with server clients.
        /// </summary>
        public uint CommunicationTimeout { get; set; }
    }
}