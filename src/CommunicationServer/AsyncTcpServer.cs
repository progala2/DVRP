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

        /*
         ******* Wytyczne *********
         *  Klasa ma:
         *      1. Przyjąć klienta i nawiązać z nim połączenie.
         *      2. Przeczytać cały jego bufor.
         *      3. Wygenerować callback z użyciem GenerateResponseCallback().
         *      4. Wrzucić przeczytane dane wraz z callbackiem do InputMessageQueue (podanym w StartListening()).
         *  I to wszystko ma być asynchronicznie, tj. serwer dalej może przyjmować kolejnych klientów nawet, jeżeli jakiś jest obsługiwany.
         *  Dorobić prywatne metody żeby to wszystko jakoś sensownie porozdzielać pod względem funkcjonalności.
         *  
         *  Użyć tego (nawiązywanie połączenia i czytanie):
         *  https://msdn.microsoft.com/en-us/library/fx6588te%28v=vs.110%29.aspx
         *  Nie robić samemu Tasków ani Threadów (oprócz głównej pętli nasłuchującej).
         */

        public AsyncTcpServer(ServerConfig config, MessageProcessor queue)
        {
            /*
             * Inicjalizacja.
             */
            _config = config;
            _queue = queue;
            _allDoneEvent = new ManualResetEvent(false);
            _isListening = false;
        }

        public void StartListening()
        {
            /*
             * Nasłuchiwanie.
             */
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
            /*
             * Sprzątanie.
             */
            //_listenerSocket.Shutdown(SocketShutdown.Both);
            _isListening = false;
            _allDoneEvent.Set();
            _listenerSocket.Close();
        }

        private static ResponseCallback GenerateResponseCallback(Socket clientSocket)
        {
            return new ResponseCallback((byte[] response) =>
            {
                /*
                 *  1. Wysłać response przez clientSocket.
                 *  2. Zamknąć połączanie i posprzątać.
                 *  (synchronicznie)
                 *  
                 *  Nie ważne co się stanie, funkcja ma się kiedyś kończyć. Nie może nam się zawiesić w nieskończoność i marnować zasoby.
                 */

                //needs reworking, currently unable to test it correctly
                clientSocket.Send(response);
                clientSocket.Shutdown(SocketShutdown.Send);
                clientSocket.Close();
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
