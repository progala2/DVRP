using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

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

        [XmlElement(Order = 1, ElementName = "Id")]
        public ulong ProblemInstanceId { get; set; }

        [XmlElement(Order = 2, ElementName = "Data", DataType = "base64Binary")]
        public byte[] ProblemData { get; set; }

        [XmlElement(Order = 3, ElementName = "ComputationalNodes")]
        public ulong ComputationalNodes { get; set; }

        [XmlElement(Order = 4, ElementName = "NodeID")]
        public ulong TaskManagerId { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.DivideProblem; }
        }


        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" ProblemId(" + ProblemInstanceId + ")");
            builder.Append(" ProblemType(" + ProblemType + ")");
            builder.Append(" NodeId(" + TaskManagerId + ")");
            builder.Append(" CompNodes(" + ComputationalNodes + ")");

            return builder.ToString();
        }
    }
}
