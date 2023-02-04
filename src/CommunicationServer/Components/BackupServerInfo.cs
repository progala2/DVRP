using System.Net;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.CommunicationServer.Components.Base;

namespace Dvrp.Ucc.CommunicationServer.Components
{
    /// <summary>
    /// Information about backup server.
    /// </summary>
    public class BackupServerInfo : ComponentInfo
    {
	    /// <summary>
	    /// Creates BackupServerInfo instance.
	    /// </summary>
	    /// <param name="id"></param>
	    /// <param name="serverInfo">Communication Server specific information.</param>
	    /// <param name="numberOfThreads">Number of threads provided by the backup.</param>
	    public BackupServerInfo(ulong id, ServerInfo serverInfo, int numberOfThreads)
            : base(id, ComponentType.CommunicationServer, numberOfThreads)
        {
            Address = new IPEndPoint(
                IPAddress.Parse(serverInfo.IpAddress),
                serverInfo.Port);
        }

        /// <summary>
        /// Creates BackupServerInfo instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="endPoint">IPEndPoint data of the backup.</param>
        /// <param name="numberOfThreads">Number of threads provided by the backup.</param>
        public BackupServerInfo(ulong id, IPEndPoint endPoint, int numberOfThreads)
            : base(id, ComponentType.CommunicationServer, numberOfThreads)
        {
            Address = endPoint;
        }

        /// <summary>
        /// Backup server's address.
        /// </summary>
        public IPEndPoint Address { get; }
    }
}