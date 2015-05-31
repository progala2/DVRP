using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models.Base
{
    /// <summary>
    /// Base abstract class for all messages used in the system.
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Dictionary that allows mapping from string to MessageClass enum value.
        /// </summary>
        private static readonly Dictionary<string, MessageClass> MessageTypes;

        static Message()
        {
            var capacity = Enum.GetValues(typeof (MessageClass)).Length;
            MessageTypes = new Dictionary<string, MessageClass>(capacity);

            foreach (var type in Enum.GetValues(typeof (MessageClass)).Cast<MessageClass>())
                MessageTypes.Add(type.ToString(), type);
        }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public abstract MessageClass MessageType { get; }

        /// <summary>
        /// Casts strings to MessageClass values.
        /// </summary>
        /// <param name="str">String to cast.</param>
        /// <returns>Corresponding MessageClass enum value.</returns>
        public static MessageClass GetMessageClassFromString(string str)
        {
            return MessageTypes[str];
        }

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            return "[" + MessageType + "]";
        }
    }
}