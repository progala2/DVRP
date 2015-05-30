using System.Net;

namespace _15pl04.Ucc.CommunicationServer
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
            Primary = 1,
            Backup = 2
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