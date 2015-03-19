using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Messaging
{
    public class Marshaller
    {
        public Marshaller() { }

        public Message[] Unmarshall(byte[] data)
        {
            /* Funkcja przyjmuje surówkę, wypluwa poprawne wiadomości.
             * 
             *  W tej klasie ma się być:
             *     - parsowanie
             *     - deserializacja
             *     - walidacja
             *     - et cetera
             *  wszystko w oddzielnych metodach i klasach (można zrobić klasy XmlValidator/XmlParser etc.)
             */

            throw new System.NotImplementedException();
        }

        public byte[] Marshall(Message[] data)
        {
            /* 
             * To samo, tylko że w drugą stronę.
             */

            throw new System.NotImplementedException();
        }
    }
}
