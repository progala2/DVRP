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
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Register")]
    public class RegisterMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "Register.xsd";

        [XmlElement(ElementName = "Type", Order = 0)]
        public ComponentType ComponentType { get; set; }

        [XmlArray(Order = 1)]
        [XmlArrayItem("ProblemName", IsNullable = false)]
        public List<string> SolvableProblems { get; set; }

        [XmlElement(Order = 2)]
        public byte ParallelThreads { get; set; }

        [XmlElement(ElementName = "Deregister", Order = 3)]
        public bool? Deregistration { get; set; }

        [XmlElement(ElementName = "Id", Order = 4)]
        public ulong? IdToDeregister { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.Register; }
        }



        public RegisterMessage()
        {
            SolvableProblems = new List<string>();
        }



        public override string ToString()
        {
            var builder = new StringBuilder();

            if (Deregistration.HasValue && Deregistration.Value)
            {
                if (!IdToDeregister.HasValue)
                    throw new Exception("Deregister message doesn't have the id to deregister");

                builder.Append("[Deregister]");
                builder.Append(" Id=" + IdToDeregister);
                builder.Append("|Component=" + ComponentType.ToString());
            }
            else
            {
                builder.Append(base.ToString());

                builder.Append(" Component=" + ComponentType.ToString());
                builder.Append("|ParallelThreads=" + ParallelThreads);

                builder.Append("|SolvableProblems={");
                foreach (var problem in SolvableProblems)
                {
                    builder.Append(problem);
                    builder.Append(",");
                }
                builder.Append("}");
            }
            return builder.ToString();
        }
    }
}
