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
    public class PartialProblemsMessage : Message
    {
        private string problemTypeField;

        private ulong idField;

        private byte[] commonDataField;

        private ulong solvingTimeoutField;

        private bool solvingTimeoutFieldSpecified;

        private List<PartialProblemsPartialProblem> partialProblemsField;

        public PartialProblemsMessage()
        {
            this.partialProblemsField = new List<PartialProblemsPartialProblem>();
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string ProblemType
        {
            get
            {
                return this.problemTypeField;
            }
            set
            {
                this.problemTypeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
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

        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary", Order = 2)]
        public byte[] CommonData
        {
            get
            {
                return this.commonDataField;
            }
            set
            {
                this.commonDataField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public ulong SolvingTimeout
        {
            get
            {
                return this.solvingTimeoutField;
            }
            set
            {
                this.solvingTimeoutField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SolvingTimeoutSpecified
        {
            get
            {
                return this.solvingTimeoutFieldSpecified;
            }
            set
            {
                this.solvingTimeoutFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Order = 4)]
        [System.Xml.Serialization.XmlArrayItemAttribute("PartialProblem", IsNullable = false)]
        public List<PartialProblemsPartialProblem> PartialProblems
        {
            get
            {
                return this.partialProblemsField;
            }
            set
            {
                this.partialProblemsField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class PartialProblemsPartialProblem
    {
        private ulong taskIdField;

        private byte[] dataField;

        private ulong nodeIDField;

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public ulong TaskId
        {
            get
            {
                return this.taskIdField;
            }
            set
            {
                this.taskIdField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary", Order = 1)]
        public byte[] Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public ulong NodeID
        {
            get
            {
                return this.nodeIDField;
            }
            set
            {
                this.nodeIDField = value;
            }
        }
    }
}
