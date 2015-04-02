using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class ThreadStatus
    {
        [Serializable]
        [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
        public enum ThreadState
        {
            Idle,
            Busy,
        }


        [XmlElement(Order = 0)]
        public ThreadState State { get; set; }

        [XmlElement(Order = 1, ElementName = "HowLong")]
        public ulong? TimeInThisState { get; set; }

        [XmlElement(Order = 2)]
        public ulong? ProblemInstanceId { get; set; }

        [XmlElement(Order = 3, ElementName = "TaskId")]
        public ulong? PartialProblemId { get; set; } // PartialProblemId / ProblemInstanceId

        [XmlElement(Order = 4)]
        public string ProblemType { get; set; }



        public bool ShouldSerializeTimeInThisState()
        {
            return TimeInThisState.HasValue;
        }

        public bool ShouldSerializeProblemInstanceId()
        {
            return ProblemInstanceId.HasValue;
        }

        public bool ShouldSerializePartialProblemId()
        {
            return PartialProblemId.HasValue;
        }

        public bool ShouldSerializeProblemType()
        {
            return ProblemType != null;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("State(" + State + ")");

            if (TimeInThisState.HasValue)
                builder.Append(" HowLong(" + TimeInThisState + ")");

            if (ProblemInstanceId.HasValue)
                builder.Append(" ProblemInstanceId(" + ProblemInstanceId + ")");

            if (PartialProblemId.HasValue)
                builder.Append(" PartialProblemId(" + PartialProblemId + ")");

            if (ProblemType != null)
                builder.Append(" ProblemType(" + ProblemType + ")");

            return builder.ToString();
        }
    }
}
