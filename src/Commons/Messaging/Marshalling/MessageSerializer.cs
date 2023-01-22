using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Exceptions;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging.Marshalling
{
    /// <summary>
    /// Class used for (de)serializing messages
    /// </summary>
    public class MessageSerializer : ISerializer<Message>
    {
        private readonly Dictionary<MessageClass, XmlSerializer> _serializers;
        
        /// <summary>
        /// Create message serializer.
        /// </summary>
        public MessageSerializer()
        {
            var capacity = Enum.GetValues(typeof (MessageClass)).Length;
            _serializers = new Dictionary<MessageClass, XmlSerializer>(capacity);

            foreach (var type in Enum.GetValues(typeof (MessageClass)).Cast<MessageClass>())
                _serializers.Add(type, new XmlSerializer(type.GetMessageType()));
        }

        /// <summary>
        /// Convert bytes to message
        /// </summary>
        /// <param name="buffer">Input bytes to convert</param>
        /// <returns>Message after conversion.</returns>
        public Message Deserialize(byte[] buffer)
        {
            return Deserialize(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Convert bytes part of bytes to message.
        /// </summary>
        /// <param name="buffer">Array of bytes to convert.</param>
        /// <param name="index">Index where to begin reading bytes to convert.</param>
        /// <param name="count">Number of bytes to convert.</param>
        /// <returns>Message after conversion.</returns>
        public Message Deserialize(byte[] buffer, int index, int count)
        {
            var type = GetMessageType(buffer, index, count);

            using var stream = new MemoryStream(buffer, index, count);
            var serializer = _serializers[type];
            return (Message?) serializer.Deserialize(stream) ?? throw new ParsingNullException(nameof(buffer));
        }

        /// <summary>
        /// Convert message to array of bytes.
        /// </summary>
        /// <param name="obj">Message to turn into bytes.</param>
        /// <returns>Serialized message as raw bytes.</returns>
        public byte[] Serialize(Message obj)
        {
	        using var memStream = new MemoryStream();
	        var serializer = _serializers[obj.MessageType];

	        var namespaces = new XmlSerializerNamespaces();
	        namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

	        serializer.Serialize(memStream, obj, namespaces);
	        return memStream.ToArray();
        }

        /// <summary>
        /// Get message type from bytes.
        /// </summary>
        /// <param name="buffer">Array of bytes to read message type from.</param>
        /// <param name="index">Index where to start reading message.</param>
        /// <param name="count">Length of message in array.</param>
        /// <returns>Enumerable message class.</returns>
        /// <exception cref="Exception">Throws when no root element is found.</exception>
        private static MessageClass GetMessageType(byte[] buffer, int index, int count)
        {
	        using var memStream = new MemoryStream(buffer, index, count);
	        using var reader = XmlReader.Create(memStream);
	        while (reader.Read())
		        if (reader.NodeType == XmlNodeType.Element)
			        return Message.GetMessageClassFromString(reader.Name);

	        throw new Exception("No root element found.");
        }
    }
}