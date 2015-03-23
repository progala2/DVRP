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
        public void TcpClientConnectingWithSpecifiedSocketAndReceivingAnswer()
        {
            TcpClient client = new TcpClient(new IPEndPoint(ipAddress, Port));

            const string message = "to jest wiadomosc do przekazania";

            byte[] data = Encoding.ASCII.GetBytes(message);

            Task t = new Task(new Action(ListenAndResend));
            t.Start();
            byte[] received = client.SendData(data);



            Assert.AreEqual(data.Length, received.Length);
            for (int i = 0; i < data.Length; ++i)
            {
                Assert.AreEqual(data[i], received[i]);
                i++;
            }
            //check whether wait or dispose
            t.Wait();
        }

        //long test, about 20 seconds
        [ExpectedException(typeof(Commons.Exceptions.TimeoutException))]
        [TestMethod]
        public void TcpClientConnectingToWrongIPAndThrowingOwnException()
        {
            Task t = new Task(new Action(StartListening));
            t.Start();
            try
            {
                TcpClient client = new TcpClient(
                new IPEndPoint(new IPAddress(new Byte[] { 126, 0, 0, 1 }), Port));

                const string message = "to jest wiadomosc do przekazania";

                byte[] data = Encoding.ASCII.GetBytes(message);
                                
                byte[] received = client.SendData(data);
            }
            catch (Commons.Exceptions.TimeoutException e)
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

        private void ListenAndResend()
        {
            StartListening();
            AcceptConnection();
            EndConnection();
        }

        private void AcceptConnection()
        {
            Socket handlerSocket = _socket.Accept();
            byte[] bytes = new byte[BufferSize];
            int bytesReceived = handlerSocket.Receive(bytes);

            bytes = bytes.Take(bytesReceived).ToArray();

            handlerSocket.Send(bytes);
            handlerSocket.Shutdown(SocketShutdown.Both);
            handlerSocket.Close();  
        }

        private void EndConnection()
        {
            _socket.Close();
        }

        private void StartListening()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(new IPEndPoint(ipAddress, Port));
            _socket.Listen(10);
        }
    }
}
