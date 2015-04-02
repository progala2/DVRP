using _15pl04.Ucc.Commons.Messaging.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace _15pl04.Ucc.Commons.Messaging
{
    public class MessageMarshaller : IMarshaller<Message>
    {
        

        public Message[] Unmarshall(byte[] data)
        {
            var listOfMessages = new List<Message>();

            var separatorsPositions = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 23)
                {
                    separatorsPositions.Add(i);
                }
            }
            separatorsPositions.Add(data.Length);

            for (int i = 0, begin = 0; i < separatorsPositions.Count; i++)
            {
                var str = Encoding.UTF8.GetString(data, begin, separatorsPositions[i] - begin);

                var doc = new XmlDocument();
                doc.LoadXml(str);
                if (doc.DocumentElement == null)
                    throw new Exception("Invalid Message Data");
                var type = Message.GetMessageClassFromString(doc.DocumentElement.Name);

                MessageValidator.Validate(str, type);
                var message = MessageSerializer.Deserialize(data, begin, separatorsPositions[i] - begin, type);
                listOfMessages.Add(message);

                begin = separatorsPositions[i] + 1;
            }

            return listOfMessages.ToArray();
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
                        MessageSerializer.Serialize(messages[i], type, out data);
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
