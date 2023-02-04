using System;
using System.Text;

namespace Dvrp.Ucc.Commons.Components
{
    /// <summary>
    /// Server-specific information.
    /// </summary>
    [Serializable]
    public class ServerInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
	    public ServerInfo(string ipAddress, ushort port)
	    {
		    IpAddress = ipAddress;
		    Port = port;
	    }

	    /// <summary>
        /// Server IP address.
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// Listening port.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(IpAddress);
            builder.Append(':');
            builder.Append(Port.ToString("0000"));

            return builder.ToString();
        }
    }
}