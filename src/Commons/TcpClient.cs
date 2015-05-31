using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using _15pl04.Ucc.Commons.Exceptions;

namespace _15pl04.Ucc.Commons
{
    /// <summary>
    /// Class allowing direct sending data and receiving response from hosts.
    /// </summary>
    public class TcpClient
    {
        /// <summary>
        /// Main server address.
        /// </summary>
        public IPEndPoint ServerAddress { get; set; }

        private const int BufferSize = 1024;

        /// <summary>
        /// Main server address.
        /// </summary>
        /// <param name="serverAddress"></param>
        public TcpClient(IPEndPoint serverAddress)
        {
            ServerAddress = serverAddress;
        }

        /// <summary>
        /// Send data to the server and returns its response.
        /// </summary>
        /// <param name="data">Byte data to send.</param>
        /// <returns>Marshalled messages from the server.</returns>
        /// <exception cref="_15pl04.Ucc.Commons.Exceptions.TimeoutException">Connection to host timed out.</exception>
        public byte[] SendData(byte[] data)
        {
            var buf = new byte[BufferSize];

            var socket = new Socket(ServerAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                socket.Connect(ServerAddress);

                Debug.WriteLine("Socket connected to " + ServerAddress);

                socket.Send(data);
                socket.Shutdown(SocketShutdown.Send);

                using (var memory = new MemoryStream(BufferSize))
                {
                    int bytesRec;
                    while ((bytesRec = socket.Receive(buf)) > 0)
                    {
                        memory.Write(buf, 0, bytesRec);
                        Debug.WriteLine("Capacity: " + memory.Capacity + " Length: " + memory.Length);
                    }

                    socket.Shutdown(SocketShutdown.Receive);
                    socket.Close();
                    buf = memory.ToArray();
                }
            }
            catch (SocketException e)
            {
                switch (e.ErrorCode)
                {
                    case 10060: //timeout
                        throw new TimeoutException(ServerAddress.ToString(), e);
                    default:
                        throw;
                }
            }
            return buf;
        }
    }
}