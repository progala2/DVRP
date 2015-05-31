using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging.Marshalling
{
    /// <summary>
    /// Class converting raw byte data to messages and inverse.
    /// </summary>
    public class Marshaller : IMarshaller<Message>
    {
        private readonly ISerializer<Message> _serializer;
        private readonly IXmlValidator<MessageClass> _validator;

        /// <summary>
        /// Creates Marshaller.
        /// </summary>
        /// <param name="serializer">Serializer used for messages.</param>
        /// <param name="validator">Validator used for messages.</param>
        public Marshaller(ISerializer<Message> serializer, IXmlValidator<MessageClass> validator)
        {
            _serializer = serializer;
            _validator = validator;
        }

        /// <summary>
        /// Convert raw bytes of data to messages.
        /// </summary>
        /// <param name="rawData">Messages data to convert.</param>
        /// <returns>Messages returned</returns>
        /// <exception cref="ArgumentNullException">Throws when argument rawData is null.</exception>
        public List<Message> Unmarshall(byte[] rawData)
        {
            if (rawData == null)
                throw new ArgumentNullException();

            var outputMessages = new List<Message>();

            var separatorIndices = new List<int>();
            for (var i = 0; i < rawData.Length; i++)
                if (rawData[i] == 23)
                    separatorIndices.Add(i);
            separatorIndices.Add(rawData.Length);

            for (int i = 0, begin = 0; i < separatorIndices.Count; i++)
            {
                // Validation - might not be necessary. 
                var xmlString = Encoding.UTF8.GetString(rawData, begin, separatorIndices[i] - begin);

                if (xmlString == string.Empty) // TODO fix & remove
                    break;

                var xmlDoc = XDocument.Parse(xmlString);
                var msgClass = Message.GetMessageClassFromString(xmlDoc.Root.Name.LocalName);
                _validator.Validate(msgClass, xmlDoc);

                // Deserialization
                var message = _serializer.Deserialize(rawData, begin, separatorIndices[i] - begin);
                outputMessages.Add(message);

                begin = separatorIndices[i] + 1;
            }

            return outputMessages;
        }

        /// <summary>
        /// Convert messages to raw bytes
        /// </summary>
        /// <param name="messages">List of messages to convert</param>
        /// <returns>Raw bytes after conversion</returns>
        /// <exception cref="ArgumentNullException">Throws when argument messages is null.</exception>
        public byte[] Marshall(IList<Message> messages)
        {
            if (messages == null)
                throw new ArgumentNullException();

            using (var memStream = new MemoryStream())
            {
                if (messages.Count > 0)
                {
                    MessageClass type;
                    for (var i = 0; i < messages.Count; ++i)
                    {
                        type = messages[i].MessageType;
                        var data = _serializer.Serialize(messages[i]);
                        memStream.Write(data, 0, data.Length);

                        if (i != messages.Count - 1)
                            memStream.WriteByte(23);
                    }
                }
                return memStream.ToArray();
            }
        }
    }
}