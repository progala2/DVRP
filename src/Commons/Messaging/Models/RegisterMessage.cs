using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Register message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Register")]
    public class RegisterMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "Register.xsd";

        /// <summary>
        /// Creates RegisterMessage instance.
        /// </summary>
        public RegisterMessage()
        {
            SolvableProblems = new List<string>();
        }

        /// <summary>
        /// The type of component.
        /// </summary>
        [XmlElement(Order = 0, ElementName = "Type")]
        public ComponentType ComponentType { get; set; }

        /// <summary>
        /// The list of names of the problems which could be solved.
        /// </summary>
        [XmlArray(Order = 1)]
        [XmlArrayItem("ProblemName", IsNullable = false)]
        public List<string> SolvableProblems { get; set; }

        /// <summary>
        /// The number of threads that could be efficiently run in parallel.
        /// </summary>
        [XmlElement(Order = 2)]
        public byte ParallelThreads { get; set; }

        /// <summary>
        /// When message is used to inform Backup Server of the need to remove element should be set to true.
        /// </summary>
        [XmlElement(Order = 3, ElementName = "Deregister")]
        public bool? Deregistration { get; set; }

        /// <summary>
        /// When message is used to inform Backup Server of the need to add/remove element it is set to ID given by main server.
        /// </summary>
        [XmlElement(Order = 4, ElementName = "Id")]
        public ulong? Id { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.Register; }
        }

        /// <summary>
        /// Determines whether Deregistration property should be serialized.
        /// </summary>
        /// <returns>True if Deregistration property should be serialized; false otherwise.</returns>
        public bool ShouldSerializeDeregistration()
        {
            return Deregistration.HasValue;
        }

        /// <summary>
        /// Determines whether Id property should be serialized.
        /// </summary>
        /// <returns>True if Id property should be serialized; false otherwise.</returns>
        public bool ShouldSerializeId()
        {
            return Id.HasValue;
        }

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            if (Deregistration.HasValue && Deregistration.Value)
            {
                builder.Append("[Deregister]");
                builder.Append(" ComponentId(" + Id + ")");
                builder.Append(" ComponentType(" + ComponentType + ")");
            }
            else
            {
                builder.Append(base.ToString());

                builder.Append(" ComponentType(" + ComponentType + ")");
                builder.Append(" ParallelThreads(" + ParallelThreads + ")");

                builder.Append(" SolvableProblems{");
                builder.Append(string.Join(",", SolvableProblems));
                builder.Append("}");
            }
            return builder.ToString();
        }
    }
}