using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Messaging
{
    interface IMessageSerializer
    {
        Message Deserialize(byte[] buffer);

        void Serialize(Message obj, out byte[] buffer);
    }
    class MessageSerializerHelper<T> : IMessageSerializer
         where T : Message
    {
        private readonly Type _type = typeof (T);
        public Message Deserialize(byte[] buffer)
        {
            using (var reader = new MemoryStream(buffer))
            {
                var xml = new XmlSerializer(_type);
                var instance = (T)xml.Deserialize(reader);
                return instance;
            }
        }

        public void Serialize(Message obj, out byte[] buffer)
        {
            using (var writer = new MemoryStream())
            {
                var xml = new XmlSerializer(_type);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xml.Serialize(writer, obj, ns);
                buffer = writer.ToArray();
            }
        }
    }

    public static class MessageSerializer
    {
        static readonly Dictionary<Message.MessageClassType, IMessageSerializer> _messageSerializerForMessageTypeDictionary;

        static MessageSerializer()
        {
            _messageSerializerForMessageTypeDictionary = new Dictionary<Message.MessageClassType, IMessageSerializer>
                {
                    {Message.MessageClassType.NoOperation, new MessageSerializerHelper<NoOperationMessage>()},
                    {Message.MessageClassType.DivideProblem, new MessageSerializerHelper<DivideProblemMessage>()},
                    {Message.MessageClassType.Error, new MessageSerializerHelper<ErrorMessage>()},
                    {Message.MessageClassType.PartialProblems, new MessageSerializerHelper<PartialProblemsMessage>()},
                    {Message.MessageClassType.Register, new MessageSerializerHelper<RegisterMessage>()},
                    {Message.MessageClassType.RegisterResponse, new MessageSerializerHelper<RegisterResponseMessage>()},
                    {Message.MessageClassType.SolutionRequest, new MessageSerializerHelper<SolutionRequestMessage>()},
                    {Message.MessageClassType.Solutions, new MessageSerializerHelper<SolutionsMessage>()},
                    {Message.MessageClassType.SolveRequest, new MessageSerializerHelper<SolveRequestMessage>()},
                    {Message.MessageClassType.SolveRequestResponse, new MessageSerializerHelper<SolveRequestResponseMessage>()},
                    {Message.MessageClassType.Status, new MessageSerializerHelper<StatusMessage>()}
                };
        }
        static IMessageSerializer GetSerializerForMessageClassType(Message.MessageClassType type)
        {
            return _messageSerializerForMessageTypeDictionary[type];
        }

        public static Message Deserialize(byte[] buffer, Message.MessageClassType type)
        {
            return GetSerializerForMessageClassType(type).Deserialize(buffer);
        }

        public static void Serialize(Message obj, Message.MessageClassType type, out byte[] buffer)
        {
            GetSerializerForMessageClassType(type).Serialize(obj, out buffer);
        }
    }
}
