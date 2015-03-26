using System;
using System.IO;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Tests.Tools
{
    static public class XmlFileParser<T>
    {
        private static readonly Type Type = typeof(T);

        public static T Deserialize(string path)
        {
            using (var reader = new FileStream(path, FileMode.Open))
            {
                var xml = new XmlSerializer(Type);
                var instance = (T)xml.Deserialize(reader);
                return instance;
            }
        }
    }
}
