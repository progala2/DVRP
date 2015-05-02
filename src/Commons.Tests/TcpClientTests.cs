using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeoutException = _15pl04.Ucc.Commons.Exceptions.TimeoutException;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class TcpClientTests
    {
        private const int Port = 9123;
        private const int BufferSize = 2048;
        private readonly IPAddress _ipAddressv4 = new IPAddress(new byte[] {127, 0, 0, 1});
        private readonly IPAddress _ipAddressv6 = IPAddress.Parse("0:0:0:0:0:0:0:1");
        private Socket _socket;

        [TestMethod]
        public void TcpClientConnectingWithSpecifiedSocketAndReceivingAnswerIpV4()
        {
            var client = new TcpClient(new IPEndPoint(_ipAddressv4, Port));

            const string message = "to jest wiadomosc do przekazania";

            var data = Encoding.UTF8.GetBytes(message);

            var t = new Task(ListenAndResendV4);
            t.Start();
            var received = client.SendData(data);


            Assert.AreEqual(data.Length, received.Length);
            for (var i = 0; i < data.Length; ++i)
            {
                Assert.AreEqual(data[i], received[i]);
                i++;
            }
            //check whether wait or dispose
            EndConnection();
            t.Wait();
        }

        [TestMethod]
        public void TcpClientConnectingWithSpecifiedSocketAndReceivingAnswerIpv6()
        {
            var client = new TcpClient(new IPEndPoint(_ipAddressv6, Port));

            const string message = "to jest wiadomosc do przekazania";

            var data = Encoding.UTF8.GetBytes(message);

            var t = new Task(ListenAndResendV6);
            t.Start();
            var received = client.SendData(data);


            Assert.AreEqual(data.Length, received.Length);
            for (var i = 0; i < data.Length; ++i)
            {
                Assert.AreEqual(data[i], received[i]);
                i++;
            }
            //check whether wait or dispose
            EndConnection();
            t.Wait();
        }

        //long test, takes TimeoutSeconds seconds
        [ExpectedException(typeof (TimeoutException))]
        [TestMethod]
        public void TcpClientConnectingToWrongIpAndThrowingOwnException()
        {
            var t = new Task(StartListeningV4);
            t.Start();
            try
            {
                var client = new TcpClient(
                    new IPEndPoint(new IPAddress(new byte[] {126, 0, 0, 1}), Port));

                const string message = "to jest wiadomosc do przekazania";

                var data = Encoding.UTF8.GetBytes(message);

                var received = client.SendData(data);
            }
            catch (TimeoutException e)
            {
                EndConnection();
                throw e;
            }
            catch (Exception)
            {
                EndConnection();
                throw;
            }

            //throw new Exception();
        }

        private void ListenAndResendV4()
        {
            StartListeningV4();
            AcceptConnection();
            //EndConnection();
        }

        private void ListenAndResendV6()
        {
            StartListeningV6();
            AcceptConnection();
            //EndConnection();
        }

        private void AcceptConnection()
        {
            var handlerSocket = _socket.Accept();
            var bytes = new byte[BufferSize];
            var bytesReceived = handlerSocket.Receive(bytes);

            bytes = bytes.Take(bytesReceived).ToArray();

            handlerSocket.Send(bytes);
            handlerSocket.Shutdown(SocketShutdown.Send);
            handlerSocket.Close();
        }

        private void EndConnection()
        {
            //Thread.Sleep(1500);
            _socket.Close();
        }

        private void StartListeningV4()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(new IPEndPoint(_ipAddressv4, Port));
            _socket.Listen(10);
        }

        private void StartListeningV6()
        {
            _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(new IPEndPoint(_ipAddressv6, Port));
            _socket.Listen(10);
        }
    }
}