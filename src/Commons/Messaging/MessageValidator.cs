using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace _15pl04.Ucc.Commons.Messaging
{
    interface IMessageValidator
    {
        void Validate(string xmlDocumentContent);
        void Validate(XDocument xDocument);
    }
    /// <summary>
    /// Provides validating messages of <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T">Type derived from Message.</typeparam>
    public class MessageValidatorHelper<T> : IMessageValidator
        where T : Message
    {
        private readonly XmlSchemaSet _xmlSchemaSet;

        public MessageValidatorHelper(MessageClass msgClass)
        {
            var xsdFileContent = msgClass.GetXmlSchema();
            _xmlSchemaSet = new XmlSchemaSet();
            _xmlSchemaSet.Add("http://www.mini.pw.edu.pl/ucc/", XmlReader.Create(new StringReader(xsdFileContent)));
        }

        /// <summary>
        /// Validates given <paramref name="xmlDocumentContent"/> with .xsd file corresponding to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="xmlDocumentContent">Content of XML document to validate.</param>
        /// <returns>True if content of document is valid; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// 
        public void Validate(string xmlDocumentContent)
        {
            if (xmlDocumentContent == null)
            {
                throw new ArgumentNullException("xmlDocumentContent");
            }

                var xDocument = XDocument.Parse(xmlDocumentContent);
            Validate(xDocument);
        }

        /// <summary>
        /// Validates given <paramref name="xDocument"/> with .xsd file corresponding to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="xDocument">XDocument to validate.</param>
        /// <returns>True if document is valid; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Validate(XDocument xDocument)
        {
            if (xDocument == null)
            {
                throw new ArgumentNullException("xDocument");
            }
             xDocument.Validate(_xmlSchemaSet, null);
        }
    }

    public static class MessageValidator
    {
        static readonly Dictionary<MessageClass, IMessageValidator> _messageValidatorForMessageTypeDictionary;

        static MessageValidator()
        {
            _messageValidatorForMessageTypeDictionary = new Dictionary<MessageClass, IMessageValidator>
                {
                    {MessageClass.NoOperation, new MessageValidatorHelper<NoOperationMessage>(MessageClass.NoOperation)},
                    {MessageClass.DivideProblem, new MessageValidatorHelper<DivideProblemMessage>(MessageClass.DivideProblem)},
                    {MessageClass.Error, new MessageValidatorHelper<ErrorMessage>(MessageClass.Error)},
                    {MessageClass.SolvePartialProblems, new MessageValidatorHelper<PartialProblemsMessage>(MessageClass.SolvePartialProblems)},
                    {MessageClass.Register, new MessageValidatorHelper<RegisterMessage>(MessageClass.Register)},
                    {MessageClass.RegisterResponse, new MessageValidatorHelper<RegisterResponseMessage>(MessageClass.RegisterResponse)},
                    {MessageClass.SolutionRequest, new MessageValidatorHelper<SolutionRequestMessage>(MessageClass.SolutionRequest)},
                    {MessageClass.Solutions, new MessageValidatorHelper<SolutionsMessage>(MessageClass.Solutions)},
                    {MessageClass.SolveRequest, new MessageValidatorHelper<SolveRequestMessage>(MessageClass.SolveRequest)},
                    {MessageClass.SolveRequestResponse, new MessageValidatorHelper<SolveRequestResponseMessage>(MessageClass.SolveRequestResponse)},
                    {MessageClass.Status, new MessageValidatorHelper<StatusMessage>(MessageClass.Status)}
                };
        }
        static IMessageValidator GetValidatorForMessageClassType(MessageClass type)
            {
            return _messageValidatorForMessageTypeDictionary[type];
            }
        public static void Validate(XDocument xDocument, MessageClass type)
            {
            GetValidatorForMessageClassType(type).Validate(xDocument);
            }

        public static void Validate(string xDocument, MessageClass type)
        {
            GetValidatorForMessageClassType(type).Validate(xDocument);
        }
    }
}
