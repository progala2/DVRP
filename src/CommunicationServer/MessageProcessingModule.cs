using System;

namespace _15pl04.Ucc.CommunicationServer
{
    class MessageProcessingModule
    {
        public void ProcessMessage(Byte[] message)
        {
            /* "If messages are sent during a
single connection they are separated with the sign with decimal code equal 23 (ETB - End transmission
blocks)." */
            // check if there are many messages, parse them individually to string
            // foreach invoke ProcessMessage(String)
        }

        private void ProcessMessage(string message)
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
    }
}
