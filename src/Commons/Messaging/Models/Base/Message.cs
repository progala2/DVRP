using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    public abstract class Message
    {
        private static Dictionary<string, MessageClass> _messageClassDictionary;

        [XmlIgnore]
        public abstract MessageClass MessageType { get; }

        static Message()
        {
            int capacity = Enum.GetValues(typeof(MessageClass)).Length;
            _messageClassDictionary = new Dictionary<string, MessageClass>(capacity);

            foreach (MessageClass type in Enum.GetValues(typeof(MessageClass)).Cast<MessageClass>())
                _messageClassDictionary.Add(type.ToString(), type);
        }

        /// <summary>
        /// Gets content of corresponding .xsd file.
        /// </summary>
        /// <param name="type">Message class type.</param>
        /// <returns>Content of corresponding .xsd file.</returns>
        /// <remarks>
        /// This method assumes that each non-abstract derived type:
        ///  - has name like "NameMessage",
        ///  - has corresponding "Name.xsd" file in Resources folder.
        ///  Because of this restriction it is an internal method.
        ///  </remarks>
        internal static string GetXsdFileContent(Type type)
        {
            var className = type.Name;
            var resourceFileName = className.Remove(className.Length - "Message".Length) + ".xsd";
            var resourceContent = Resources.GetResourceContent(resourceFileName);
            return resourceContent;
        }

        public static MessageClass GetMessageClassFromString(string str)
        {
            return _messageClassDictionary[str];
        }

        public override string ToString()
        {
            return "[" + MessageType.ToString() + "] ";
        }
    }
}
