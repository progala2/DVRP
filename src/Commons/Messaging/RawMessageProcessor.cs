using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Messaging
{
    public class RawMessageProcessor : IUnmarshaller<Message>
    {
        public RawMessageProcessor() { }

        public Message[] Unmarshall(byte[] data)
        {
            /* Funkcja przyjmuje surówkę, wypluwa poprawne wiadomości.
             * 
             *  W tej klasie ma się być:
             *     - parsowanie
             *     - serializacja
             *     - walidacja
             *     - et cetera
             *  wszystko w oddzielnych metodach
             */

            throw new System.NotImplementedException();
        }
    }
}
