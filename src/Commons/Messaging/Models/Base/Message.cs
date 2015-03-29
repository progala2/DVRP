using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    public abstract class Message
    {
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

        public enum MessageClassType
        {
            DivideProblem,
            Error,
            NoOperation,
            PartialProblems,
            Register,
            RegisterResponse,
            SolutionRequest,
            Solutions,
            SolveRequest,
            SolveRequestResponse,
            Status
        }

        static Dictionary<string, MessageClassType> _messageClassTypeStringDictionary;

        public static MessageClassType GetMessageClassTypeFromString(string str)
        {
            if (_messageClassTypeStringDictionary == null)
            {
                _messageClassTypeStringDictionary = new Dictionary<string, MessageClassType>
                {
                    {"NoOperation", MessageClassType.NoOperation},
                    {"DivideProblem", MessageClassType.DivideProblem},
                    {"Error", MessageClassType.Error},
                    {"SolvePartialProblems", MessageClassType.PartialProblems},
                    {"PartialProblems", MessageClassType.PartialProblems},
                    {"Register", MessageClassType.Register},
                    {"RegisterResponse", MessageClassType.RegisterResponse},
                    {"SolutionRequest", MessageClassType.SolutionRequest},
                    {"Solutions", MessageClassType.Solutions},
                    {"SolveRequest", MessageClassType.SolveRequest},
                    {"SolveRequestResponse", MessageClassType.SolveRequestResponse},
                    {"Status", MessageClassType.Status}
                };
            }
            return _messageClassTypeStringDictionary[str];
        }

        [XmlIgnore]
        public abstract MessageClassType MessageType { get; }
    }
}
