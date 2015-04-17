using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Solutions")]
    public class SolutionsMessage : Message
    {
        [Serializable]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
        public class Solution
        {
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

        [Serializable]
        [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
        public enum SolutionType
        {
            Ongoing,
            Partial,
            Final
        }



        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "Solutions.xsd";



        [XmlElement(Order = 0)]
        public string ProblemType { get; set; }

        [XmlElement(Order = 1, ElementName="Id")]
        public ulong ProblemInstanceId { get; set; }

        [XmlElement(DataType = "base64Binary", Order = 2)]
        public byte[] CommonData { get; set; }

        [XmlArray(Order = 3)]
        [XmlArrayItem("Solution", IsNullable = false)]
        public List<Solution> Solutions { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.Solutions; }
        }



        public SolutionsMessage()
        {
            Solutions = new List<Solution>();
        }

        public bool ShouldSerializeCommonData()
        {
            return CommonData != null;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" ProblemInstanceId(" + ProblemInstanceId + ")");
            builder.Append(" ProblemType(" + ProblemType + ")");

            builder.Append(" Solutions{");
            foreach (var solution in Solutions)
                builder.Append(solution.ToString() + ",");
            builder.Append("}");

            return builder.ToString();
        }
    }
}
