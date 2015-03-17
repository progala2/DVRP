using System;

namespace _15pl04.Ucc.CommunicationServer.Messaging
{
    internal class MessageProcessor : IDataProcessor
    {
        public MessageProcessor() { }

        public void ProcessByteData(byte[] data)
        {
            /* "If messages are sent during a
            single connection they are separated with the sign with decimal code equal 23 (ETB - End transmission
                blocks)." */
            // check if there are many messages, parse them individually to string
            // foreach invoke ProcessMessage(String)
        }

        private void CastToString(string message)
        {
            // parse to xml
            // check with proper *.xsd file (based on root element)
            // deserialize

            // deal with it...
            /*
             * 
             * 
             * 
             * 
             * 
             */
        }

        public byte[] ProcessData(byte[] data)
        {


            throw new NotImplementedException();
        }
    }
}
