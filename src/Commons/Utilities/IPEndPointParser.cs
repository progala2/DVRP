using System;
using System.Linq;
using System.Net;

namespace _15pl04.Ucc.Commons.Utilities
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
            int port;
            if (int.TryParse(defaultPort, out port))
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
                throw new ArgumentException(string.Format("Invalid default port '{0}'", defaultPort));

            var values = endPointString.Split(':');
            IPAddress ipAddress;
            var port = -1;

            // check if we have an IPv6 or ports
            if (values.Length <= 2) // ipv4 or hostname
            {
                port = values.Length == 1 ? defaultPort : GetPort(values[1]);

                // try to use the address as IPv4, otherwise get hostname
                if (!IPAddress.TryParse(values[0], out ipAddress))
                    ipAddress = GetIpFromHost(values[0]);
            }
            else if (values.Length > 2) // ipv6
            {
                // could [a:b:c]:d
                if (values[0].StartsWith("[") && values[values.Length - 2].EndsWith("]"))
                {
                    var ipaddressstring = string.Join(":", values.Take(values.Length - 1).ToArray());
                    ipAddress = IPAddress.Parse(ipaddressstring);
                    port = GetPort(values[values.Length - 1]);
                }
                else // [a:b:c] or a:b:c
                {
                    ipAddress = IPAddress.Parse(endPointString);
                    port = defaultPort;
                }
            }
            else
            {
                throw new FormatException(string.Format("Invalid endpoint ipaddress '{0}'", endPointString));
            }

            if (port == -1)
                throw new ArgumentException(string.Format("No port specified: '{0}'", endPointString));

            return new IPEndPoint(ipAddress, port);
        }

        /// <summary>
        /// Get port from string.
        /// </summary>
        /// <param name="p">The port as string.</param>
        /// <returns>The port as int.</returns>
        /// <exception cref="System.FormatException">Invalid port number.</exception>
        private static int GetPort(string p)
        {
            int port;

            if (!int.TryParse(p, out port) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                throw new FormatException(string.Format("Invalid end point port '{0}'", p));

            return port;
        }

        /// <summary>
        /// Gets IPAddress based on host name or address.
        /// </summary>
        /// <param name="hostNameOrAddress">Host name or address.</param>
        /// <returns>The IPAddress.</returns>
        /// <exception cref="System.FormatException">Host not found.</exception>
        private static IPAddress GetIpFromHost(string hostNameOrAddress)
        {
            var hosts = Dns.GetHostAddresses(hostNameOrAddress);

            if (hosts == null || hosts.Length == 0)
                throw new ArgumentException(string.Format("Host not found: {0}", hostNameOrAddress));

            return hosts[0];
        }
    }
}