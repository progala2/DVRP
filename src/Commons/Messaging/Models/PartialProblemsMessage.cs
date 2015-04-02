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
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "SolvePartialProblems")]
    public class PartialProblemsMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "PartialProblems.xsd";


        [XmlElement(Order = 0)]
        public string ProblemType { get; set; }

        [XmlElement(Order = 1, ElementName = "Id")]
        public ulong ProblemInstanceId { get; set; }

        [XmlElement(Order = 2, DataType = "base64Binary")]
        public byte[] CommonData { get; set; }

        [XmlElement(Order = 3)]
        public ulong? SolvingTimeout { get; set; }

        [XmlArray(Order = 4)]
        [XmlArrayItem("PartialProblem", IsNullable = false)]
        public List<PartialProblem> PartialProblems { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.SolvePartialProblems;}
        }



        public PartialProblemsMessage()
        {
            PartialProblems = new List<PartialProblem>();
        }

        public bool ShouldSerializeSolvingTimeout()
        {
            return SolvingTimeout.HasValue;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" ProblemType=" + ProblemType);
            builder.Append("|ProblemInstanceId=" + ProblemInstanceId);

            if (SolvingTimeout.HasValue)
                builder.Append("|SolvingTimeout=" + SolvingTimeout.Value);

            builder.Append("|PartialProblems={");
            foreach (var pp in PartialProblems)
            {
                builder.Append("Id=" + pp.PartialProblemId);
                builder.Append(",NodeId=" + pp.NodeId);
                builder.Append("|");
            }
            builder.Append("}");

            return builder.ToString();
        }
    }
}
