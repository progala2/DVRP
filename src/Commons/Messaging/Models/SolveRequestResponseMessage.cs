using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "SolveRequestResponse")]
    public class SolveRequestResponseMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "SolveRequestResponse.xsd";

        private ulong _idField;

        [XmlElement(Order = 0)]
        public ulong Id
        {
            get
            {
                return _idField;
            }
            set
            {
                _idField = value;
            }
        }

        [XmlIgnore]
        public override MessageClassType MessageType
        {
            get { return MessageClassType.SolveRequestResponse; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append("Id=" + Id.ToString());
            sb.Append("]");
            return sb.ToString();
        }
    }
}
