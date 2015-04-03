using _15pl04.Ucc.Commons.Messaging.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace _15pl04.Ucc.Commons.Messaging
{
    public class Marshaller : IMarshaller<Message>
    {
        private ISerializer<Message> _serializer;
        private IXmlValidator<MessageClass> _validator;
        

        public Marshaller(ISerializer<Message> serializer, IXmlValidator<MessageClass> validator)
        {
            _serializer = serializer;
            _validator = validator;
        }

        public Message[] Unmarshall(byte[] rawData)
        {
            if (rawData == null)
                throw new ArgumentNullException();

            var outputMessages = new List<Message>();

            var separatorIndices = new List<int>();
            for (int i = 0; i < rawData.Length; i++)
                if (rawData[i] == 23)
                    separatorIndices.Add(i);
            separatorIndices.Add(rawData.Length);

            for (int i = 0, begin = 0; i < separatorIndices.Count; i++)
            {
                // Validation - might not be necessary. 
                string xmlString = Encoding.UTF8.GetString(rawData, begin, separatorIndices[i] - begin);
                XDocument xmlDoc = XDocument.Parse(xmlString);
                MessageClass msgClass = Message.GetMessageClassFromString(xmlDoc.Root.ToString());
                _validator.Validate(msgClass, xmlDoc);

                // Deserialization
                Message message = _serializer.Deserialize(rawData, begin, separatorIndices[i] - begin);
                outputMessages.Add(message);

                begin = separatorIndices[i] + 1;
            }

            return outputMessages.ToArray();
        }

        public byte[] Marshall(Message[] messages)
        {
            if (messages == null)
                throw new ArgumentNullException();

            using (var memStream = new MemoryStream())
            {
                if (messages.Length > 0)
                {
                    byte[] data;
                    MessageClass type;
                    for (int i = 0; i < messages.Length; ++i)
                    {
                        type = messages[i].MessageType;
                        data = _serializer.Serialize(messages[i]);
                        memStream.Write(data, 0, data.Length);

                        if (i != messages.Length - 1)
                            memStream.WriteByte(23);
                    }
                }
                return memStream.ToArray();
            }
        }
    }
}
