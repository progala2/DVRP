using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Messaging;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;


namespace _15pl04.Ucc.CommunicationServer
{
    internal class AsyncTcpServer
    {
        private const int MaxConnectionQueue = 100;
        private readonly ServerConfig _config;
        private readonly MessageProcessor _queue;
        private readonly ManualResetEvent _allDoneEvent;


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
        }

        public void StartListening()
        {
            /*
             * Nasłuchiwanie.
             */
            Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listenerSocket.Bind(_config.Address);
                listenerSocket.Listen(MaxConnectionQueue);

                while (true)
                {
                    _allDoneEvent.Reset();

                    listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), listenerSocket);

                    _allDoneEvent.WaitOne();
                }
            }
            catch (Exception e)
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

            Socket listenerSocket = (Socket)ar.AsyncState;
            Socket handlerSocket = listenerSocket.EndAccept(ar);

            var state = new StateObject();
            state.WorkSocket = handlerSocket;
            handlerSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handlerSocket = state.WorkSocket;

            int bytesRead = 0;


            bytesRead = handlerSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                state.RawDataList.AddRange(state.Buffer.Take(bytesRead));
                Debug.WriteLine("ReadCallback rawDataList size part: " + state.RawDataList.Count);

                handlerSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
            else
            {
                Debug.WriteLine("ReadCallback rawDataList size whol: " + state.RawDataList.Count);

                //state.workSocket.Send(state.RawDataList.ToArray());
                //enqueue rawdata
                _queue.EnqeueInputMessage(state.RawDataList.ToArray(),
                    GenerateResponseCallback(state.WorkSocket));
            }
        }

        internal class StateObject
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
            public List<byte> RawDataList = new List<byte>(BufferSize);
        }
    }
}
