using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging
{
    public class XMLParser<T>
    {
        public Type Type { get; set; }

        public XMLParser()
        {
            Type = typeof(T);
        }

        public T Deserialise(byte[] buffer)
        {
            using (MemoryStream reader = new MemoryStream(buffer))
            {
                T instance;
                XmlSerializer xml = new XmlSerializer(Type);
                instance = (T)xml.Deserialize(reader);
                return instance;
            }
        }

        public void Serialise(object obj, out byte[] buffer)
        {
            using (MemoryStream writer = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(Type);
                xml.Serialize(writer, obj);
                buffer = writer.GetBuffer();
            }
        }
    }
}
