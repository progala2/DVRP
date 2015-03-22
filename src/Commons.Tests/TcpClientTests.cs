using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class TcpClientTests
    {
        private const int Port = 9123;
        private readonly IPAddress ipAddress = new IPAddress(new byte[]{127, 0, 0, 1});
        private const int BufferSize = 1024;
        Socket _socket;


        [TestMethod]
        public void TcpClientConnectingWithSpecificSocketAndReceivingAnswer()
        {
            TcpClient client = new TcpClient(new IPEndPoint(ipAddress, Port));

            const string message = "to jest wiadomosc do przekazania";

            byte[] data = Encoding.ASCII.GetBytes(message);

            new Task(new Action(StartListening)).Start();
            byte[] received = client.SendData(data);



            Assert.AreEqual(data.Length, received.Length);
            for (int i = 0; i < data.Length; ++i)
            {
                Assert.AreEqual(data[i], received[i]);
                i++;
            }
        }

        private void  StartListening()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(new IPEndPoint(ipAddress, Port));
            _socket.Listen(10);
            
            Socket handlerSocket = _socket.Accept();
            byte[] bytes = new byte[BufferSize];
            int bytesReceived = handlerSocket.Receive(bytes);

            bytes = bytes.Take(bytesReceived).ToArray();

            handlerSocket.Send(bytes);
            handlerSocket.Shutdown(SocketShutdown.Both);
            handlerSocket.Close();
        }
    }
}
