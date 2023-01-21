using System.Net.Sockets;
using _15pl04.Ucc.Commons.Utilities;
using Xunit;

namespace _15pl04.Ucc.Commons.Tests
{
    public class IpEndPointParserTests
    {
        private readonly int _port = 12345;

        [Fact]
        public void Pv4StringReturnIpEndPointWithInternetworkAddressFamily()
        {
            var address = "127.0.0.1";
            var ipEndPoint = IpEndPointParser.Parse(address, _port);
            Assert.True(ipEndPoint.AddressFamily == AddressFamily.InterNetwork);
        }

        [Fact]
        public void Pv6StringReturnIpEndPointWithInternetworkV6AddressFamily()
        {
            var address = "::1";
            var ipEndPoint = IpEndPointParser.Parse(address, _port);
            Assert.True(ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6);
        }
    }
}