using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "DivideProblem")]
    public class DivideProblemMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "DivideProblem.xsd";


        [XmlElement(Order = 0)]
        public string ProblemType { get; set; }

        [XmlElement(ElementName = "Id", Order = 1)]
        public ulong ProblemInstanceId { get; set; }

        [XmlElement(ElementName = "Data", DataType = "base64Binary", Order = 2)]
        public byte[] ProblemData { get; set; }

        [XmlElement(ElementName = "ComputationalNodes", Order = 3)]
        public ulong ComputationalNodes { get; set; }

        [XmlElement(ElementName = "NodeID", Order = 4)]
        public ulong NodeId { get; set; }


        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.DivideProblem; }
        }

        public override string ToString()
        {
            return base.ToString() +
                " ProblemId=" + ProblemInstanceId +
                "|NodeId=" + NodeId +
                "|ComputationalNodes=" + ComputationalNodes;
        }
    }
}
