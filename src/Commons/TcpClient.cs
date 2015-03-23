using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace _15pl04.Ucc.Commons
{
    public class TcpClient
    {
        private readonly IPEndPoint _serverAddress;
#if DEBUG
        private const int BufferSize = 8;
#else
        private const int BufferSize = 1024;
#endif
        public TcpClient(IPEndPoint serverAddress)
        {
            _serverAddress = serverAddress;
        }

        public byte[] SendData(byte[] data)
        {
            List<byte> ret = new List<byte>();

            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    byte[] buf = new byte[BufferSize];
                    socket.Connect(_serverAddress);

                    Debug.WriteLine("Socket connected to " + _serverAddress.ToString());

                    socket.Send(data);

                    int bytesRec;
                    while ((bytesRec = socket.Receive(buf)) > 0)
                    {
                        ret.AddRange(buf.Take(bytesRec));
                        Debug.WriteLine(System.Text.Encoding.ASCII.GetString(ret.ToArray()));
                    }

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    buf = ret.ToArray();
                }
                catch (SocketException e)
                {
                    switch (e.ErrorCode)
                    {
                        case 10060:
                            throw new Commons.Exceptions.TimeoutException(_serverAddress.ToString(), e);
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
            return ret.ToArray(); ;
        }
    }
}
