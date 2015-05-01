using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class IpEndPointParserTests
    {
        private readonly int _port = 12345;

        [TestMethod]
        public void Pv4StringReturnIpEndPointWithInternetworkAddressFamily()
        {
            var address = "127.0.0.1";
            var ipEndPoint = IpEndPointParser.Parse(address, _port);
            Assert.IsTrue(ipEndPoint.AddressFamily == AddressFamily.InterNetwork);
        }

        [TestMethod]
        public void Pv6StringReturnIpEndPointWithInternetworkV6AddressFamily()
        {
            var address = "::1";
            var ipEndPoint = IpEndPointParser.Parse(address, _port);
            Assert.IsTrue(ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6);
        }

        [TestMethod]
        public void HostNameReturnIpEndPointWithInternetworkV6AddressFamily()
        {
            var address = Dns.GetHostName();
            var ipEndPoint = IpEndPointParser.Parse(address, _port);
            Assert.IsTrue(ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6);
        }
    }
}