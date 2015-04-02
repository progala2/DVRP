using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class PartialProblem
    {
        [XmlElement(ElementName = "TaskId", Order = 0)]
        public ulong PartialProblemId { get; set; }

        [XmlElement(DataType = "base64Binary", Order = 1)]
        public byte[] Data { get; set; }

        [XmlElement(ElementName = "NodeID", Order = 2)]
        public ulong NodeId { get; set; }


        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("Id(" + PartialProblemId + ")");
            builder.Append(" NodeId(" + NodeId + ")");

            return builder.ToString();
        }
    }
}
