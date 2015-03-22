using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace _15pl04.Ucc.Commons
{
    public class TcpClient
    {
        private readonly IPEndPoint _serverAddress;
        private const int BufferSize = 1024;
        public TcpClient(IPEndPoint serverAddress)
        {
            _serverAddress = serverAddress;
        }

        public byte[] SendData(byte[] data)
        {
             /* Wszystko synchronicznie, a więc możliwe, że z blokowaniem.
             * Wziąć pod uwagę backup serwery w przypadku braku odpowiedzi głównego CS.
             */
            byte[] bytes = new byte[BufferSize];

            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(_serverAddress);

                    Debug.WriteLine("Socket connected to " + _serverAddress.ToString());

                    socket.Send(data);

                    int bytesRec = socket.Receive(bytes);   //TODO increase buffer in case of bigger data

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    bytes = bytes.Take(bytesRec).ToArray();
                }
                catch (SocketException e)
                {
                    switch (e.ErrorCode)
                    {
                        case 10060: //Timed out
                        //TODO connect to backup server
                            break;
                        default:
                            return null;
                    }
                    throw;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return bytes;
        }
    }
}
