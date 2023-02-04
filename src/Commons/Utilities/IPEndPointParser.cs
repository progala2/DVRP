using System;
using System.Linq;
using System.Net;

namespace Dvrp.Ucc.Commons.Utilities
{
    /// <summary>
    /// Provides converting IP end point representation to an instance of System.Net.IPEndPoint class.
    /// </summary>
    public static class IpEndPointParser
    {
        /// <summary>
        /// Converts IP end point representation to an instance of System.Net.IPEndPoint class.
        /// </summary>
        /// <param name="endPointString">The address as IPv4/IPv6/host name.</param>
        /// <returns>The parsed IP end point.</returns>
        /// <exception cref="System.FormatException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static IPEndPoint Parse(string endPointString)
        {
            return Parse(endPointString, -1);
        }

        /// <summary>
        /// Converts IP end point representation to an instance of System.Net.IPEndPoint class.
        /// </summary>
        /// <param name="endPointString">The address as IPv4/IPv6/host name.</param>
        /// <param name="defaultPort">The default port.</param>
        /// <returns>The parsed IP end point.</returns>
        /// <exception cref="System.FormatException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static IPEndPoint Parse(string endPointString, string defaultPort)
        {
	        if (int.TryParse(defaultPort, out var port))
                return Parse(endPointString, port);
            return Parse(endPointString);
        }

        /// <summary>
        /// Converts IP end point representation to an instance of System.Net.IPEndPoint class.
        /// </summary>
        /// <param name="endPointString">The address as IPv4/IPv6/host name.</param>
        /// <param name="defaultPort">The default port.</param>
        /// <returns>The parsed IP end point.</returns>
        /// <exception cref="System.FormatException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static IPEndPoint Parse(string endPointString, int defaultPort)
        {
            if (string.IsNullOrWhiteSpace(endPointString))
                throw new ArgumentException("Endpoint descriptor may not be empty.");

            if (defaultPort != -1 && (defaultPort < IPEndPoint.MinPort || defaultPort > IPEndPoint.MaxPort))
                throw new ArgumentException($"Invalid default port '{defaultPort}'");

            var values = endPointString.Split(':');
            IPAddress? ipAddress;
            int port;

            switch (values.Length)
            {
	            // check if we have an IPv6 or ports
	            // ipv4 or hostname
	            case <= 2:
	            {
		            port = values.Length == 1 ? defaultPort : GetPort(values[1]);

		            // try to use the address as IPv4, otherwise get hostname
		            if (!IPAddress.TryParse(values[0], out ipAddress))
			            ipAddress = GetIpFromHost(values[0]);
		            break;
	            }
	            // ipv6
	            // could [a:b:c]:d
	            case > 2 when values[0].StartsWith("[") && values[^2].EndsWith("]"):
	            {
		            var ipAddressString = string.Join(":", values.Take(values.Length - 1).ToArray());
		            ipAddress = IPAddress.Parse(ipAddressString);
		            port = GetPort(values[^1]);
		            break;
	            }
	            // [a:b:c] or a:b:c
	            case > 2:
		            ipAddress = IPAddress.Parse(endPointString);
		            port = defaultPort;
		            break;
            }

            if (port == -1)
                throw new ArgumentException($"No port specified: '{endPointString}'");

            return new IPEndPoint(ipAddress, port);
        }

        /// <summary>
        /// Get port from string.
        /// </summary>
        /// <param name="p">The port as string.</param>
        /// <returns>The port as int.</returns>
        /// <exception cref="FormatException">Invalid port number.</exception>
        private static int GetPort(string p)
        {
	        if (!int.TryParse(p, out var port) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                throw new FormatException($"Invalid end point port '{p}'");

            return port;
        }

        /// <summary>
        /// Gets IPAddress based on host name or address.
        /// </summary>
        /// <param name="hostNameOrAddress">Host name or address.</param>
        /// <returns>The IPAddress.</returns>
        /// <exception cref="FormatException">Host not found.</exception>
        private static IPAddress GetIpFromHost(string hostNameOrAddress)
        {
            var hosts = Dns.GetHostAddresses(hostNameOrAddress);

            if (hosts == null || hosts.Length == 0)
                throw new ArgumentException($"Host not found: {hostNameOrAddress}");

            return hosts[0];
        }
    }
}