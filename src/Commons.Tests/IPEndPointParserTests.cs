using System.Net.Sockets;
using Dvrp.Ucc.Commons.Utilities;
using Xunit;

namespace Dvrp.Ucc.Commons.Tests
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