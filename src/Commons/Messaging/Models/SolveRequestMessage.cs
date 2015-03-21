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
    public class SolveRequestMessage : Message
    {
        private string problemTypeField;

        private ulong solvingTimeoutField;

        private bool solvingTimeoutFieldSpecified;

        private byte[] dataField;

        private ulong idField;

        private bool idFieldSpecified;

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

        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary", Order = 2)]
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

        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
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
}
