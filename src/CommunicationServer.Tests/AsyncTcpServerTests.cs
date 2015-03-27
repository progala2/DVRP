using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.CommunicationServer.Messaging;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class AsyncTcpServerTests
    {
        private static readonly IPAddress TestIp = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private const int Port = 9123;
        private const int BufferSize = 204800;
        private AsyncTcpServer _tcpServer;

        //[TestMethod]
        public void Test1() //not working properly yet
        {
            //Preparing TcpServer
            Init();

            //Sending
            Socket socket;
            InitSocket(out socket);

            const string message = "To jest wiadomosc testowa majaca wiecej bajtow niz 8";
            byte[] expectedResponse = Encoding.ASCII.GetBytes(message);

            byte[] response =  Send(socket, message);


            Assert.AreEqual(expectedResponse.Length, response.Length);
            for (int i = 0; i < response.Length; ++i)
            {
                Assert.AreEqual(expectedResponse[i], response[i]);
                i++;
            }
            _tcpServer.StopListening();
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
            }
        }

        private byte[] Send(Socket socket, string message)
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
                    return null;
                }
                socket.Shutdown(SocketShutdown.Send);

                int bytesRec = socket.Receive(bytes);
                Debug.WriteLine("Length of echoed test = {0}",
                    bytesRec);
                //should server or client close socket?
                socket.Close();
                return bytes.Take(bytesRec).ToArray();

            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                throw;
            }

        }

    }
}

