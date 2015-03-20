using _15pl04.Ucc.CommunicationServer.Collections;
using System.Net.Sockets;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class AsyncTcpServer
    {
        public delegate void ResponseCallback(byte[] response);

        public static AsyncTcpServer Instance
        {
            get { return _instance; }
        }

        static AsyncTcpServer() { } // Do not delete.
        private static AsyncTcpServer _instance = new AsyncTcpServer();

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

        private AsyncTcpServer()
        {
            /*
             * Początek inicjalizacji (prawdopodobnie tylko ustawienie wielkości bufora).
             */
        }

        public void StartListening(ServerConfig config, InputMessageQueue queue)
        {
            /*
             * Dalsza inicjalizacja & nasłuchiwanie.
             */
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
                 */
            });
        }
    }
}
