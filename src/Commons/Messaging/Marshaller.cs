using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Messaging
{
    public class Marshaller
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
                var type = Message.GetMessageClassTypeFromString(doc.DocumentElement.Name);

                MessageValidator.Validate(str, type);
                var message = MessageSerializer.Deserialize(data, begin, separatorsPositions[i] - begin, type);
                listOfMessages.Add(message);

                begin = separatorsPositions[i] + 1;
            }

            return listOfMessages.ToArray();
        }

        public byte[] Marshall(Message[] messages)
        {
            using (var list = new MemoryStream())
            {
                if (messages != null && messages.Length > 0)
                {
                    byte[] data;
                    Message.MessageClassType type;
                    for (int i = 0; i < messages.Length - 1; i++)
                    {
                        type = messages[i].MessageType;
                        MessageSerializer.Serialize(messages[i], type, out data);
                        list.Write(data, 0, data.Length);
                        list.WriteByte(23);
                    }
                    type = messages[messages.Length - 1].MessageType;
                    MessageSerializer.Serialize(messages[messages.Length - 1], type, out data);
                    list.Write(data, 0, data.Length);
                }
                return list.ToArray();
            }
        }
    }
}
