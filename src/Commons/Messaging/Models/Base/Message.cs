using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    public abstract class Message
    {
        private static Dictionary<string, MessageClass> _messageTypes;

        [XmlIgnore]
        public abstract MessageClass MessageType { get; }


        static Message()
        {
            int capacity = Enum.GetValues(typeof(MessageClass)).Length;
            _messageTypes = new Dictionary<string, MessageClass>(capacity);

            foreach (MessageClass type in Enum.GetValues(typeof(MessageClass)).Cast<MessageClass>())
                _messageTypes.Add(type.ToString(), type);
        }


        public static MessageClass GetMessageClassFromString(string str)
        {
            return _messageTypes[str];
        }

        public override string ToString()
        {
            return "[" + MessageType.ToString() + "]";
        }
    }
}
