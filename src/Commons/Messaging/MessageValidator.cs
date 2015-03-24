using _15pl04.Ucc.Commons.Messaging.Models;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace _15pl04.Ucc.Commons.Messaging
{
    /// <summary>
    /// Provides validating messages of <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T">Type derived from Message. It must have a parameterless contstructor.</typeparam>
    public class MessageValidator<T>
        where T : Message, new()
    {
        private XmlSchemaSet _xmlSchemaSet;

        public MessageValidator()
        {
            var messageInstance = new T();
            var xsdFileContent = messageInstance.GetXsdFileContent();
            _xmlSchemaSet = new XmlSchemaSet();
            _xmlSchemaSet.Add(null, XmlReader.Create(new StringReader(xsdFileContent)));
        }

        /// <summary>
        /// Validates given <paramref name="xmlDocumentContent"/> with .xsd file corresponding to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="xmlDocumentContent">Content of XML document to validate.</param>
        /// <returns>True if content of document is valid; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public bool Validate(string xmlDocumentContent)
        {
            if (xmlDocumentContent == null)
            {
                throw new ArgumentNullException("xmlDocumentContent");
            }

            bool result;
            try
            {
                var xDocument = XDocument.Parse(xmlDocumentContent);
                result = Validate(xDocument);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Validates given <paramref name="xDocument"/> with .xsd file corresponding to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="xDocument">XDocument to validate.</param>
        /// <returns>True if document is valid; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public bool Validate(XDocument xDocument)
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
