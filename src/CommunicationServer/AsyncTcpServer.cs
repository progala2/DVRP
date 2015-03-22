using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Messaging;
using System.Net.Sockets;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class AsyncTcpServer
    {
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
        }

        public void StartListening()
        {
            /*
             * Nasłuchiwanie.
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
                 *  (synchronicznie)
                 *  
                 *  Nie ważne co się stanie, funkcja ma się kiedyś kończyć. Nie może nam się zawiesić w nieskończoność i marnować zasoby.
                 */
            });
        }
    }
}
