using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class TcpListener
    {
        public delegate void DataCallback(byte[] data);

        private static TcpListener() { }
        private static TcpListener _instance = new TcpListener();

        public static TcpListener Instance
        {
            get { return _instance; }
        }

        private TcpListener()
        {
            /*
             * Cała inicjalizacja która musi wystąpić oprócz rzeczy rozpoczynających nasłuchiwanie i ustawienia portu.
             */
        }

        public void StartListening(int port, DataCallback callback)
        {
            /*
             * Asynchroniczne przyjmowanie połączeń wywołujące ConnectionEstablishedCallback().
             */ 
        }

        public void StopListening()
        {
            /*
             * Sprzątanie.
             */ 
        }

        private void ConnectionEstablishedCallback(IAsyncResult ar)
        {
            /*
             * Przeczytanie *wszystkiego* co jest w buforze klienta oraz puszczenie tego dalej w obieg callbackiem podanym w StartListening()
             */
        }
    }
}
