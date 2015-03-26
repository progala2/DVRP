using System;
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
            var list = new List<Message>();
            var listBytes = new List<byte[]>();
            for (int i = 0, last = 0; i < data.Length; i++)
            {
                if (data[i] != 23) continue;
                var tmp = new byte[i-last];
                Array.ConstrainedCopy(data, last, tmp, 0, i - last);
                listBytes.Add(tmp);
                last = ++i;
            }
            foreach (var b in listBytes)
            {
                var str = Encoding.UTF8.GetString(b);
                var doc = new XmlDocument();
                doc.LoadXml(str);
                if (doc.DocumentElement == null)
                    throw new Exception("Invalid Message Data");
                var type = Message.GetMessageClassTypeFromString(doc.DocumentElement.Name);

                if (!MessageValidator.Validate(str, type))
                    throw new Exception("Invalid " + doc.DocumentElement.Name + " Message");
                list.Add(MessageSerializer.Deserialize(b, type));
            }
            
            return list.ToArray();
        }

        public byte[] Marshall(Message[] messages)
        {
            var list = new MemoryStream();
            foreach (var message in messages)
            {
                byte[] data;
                var type = message.MessageType;
                MessageSerializer.Serialize(message, type, out data);
                list.Write(data, 0, data.Length);
                list.Write(new byte[] { 23 }, 0, 1);
            }
            return list.ToArray();
        }
    }
}
