using _15pl04.Ucc.Commons.Messaging.Models.Base;
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



        static Dictionary<string, MessageClass> _messageClassTypeStringDictionary;

        public static MessageClass GetMessageClassTypeFromString(string str)
        {
            if (_messageClassTypeStringDictionary == null)
            {
                _messageClassTypeStringDictionary = new Dictionary<string, MessageClass>
                {
                    {"NoOperation", MessageClass.NoOperation},
                    {"DivideProblem", MessageClass.DivideProblem},
                    {"Error", MessageClass.Error},
                    {"SolvePartialProblems", MessageClass.PartialProblems},
                    {"PartialProblems", MessageClass.PartialProblems},
                    {"Register", MessageClass.Register},
                    {"RegisterResponse", MessageClass.RegisterResponse},
                    {"SolutionRequest", MessageClass.SolutionRequest},
                    {"Solutions", MessageClass.Solutions},
                    {"SolveRequest", MessageClass.SolveRequest},
                    {"SolveRequestResponse", MessageClass.SolveRequestResponse},
                    {"Status", MessageClass.Status}
                };
            }
            return _messageClassTypeStringDictionary[str];
        }

        [XmlIgnore]
        public abstract MessageClass MessageType { get; }
    }
}
