using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace _15pl04.Ucc.Commons
{
    public class TcpClient
    {
        public IPEndPoint ServerAddress {get;set;}

#if DEBUG
        private const int BufferSize = 8;
#else
        private const int BufferSize = 1024;
#endif

        public TcpClient(IPEndPoint serverAddress)
        {
            ServerAddress = serverAddress;
        }

        /// <summary>
        /// Functions send data to server and returns server's respnse
        /// </summary>
        /// <param name="data">data to send</param>
        /// <returns>data received from host</returns>
        /// <exception cref="_15pl04.Ucc.Commons.Exceptions.TimeoutException">connection to host timed out</exception>
        public byte[] SendData(byte[] data)
        {
            byte[] buf = new byte[BufferSize];

            try
            {
                Socket socket = new Socket(ServerAddress.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                try
                {
                    socket.Connect(ServerAddress);

                    Debug.WriteLine("Socket connected to " + ServerAddress.ToString());

                    socket.Send(data);
                    socket.Shutdown(SocketShutdown.Send);

                    using (MemoryStream memory = new MemoryStream(BufferSize))
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
                            throw new Commons.Exceptions.TimeoutException(ServerAddress.ToString(), e);
                        default:
                            throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return buf;
        }
    }
}
