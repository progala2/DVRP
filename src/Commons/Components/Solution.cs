using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class Solution
    {
        [Serializable]
        [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
        public enum SolutionType
        {
            Ongoing,
            Partial,
            Final
        }


        [XmlElement(Order = 0, ElementName = "TaskId")]
        public ulong? PartialProblemId { get; set; }

        [XmlElement(Order = 1)]
        public bool TimeoutOccured { get; set; }

        [XmlElement(Order = 2)]
        public SolutionType Type { get; set; }

        [XmlElement(Order = 3)]
        public ulong ComputationsTime { get; set; }

        [XmlElement(Order = 4, DataType = "base64Binary")]
        public byte[] Data { get; set; }



        public bool ShouldSerializePartialProblemId()
        {
            return PartialProblemId.HasValue;
        }

        public bool ShouldSerializeData()
        {
            return Data != null;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("Type(" + Type.ToString() + ")");

            if (PartialProblemId.HasValue)
                builder.Append(" Id(" + PartialProblemId + ")");
            
            builder.Append(" TimeoutOccured(" + TimeoutOccured + ")");
            builder.Append(" ComputationsTime(" + ComputationsTime + ")");

            return builder.ToString();
        }
    }
}
