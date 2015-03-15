using System;

namespace _15pl04.Ucc.Commons.Messaging
{
    

    public class MessageReceiver
    {
        public delegate void MessageReceivedEventHandler(object sender, Message m);

        public event MessageReceivedEventHandler MessageReceived;

        public MessageReceiver()
        {

        }

        public void Start()
        {

        }// 



    }
}
