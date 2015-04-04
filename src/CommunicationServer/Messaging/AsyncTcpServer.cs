using System;
using System.Diagnostics;
using System.IO;
using _15pl04.Ucc.CommunicationServer.Messaging;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class AsyncTcpServer
    {
        private IDataProcessor<IPEndPoint> _dataProcessor;
        private EndPoint _address;


        private const int MaxPendingConnections = 100;
        private const int ReadBufferSize = 1024;

        

        private readonly ManualResetEvent _allDoneEvent;
        private Socket _listenerSocket;
        private bool _isListening;


        public delegate void ResponseCallback(byte[] response);

        public AsyncTcpServer(IPEndPoint address, IDataProcessor<IPEndPoint> processor)
        {
            _dataProcessor = processor;
            _address = address;

            _allDoneEvent = new ManualResetEvent(false);
            _isListening = false;
        }

        public void StartListening()
        {
            Console.WriteLine("Listening for incoming connections...");

            _isListening = true;
            _listenerSocket = new Socket(_address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                
                _listenerSocket.Bind(_address);
                _listenerSocket.Listen(MaxPendingConnections);

                while (_isListening)
                {
                    _allDoneEvent.Reset();

                    _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket);

                    _allDoneEvent.WaitOne();
                }
            }
            catch (Exception)
            {
                //TODO
                throw;
            }
        }

        public void StopListening()
        {
            _isListening = false;
            _allDoneEvent.Set();
            _listenerSocket.Close();
        }

        private static ProcessedDataCallback GenerateResponseCallback(Socket clientSocket)
        {
            return new ProcessedDataCallback((byte[] response) =>
            {
                //There can happen a situation where clientSocked is closed before message would be sent. 
                //In such situation function does nothing
                try
                {
                    clientSocket.Send(response);
                    clientSocket.Shutdown(SocketShutdown.Send);
                    clientSocket.Close();
                }
                catch (Exception)
                {
                    //Nothing to do, function doesnt inform that it could not send response
                }
            });
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _allDoneEvent.Set();

            if (!_isListening)
                return;

            Socket listenerSocket = (Socket)ar.AsyncState;
            Socket handlerSocket = listenerSocket.EndAccept(ar);


            byte[] buffer = new byte[ReadBufferSize];


            using (var memStream = new MemoryStream())
            {
                int bytesRead;
                do
                {
                    bytesRead = handlerSocket.Receive(buffer);
                    memStream.Write(buffer, 0, bytesRead);

                } while (bytesRead > 0);
                _dataProcessor.EnqueueDataToProcess(memStream.ToArray(), (IPEndPoint)handlerSocket.RemoteEndPoint, GenerateResponseCallback(handlerSocket));
            }
        }
    }
}
