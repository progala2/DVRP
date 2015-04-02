using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.Commons.Messaging.Base;

namespace _15pl04.Ucc.Commons.Messaging
{
    class MessageSerializerHelper<T> : ISerializer<Message>
         where T : Message
    {
        private readonly Type _type = typeof(T);
        public Message Deserialize(byte[] buffer)
        {
            using (var reader = new MemoryStream(buffer))
            {
                var xml = new XmlSerializer(_type);
                var instance = (T)xml.Deserialize(reader);
                return instance;
            }
        }

        public Message Deserialize(byte[] buffer, int index, int count)
        {
            using (var reader = new MemoryStream(buffer, index, count))
            {
                var xml = new XmlSerializer(_type);
                var instance = (T)xml.Deserialize(reader);
                return instance;
            }
        }

        public byte[] Serialize(Message obj)
        {
            using (var writer = new MemoryStream())
            {
                var xml = new XmlSerializer(_type);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xml.Serialize(writer, obj, ns);

                return writer.ToArray();
            }
        }
    }

    public static class MessageSerializer
    {
        static readonly Dictionary<MessageClass, ISerializer<Message>> _messageSerializerForMessageTypeDictionary;

        static MessageSerializer()
        {
            _messageSerializerForMessageTypeDictionary = new Dictionary<MessageClass, ISerializer<Message>>
                {
                    {MessageClass.NoOperation, new MessageSerializerHelper<NoOperationMessage>()},
                    {MessageClass.DivideProblem, new MessageSerializerHelper<DivideProblemMessage>()},
                    {MessageClass.Error, new MessageSerializerHelper<ErrorMessage>()},
                    {MessageClass.SolvePartialProblems, new MessageSerializerHelper<PartialProblemsMessage>()},
                    {MessageClass.Register, new MessageSerializerHelper<RegisterMessage>()},
                    {MessageClass.RegisterResponse, new MessageSerializerHelper<RegisterResponseMessage>()},
                    {MessageClass.SolutionRequest, new MessageSerializerHelper<SolutionRequestMessage>()},
                    {MessageClass.Solutions, new MessageSerializerHelper<SolutionsMessage>()},
                    {MessageClass.SolveRequest, new MessageSerializerHelper<SolveRequestMessage>()},
                    {MessageClass.SolveRequestResponse, new MessageSerializerHelper<SolveRequestResponseMessage>()},
                    {MessageClass.Status, new MessageSerializerHelper<StatusMessage>()}
                };
        }
        static ISerializer<Message> GetSerializerForMessageClassType(MessageClass type)
        {
            return _messageSerializerForMessageTypeDictionary[type];
        }

        public static Message Deserialize(byte[] buffer, MessageClass type)
        {
            return GetSerializerForMessageClassType(type).Deserialize(buffer);
        }

        public static Message Deserialize(byte[] buffer, int index, int count, MessageClass type)
        {
            return GetSerializerForMessageClassType(type).Deserialize(buffer, index, count);
        }

        public static void Serialize(Message obj, MessageClass type, out byte[] buffer)
        {
            buffer = GetSerializerForMessageClassType(type).Serialize(obj);
        }
    }
}
