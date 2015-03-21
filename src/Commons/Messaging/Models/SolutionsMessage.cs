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
    public class SolutionsMessage : Message
    {
        private string problemTypeField;

        private ulong idField;

        private byte[] commonDataField;

        private List<SolutionsSolution> solutions1Field;

        public SolutionsMessage()
        {
            this.solutions1Field = new List<SolutionsSolution>();
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

        [System.Xml.Serialization.XmlArrayAttribute("Solutions", Order = 3)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Solution", IsNullable = false)]
        public List<SolutionsSolution> Solutions1
        {
            get
            {
                return this.solutions1Field;
            }
            set
            {
                this.solutions1Field = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class SolutionsSolution
    {
        private ulong taskIdField;

        private bool taskIdFieldSpecified;

        private bool timeoutOccuredField;

        private SolutionType typeField;

        private ulong computationsTimeField;

        private byte[] dataField;

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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TaskIdSpecified
        {
            get
            {
                return this.taskIdFieldSpecified;
            }
            set
            {
                this.taskIdFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public bool TimeoutOccured
        {
            get
            {
                return this.timeoutOccuredField;
            }
            set
            {
                this.timeoutOccuredField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public SolutionType Type
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

        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public ulong ComputationsTime
        {
            get
            {
                return this.computationsTimeField;
            }
            set
            {
                this.computationsTimeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary", Order = 4)]
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
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public enum SolutionType
    {
        Ongoing,

        Partial,

        Final,
    }
}
