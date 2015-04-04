using _15pl04.Ucc.Commons.Messaging.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging
{
    public class MessageSerializer : ISerializer<Message>
    {
        private Dictionary<MessageClass, XmlSerializer> _serializers;

        public MessageSerializer()
        {
            int capacity = Enum.GetValues(typeof(MessageClass)).Length;
            _serializers = new Dictionary<MessageClass, XmlSerializer>(capacity);

            foreach (MessageClass type in Enum.GetValues(typeof(MessageClass)).Cast<MessageClass>())
                _serializers.Add(type, new XmlSerializer(type.GetMessageType()));
        }

        public Message Deserialize(byte[] buffer)
        {
            return Deserialize(buffer, 0, buffer.Length);
        }

        public Message Deserialize(byte[] buffer, int index, int count)
        {
            var type = GetMessageType(buffer, index, count);

            using (var stream = new MemoryStream(buffer, index, count))
            {
                XmlSerializer serializer = _serializers[type];
                return (Message)serializer.Deserialize(stream);
            }
        }

        public byte[] Serialize(Message obj)
        {
            using (var memStream = new MemoryStream())
            {
                XmlSerializer serializer = _serializers[obj.MessageType];

                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                serializer.Serialize(memStream, obj, namespaces);
                return memStream.ToArray();
            }
        }

        private MessageClass GetMessageType(byte[] buffer, int index, int count)
        {
            using (var memStream = new MemoryStream(buffer, index, count))
            {
                using (var reader = XmlReader.Create(memStream))
                {
                    while (reader.Read())
                        if (reader.NodeType == XmlNodeType.Element)
                            return Message.GetMessageClassFromString(reader.Name);

                    throw new Exception("No root element found.");
                }
            }
        }
    }
}
