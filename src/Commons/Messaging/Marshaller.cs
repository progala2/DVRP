using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Messaging
{
    public static class Marshaller
    {
        static Marshaller() { }

        public static Message[] Unmarshall(byte[] data)
        {
            List<Message> list = new List<Message>();
            var str = Encoding.UTF8.GetString(data);
            var posBegin = str.IndexOf("<", 3, StringComparison.Ordinal) + 1;
            var posEnd = str.IndexOf(" ", posBegin, StringComparison.Ordinal);
            var typeStr = str.Substring(posBegin, posEnd - posBegin);
            var type = Message.GetMessageClassTypeFromString(typeStr);

            switch (type)
            {
                case Message.MessageClassType.DivideProblem:
                    if (MessageValidator<DivideProblemMessage>.Validate(str))
                        list.Add(MessageSerializer<DivideProblemMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.Error:
                    if (MessageValidator<ErrorMessage>.Validate(str))
                        list.Add(MessageSerializer<ErrorMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.NoOperation:
                    if (MessageValidator<NoOperationMessage>.Validate(str))
                        list.Add(MessageSerializer<NoOperationMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.PartialProblems:
                    if (MessageValidator<PartialProblemsMessage>.Validate(str))
                        list.Add(MessageSerializer<PartialProblemsMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.Register:
                    if (MessageValidator<RegisterMessage>.Validate(str))
                        list.Add(MessageSerializer<RegisterMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.RegisterResponse:
                    if (MessageValidator<RegisterResponseMessage>.Validate(str))
                        list.Add(MessageSerializer<RegisterResponseMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.SolutionRequest:
                    if (MessageValidator<SolutionRequestMessage>.Validate(str))
                        list.Add(MessageSerializer<SolutionRequestMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.Solutions:
                    if (MessageValidator<SolutionsMessage>.Validate(str))
                        list.Add(MessageSerializer<SolutionsMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.SolveRequest:
                    if (MessageValidator<SolveRequestMessage>.Validate(str))
                        list.Add(MessageSerializer<SolveRequestMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.SolveRequestResponse:
                    if (MessageValidator<SolveRequestResponseMessage>.Validate(str))
                        list.Add(MessageSerializer<SolveRequestResponseMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                case Message.MessageClassType.Status:
                    if (MessageValidator<StatusMessage>.Validate(str))
                        list.Add(MessageSerializer<StatusMessage>.Deserialize(data));
                    else
                        throw new Exception("Invalid " + typeStr + " Message");
                    break;
                default:
                    throw new Exception("Message is unknow type: \"" + typeStr + "\"");
            }
            return list.ToArray();
        }

        public static byte[] Marshall(Message[] data)
        {
            /* 
             * To samo, tylko że w drugą stronę.
             */

            throw new System.NotImplementedException();
        }
    }
}
