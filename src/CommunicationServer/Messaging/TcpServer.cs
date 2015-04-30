using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.CommunicationServer.Messaging;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class TcpServer
    {
        // TODO interface/abstract class

        public delegate void ResponseCallback(byte[] response);

        private const int MaxPendingConnections = 100;
        private const int ReadBufferSize = 4096; // TODO make sure it's enough


        private static ILogger _logger = new TraceSourceLogger(typeof(TcpServer).Name);

        private IDataProcessor _dataProcessor;
        private Socket _listenerSocket;

        private ManualResetEvent _clientAcceptanceEvent;
        private CancellationTokenSource _cancellationTokenSource;
        private volatile bool _isListening = false;


        public TcpServer(IPEndPoint address, IDataProcessor processor)
        {
            _dataProcessor = processor;

            _listenerSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenerSocket.Bind(address);
        }

        public void StartListening()
        {
            if (_isListening)
                return;

            _logger.Info("TcpServer is starting...");

            _clientAcceptanceEvent = new ManualResetEvent(false);
            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;
            token.Register(() =>
            {
                _isListening = false;
                _listenerSocket.Close();
                _clientAcceptanceEvent.Close();
            });

            _listenerSocket.Listen(MaxPendingConnections);

            new Task(() =>
            {
                while (true)
                {
                    
                    try
                    {
                        _clientAcceptanceEvent.Reset();
                        _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket);
                        _clientAcceptanceEvent.WaitOne();
                    }
                    catch (Exception)
                    {
                        // *crickets*
                    }
                }
            }, token).Start();

            _isListening = true;
        }

        public void StopListening()
        {
            if (!_isListening)
                return;

            _cancellationTokenSource.Cancel();
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

                    _logger.Warn("Client socket was closed before response message could be sent.");
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

            _logger.Trace("Client accepted.");

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

                _logger.Trace("Client sent " + memStream.Length + " bytes of data.");
            }
        }
    }
}
