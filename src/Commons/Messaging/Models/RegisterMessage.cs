using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false)]
    public class RegisterMessage : Message
    {
        private RegisterType typeField;

        private List<string> solvableProblemsField;

        private byte parallelThreadsField;

        private bool deregisterField;

        private bool deregisterFieldSpecified;

        private ulong idField;

        private bool idFieldSpecified;

        public RegisterMessage()
        {
            this.solvableProblemsField = new List<string>();
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public RegisterType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Order = 1)]
        [System.Xml.Serialization.XmlArrayItemAttribute("ProblemName", IsNullable = false)]
        public List<string> SolvableProblems
        {
            get
            {
                return this.solvableProblemsField;
            }
            set
            {
                this.solvableProblemsField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public byte ParallelThreads
        {
            get
            {
                return this.parallelThreadsField;
            }
            set
            {
                this.parallelThreadsField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public bool Deregister
        {
            get
            {
                return this.deregisterField;
            }
            set
            {
                this.deregisterField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DeregisterSpecified
        {
            get
            {
                return this.deregisterFieldSpecified;
            }
            set
            {
                this.deregisterFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public ulong Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IdSpecified
        {
            get
            {
                return this.idFieldSpecified;
            }
            set
            {
                this.idFieldSpecified = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public enum RegisterType
    {
        TaskManager,

        ComputationalNode,

        CommunicationServer,
    }
}
