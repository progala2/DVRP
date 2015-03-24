using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Messaging
{
    /// <summary>
    /// Provides validating messages of <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T">Type derived from Message.</typeparam>
    public static class MessageValidator<T>
        where T : Message
    {
        private static readonly XmlSchemaSet _xmlSchemaSet;

        static MessageValidator()
        {
            var xsdFileContent = Message.GetXsdFileContent(typeof(T));
            _xmlSchemaSet = new XmlSchemaSet();
            _xmlSchemaSet.Add(null, XmlReader.Create(new StringReader(xsdFileContent)));
        }

        /// <summary>
        /// Validates given <paramref name="xmlDocumentContent"/> with .xsd file corresponding to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="xmlDocumentContent">Content of XML document to validate.</param>
        /// <returns>True if content of document is valid; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool Validate(string xmlDocumentContent)
        {
            if (xmlDocumentContent == null)
            {
                throw new ArgumentNullException("xmlDocumentContent");
            }

            var xDocument = XDocument.Parse(xmlDocumentContent);
            bool result = Validate(xDocument);
            return result;
        }

        /// <summary>
        /// Validates given <paramref name="xDocument"/> with .xsd file corresponding to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="xDocument">XDocument to validate.</param>
        /// <returns>True if document is valid; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool Validate(XDocument xDocument)
        {
            if (xDocument == null)
            {
                throw new ArgumentNullException("xDocument");
            }

            bool result;
            try
            {
                xDocument.Validate(_xmlSchemaSet, null);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
