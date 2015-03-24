using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.CommunicationServer;
using _15pl04.Ucc.CommunicationServer.Messaging;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class AsyncTcpServerTests
    {
        private static readonly IPAddress TestIp = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private const int Port = 9123;
        private const int BufferSize = 2048;
        private AsyncTcpServer _tcpServer;

        [TestMethod]
        public void Test1() //not working properly yet
        {
            //Preparing TcpServer
            Init();

            //Sending
            Socket socket;
            InitSocket(out socket);

            const string message = "Test message, longer than 8 bytes for testing";
            byte[] expectedResponse = Encoding.ASCII.GetBytes(message);

            Send(socket, message);
            string receivedMessage;
            Receive(socket, out receivedMessage);
            for (int i = 0; i < receivedMessage.Length; ++i)
            {
                Assert.AreEqual(receivedMessage[i], message[i]);
                i++;
            }
            Assert.AreEqual(expectedResponse, message);
        }

        private void Init()
        {
            ServerConfig config = new ServerConfig(null)
            {
                Address = new IPEndPoint(new IPAddress(new byte[]{127, 0, 0, 1}), 9123),
                Mode = ServerConfig.ServerMode.Primary,
                Timeout = 10
            } ;

            _tcpServer = new AsyncTcpServer(config, new MessageProcessor(new Marshaller()));
            
            new Thread(_tcpServer.StartListening).Start();
        }

        private void InitSocket(out Socket socket)
        {
            IPEndPoint remoteEp = new IPEndPoint(TestIp, Port);
            socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(remoteEp);
            }
            catch (Exception e)
            {
                throw e;
                //Console.WriteLine(e.ToString());
            }
        }

        private void Receive(Socket socket, out string message)
        {
            message = String.Empty;
            int bytesReceived = 0;
            var bytes = new byte[BufferSize];

            while ((bytesReceived = socket.Receive(bytes)) > 0)
            {
                message += Encoding.ASCII.GetString(bytes, 0, bytesReceived);
            }
            //should server or client close socket?
            socket.Close();
        }

        private void Send(Socket socket, string message)
        {
            byte[] bytes = new byte[BufferSize];

            try
            {
                byte[] msg = Encoding.ASCII.GetBytes(message);

                // Send the data through the socket.
                int bytesSent = socket.Send(msg);

                if (bytesSent != msg.Length)
                {
                    //TODO break
                    return;
                }
                socket.Shutdown(SocketShutdown.Send);

                int bytesRec = socket.Receive(bytes);
                Console.WriteLine("Echoed test = {0}",
                    Encoding.ASCII.GetString(bytes, 0, bytesRec));

                //sender.Close();

            }
            //catch (ArgumentNullException ane)
            //{
            //    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            //}
            //catch (SocketException se)
            //{
            //    Console.WriteLine("SocketException : {0}", se.ToString());
            //}
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                throw;
            }

        }

    }
}

