using System;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class TcpListener
    {
        public delegate byte[] DataCallback(byte[] data);

        private static TcpListener() { }
        private static TcpListener _instance = new TcpListener();

        public static TcpListener Instance
        {
            get { return _instance; }
        }

        /*
         ******* Wytyczne *********
         *  Klasa ma:
         *      1. Przyjmować klientów i nawiązywać połączenie.
         *      2. Czytać cały bufor którym "przyszli".
         *      3. Wywoływać DataCallback dla przeczytanych danych. 
         *      4. Wysłać do klienta to, co zwróci DataCallback
         *      5. Kończyć połączenie i sprzątać.
         *  I to wszystko ma być asynchronicznie, tj. serwer dalej może przyjmować kolejnych klientów nawet, jeżeli jakiś jest obsługiwany.
         *  Na obsługę jednego klienta powinien wystarczyć jeden wątek.
         *  Można dorobić prywatne metody żeby to wszystko porozdzielać, ale mają mieć ściśle określone, spójne funkcjonalności.
         */

        private TcpListener()
        {
            /*
             * Cała inicjalizacja która musi wystąpić oprócz rzeczy rozpoczynających nasłuchiwanie i ustawienia portu.
             */
        }

        public void StartListening(int port, MessageProcessor callback)
        {
            /*
             * Resztki inicjalizacji. Nasłuchiwanie.
             */ 
        }

        public void StopListening()
        {
            /*
             * Sprzątanie.
             */ 
        }
    }
}
