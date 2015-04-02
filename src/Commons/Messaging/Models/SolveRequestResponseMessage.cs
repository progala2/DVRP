using _15pl04.Ucc.Commons.Messaging.Models.Base;
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


        [XmlElement(Order = 0, ElementName = "Id")]
        public ulong AssignedId { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.SolveRequestResponse; }
        }


        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" AssignedId(" + AssignedId + ")");

            return builder.ToString();
        }
    }
}
