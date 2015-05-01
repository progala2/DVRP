using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;

namespace _15pl04.Ucc.Commons.Messaging.Marshalling
{
    public class MessageValidator : IXmlValidator<MessageClass>
    {
        private readonly Dictionary<MessageClass, XmlSchemaSet> _schemaSets;

        public MessageValidator()
        {
            var capacity = Enum.GetValues(typeof (MessageClass)).Length;
            _schemaSets = new Dictionary<MessageClass, XmlSchemaSet>(capacity);

            foreach (var type in Enum.GetValues(typeof (MessageClass)).Cast<MessageClass>())
            {
                var reader = XmlReader.Create(new StringReader(type.GetXmlSchema()));

                _schemaSets.Add(type, new XmlSchemaSet());
                _schemaSets[type].Add("http://www.mini.pw.edu.pl/ucc/", reader);
            }
        }

        public bool Validate(MessageClass schemaKey, XDocument xml)
        {
            if (xml == null)
                throw new ArgumentNullException();

            if (!_schemaSets.ContainsKey(schemaKey))
                throw new ArgumentException("No schemas for the specified key exist.");

            try
            {
                xml.Validate(_schemaSets[schemaKey], null);
            }
            catch (XmlSchemaValidationException)
            {
                return false;
            }

            return true;
        }
    }
}