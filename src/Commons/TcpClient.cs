using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;

namespace _15pl04.Ucc.Commons
{
    public class TcpClient
    {
        private readonly IPEndPoint _serverAddress;
        private readonly int _timeoutMiliseconds = 5000;
#if DEBUG
        private const int BufferSize = 8;
#else
        private const int BufferSize = 1024;
#endif

        public TcpClient(IPEndPoint serverAddress)
        {
            _serverAddress = serverAddress;
        }

        /// <summary>
        /// constructr
        /// </summary>
        /// <param name="serverAddress">server address</param>
        /// <param name="timeoutMiliSeconds">how long client will try to connect to host before informing it couldn't</param>
        public TcpClient(IPEndPoint serverAddress, int timeoutMiliSeconds)
        {
            _serverAddress = serverAddress;
            _timeoutMiliseconds = timeoutMiliSeconds;
        }


        /// <summary>
        /// Functions send data to server and returns server's respnse
        /// </summary>
        /// <param name="data">data to send</param>
        /// <returns>data received from host</returns>
        /// <exception cref="Commons.TimeoutException">connection to host timed out</exception>
        public byte[] SendData(byte[] data)
        {
            List<byte> ret = new List<byte>();
            byte[] buf = new byte[BufferSize];

            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, 
                    SocketType.Stream, 
                    ProtocolType.Tcp);
                
                try
                {
                    IAsyncResult result = socket.BeginConnect(_serverAddress.Address, _serverAddress.Port, null, null);

                    result.AsyncWaitHandle.WaitOne(_timeoutMiliseconds, true);

                    if (!socket.Connected)
                    {
                        socket.Close();
                        throw new Commons.TimeoutException();
                    }


                    Debug.WriteLine("Socket connected to " + _serverAddress.ToString());

                    socket.Send(data);

                    int bytesRec;
                    MemoryStream memory = new MemoryStream(BufferSize);

                    while ((bytesRec = socket.Receive(buf)) > 0)
                    {
                        memory.Write(buf, 0, bytesRec);
                        Debug.WriteLine("Capacity: " + memory.Capacity + " Length: " + memory.Length);
                    }

                    socket.Shutdown(SocketShutdown.Both);   //both or receive?
                    socket.Close();
                    buf = memory.ToArray();
                }
                catch (SocketException e)
                {
                    switch (e.ErrorCode)
                    {
                        case 10060: //timeout
                            throw new Commons.TimeoutException(_serverAddress.ToString(), e);
                        default:
                            throw e;
                    }
                    throw;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return buf;
        }
    }
}
