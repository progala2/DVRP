using _15pl04.Ucc.CommunicationServer.Messaging;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class TcpServer
    {
        public delegate void ResponseCallback(byte[] response);


        private const int MaxPendingConnections = 100;
        private const int ReadBufferSize = 4096; // TODO make sure it's enough

        private IDataProcessor _dataProcessor;
        private Socket _listenerSocket;
        private EndPoint _address;

        private ManualResetEvent _clientAcceptanceEvent = new ManualResetEvent(false);
        private volatile bool _isListening = false;


        public TcpServer(IPEndPoint address, IDataProcessor processor)
        {
            _dataProcessor = processor;
            _address = address;

            _listenerSocket = new Socket(_address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListening()
        {
            if (_isListening)
                return;

            _isListening = true;

            try
            {
                _listenerSocket.Bind(_address);
                _listenerSocket.Listen(MaxPendingConnections);

                while (_isListening)
                {
                    _clientAcceptanceEvent.Reset();
                    _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket);
                    _clientAcceptanceEvent.WaitOne();
                }
            }
            catch (ObjectDisposedException e)
            {
                // Most likely ManualResetEvent got disposed due to StopListening() method invocation.

                // *crickets*
            }
            catch (Exception)
            {
                // TODO log an error
                throw;
            }
        }

        public void StopListening()
        {
            if (!_isListening)
                return;

            _isListening = false;

            _clientAcceptanceEvent.Close();
            _listenerSocket.Close();
        }

        private static ProcessedDataCallback GenerateResponseCallback(Socket clientSocket)
        {
            return new ProcessedDataCallback((byte[] response) =>
            {
                // A situation can occur where client socket has been closed before message was sent. 
                // In that case the method does nothing.
                try
                {
                    clientSocket.Send(response);
                    clientSocket.Shutdown(SocketShutdown.Send);
                    clientSocket.Close();
                }
                catch (SocketException)
                {
                    // *crickets*

                    // TODO log a warning
                }
            });
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _clientAcceptanceEvent.Set();

            if (!_isListening)
                return;

            Socket listenerSocket = (Socket)ar.AsyncState;
            Socket clientSocket = listenerSocket.EndAccept(ar);

            byte[] buffer = new byte[ReadBufferSize];

            using (var memStream = new MemoryStream())
            {
                int bytesRead;
                do
                {
                    bytesRead = clientSocket.Receive(buffer);
                    memStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                var metadata = new TcpDataProviderMetadata()
                {
                    ReceptionTime = DateTime.UtcNow,
                    SenderAddress = (IPEndPoint)clientSocket.RemoteEndPoint,
                };

                _dataProcessor.EnqueueDataToProcess(
                    memStream.ToArray(),
                    metadata,
                    GenerateResponseCallback(clientSocket));
            }
        }
    }
}
