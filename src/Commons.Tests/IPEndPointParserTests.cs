using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class IpEndPointParserTests
    {
        private readonly int port = 12345;

        [TestMethod]
        public void IPv4StringReturnIPEndPointWithInternetworkAddressFamily()
        {
            var address = "127.0.0.1";
            var ipEndPoint = IPEndPointParser.Parse(address, port);
            Assert.IsTrue(ipEndPoint.AddressFamily == AddressFamily.InterNetwork);
        }

        [TestMethod]
        public void IPv6StringReturnIPEndPointWithInternetworkV6AddressFamily()
        {
            var address = "::1";
            var ipEndPoint = IPEndPointParser.Parse(address, port);
            Assert.IsTrue(ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6);
        }

        [TestMethod]
        public void HostNameReturnIPEndPointWithInternetworkV6AddressFamily()
        {
            var address = Dns.GetHostName();
            var ipEndPoint = IPEndPointParser.Parse(address, port);
            Assert.IsTrue(ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6);
        }
    }
}