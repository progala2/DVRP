using System;
using System.IO;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging
{
    public static class XmlParser<T>
    {
        private static readonly Type _type = typeof (T);
        public static T Deserialize(byte[] buffer)
        {
            using (var reader = new MemoryStream(buffer))
            {
                var xml = new XmlSerializer(_type);
                var instance = (T)xml.Deserialize(reader);
                return instance;
            }
        }

        public static void Serialize(T obj, out byte[] buffer)
        {
            using (var writer = new MemoryStream())
            {
                var xml = new XmlSerializer(_type);
                xml.Serialize(writer, obj);
                buffer = writer.GetBuffer();
            }
        }
    }
}
