using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Dvrp.Ucc.Commons.Logging;
using Dvrp.Ucc.CommunicationServer.Messaging.Base;

namespace Dvrp.Ucc.CommunicationServer.Messaging
{
    /// <summary>
    /// Accepts TCP clients, schedules incoming data processing and passes response.
    /// </summary>
    internal class TcpServer
    {
        /// <summary>
        /// Delegate invoked with a response in order to send it back to the client.
        /// </summary>
        /// <param name="response">Response byte data.</param>
        public delegate void ResponseCallback(byte[] response);

        private const int MaxPendingConnections = 100;
        private const int ReadBufferSize = 4096;
        private static readonly ILogger Logger = new ConsoleLogger();
        private readonly IDataProcessor _dataProcessor;
        private readonly Socket _listenerSocket;
        private CancellationTokenSource? _cancellationTokenSource;
        private ManualResetEvent? _clientAcceptanceEvent;
        private volatile bool _isListening;

        /// <summary>
        /// Creates TcpServer instance.
        /// </summary>
        /// <param name="address">Listener address.</param>
        /// <param name="processor">Incoming data processing module.</param>
        public TcpServer(IPEndPoint address, IDataProcessor processor)
        {
            _dataProcessor = processor;

            _listenerSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenerSocket.Bind(address);
        }

        /// <summary>
        /// Starts listening, accepting and reading from the clients.
        /// </summary>
        public void StartListening()
        {
            if (_isListening)
                return;

            Logger.Info("TcpServer is starting...");

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
			            _listenerSocket.BeginAccept(AcceptCallback, _listenerSocket);
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

        /// <summary>
        /// Stops listening for and accepting TCP clients.
        /// </summary>
        public void StopListening()
        {
            if (!_isListening)
                return;

            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Generates a callback that sends response and finalizes the connection.
        /// </summary>
        /// <param name="clientSocket">Client socket to handle.</param>
        /// <returns>Generated callback.</returns>
        private static ProcessedDataCallback GenerateResponseCallback(Socket clientSocket)
        {
            return response =>
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

                    Logger.Warn("Client socket was closed before response message could be sent.");
                }
            };
        }

        /// <summary>
        /// Invoked on TCP client acceptance; reads data from its buffer.
        /// </summary>
        /// <param name="ar">Data about the socket.</param>
        private void AcceptCallback(IAsyncResult ar)
        {
            _clientAcceptanceEvent?.Set();

            if (!_isListening)
                return;

            var listenerSocket = (Socket?)ar.AsyncState;
            var clientSocket = listenerSocket?.EndAccept(ar) ?? throw new Exception("No Socket!");

            Logger.Trace("Client accepted.");

            var buffer = new byte[ReadBufferSize];

            using var memStream = new MemoryStream();
            int bytesRead;
            do
            {
	            bytesRead = clientSocket.Receive(buffer);
	            memStream.Write(buffer, 0, bytesRead);
            } while (bytesRead > 0);

            var metadata = new TcpDataProviderMetadata((IPEndPoint?)clientSocket.RemoteEndPoint ?? throw new Exception("No RemoteEndPoint!"))
            {
	            ReceptionTime = DateTime.UtcNow
            };

            _dataProcessor.EnqueueDataToProcess(
	            memStream.ToArray(),
	            metadata,
	            GenerateResponseCallback(clientSocket));

            Logger.Trace("Client sent " + memStream.Length + " bytes of data.");
        }
    }
}