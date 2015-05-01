using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "SolveRequest")]
    public class SolveRequestMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "SolveRequest.xsd";


        [XmlElement(Order = 0)]
        public string ProblemType { get; set; }

        [XmlElement(Order = 1)]
        public ulong? SolvingTimeout { get; set; }

        [XmlElement(Order = 2, ElementName="Data", DataType = "base64Binary")]
        public byte[] ProblemData { get; set; }

        [XmlElement(Order = 3, ElementName="Id")]
        public ulong? ProblemInstanceId { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.SolveRequest; }
        }


        public bool ShouldSerializeProblemInstanceId()
        {
            return ProblemInstanceId.HasValue;
        }

        public bool ShouldSerializeSolvingTimeout()
        {
            return SolvingTimeout.HasValue;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            if (ProblemInstanceId.HasValue)
                builder.Append(" ProblemInstanceId(" + ProblemInstanceId.Value + ")");

            builder.Append(" ProblemType(" + ProblemType + ")");

            if (SolvingTimeout.HasValue)
                builder.Append(" SolvingTimeout(" + SolvingTimeout.Value + ")");
            else
                builder.Append(" SolvingTimeout(none)");

            return builder.ToString();
        }
    }
}
