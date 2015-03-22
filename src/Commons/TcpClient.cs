using System.Net;

namespace _15pl04.Ucc.Commons
{
    public class TcpClient
    {
        public TcpClient(IPEndPoint serverAddress)
        {
            /*
             * ... 
             */
        }

        public byte[] SendData(byte[] data)
        {
            /*
             * 1. Nawiąż połączanie z serwerem.
             * 2. Wrzuć dane do bufora.
             * 3. Oczekuj odpowiedzi od CS, zwróć ją.
             * 
             * Wszystko synchronicznie, a więc możliwe, że z blokowaniem.
             * Wziąć pod uwagę backup serwery w przypadku braku odpowiedzi głównego CS.
             */

            return null;
        }

    }
}
