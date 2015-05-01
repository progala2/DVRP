using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models.Base
{
    public abstract class Message
    {
        private static readonly Dictionary<string, MessageClass> _messageTypes;

        static Message()
        {
            var capacity = Enum.GetValues(typeof (MessageClass)).Length;
            _messageTypes = new Dictionary<string, MessageClass>(capacity);

            foreach (var type in Enum.GetValues(typeof (MessageClass)).Cast<MessageClass>())
                _messageTypes.Add(type.ToString(), type);
        }

        [XmlIgnore]
        public abstract MessageClass MessageType { get; }

        public static MessageClass GetMessageClassFromString(string str)
        {
            return _messageTypes[str];
        }

        public override string ToString()
        {
            return "[" + MessageType + "]";
        }
    }
}