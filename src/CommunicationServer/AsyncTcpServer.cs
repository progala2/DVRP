using System;
using System.Diagnostics;
using System.IO;
using _15pl04.Ucc.CommunicationServer.Messaging;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace _15pl04.Ucc.CommunicationServer
{
    internal class AsyncTcpServer
    {
        private const int MaxPendingConnections = 100;
        private readonly ServerConfig _config;
        private readonly MessageProcessor _queue;
        private readonly ManualResetEvent _allDoneEvent;
        private Socket _listenerSocket;
        private bool _isListening;


        public delegate void ResponseCallback(byte[] response);

        public AsyncTcpServer(ServerConfig config, MessageProcessor queue)
        {
            _config = config;
            _queue = queue;
            _allDoneEvent = new ManualResetEvent(false);
            _isListening = false;
        }

        public void StartListening()
        {
            Console.WriteLine("Listening for incoming connections...");

            _isListening = true;
            _listenerSocket = new Socket(_config.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listenerSocket.Bind(_config.Address);
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

        private static ResponseCallback GenerateResponseCallback(Socket clientSocket)
        {
            return new ResponseCallback((byte[] response) =>
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

            var state = new StateObject {WorkSocket = handlerSocket};

            handlerSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handlerSocket = state.WorkSocket;


            var bytesRead = handlerSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                state.MemoryStream.Write(state.Buffer, 0, bytesRead);
               
                handlerSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
            else
            {
                Debug.WriteLine("ReadCallback memoryStream size whol: " + state.MemoryStream.Length);

                //state.WorkSocket.Send(state.MemoryStream.ToArray());
                //enqueue rawdata
                _queue.EnqeueInputMessage(state.MemoryStream.ToArray(),
                    GenerateResponseCallback(state.WorkSocket));

                state.Dispose();
            }
        }

        internal class StateObject : IDisposable
        {
            // Client  socket.
            public Socket WorkSocket = null;
            // Size of receive buffer.
#if DEBUG
            public const int BufferSize = 8;
#else
        public const int BufferSize = 1024;
#endif
            // Receive buffer.
            public byte[] Buffer = new byte[BufferSize];
            // Received data string.
            public MemoryStream MemoryStream = new MemoryStream();

            public void Dispose()
            {
                MemoryStream.Dispose();
            }
        }
    }
}
