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
    public class StatusMessage : Message
    {
        private ulong idField;

        private List<StatusThread> threadsField;

        public StatusMessage()
        {
            this.threadsField = new List<StatusThread>();
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
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

        [System.Xml.Serialization.XmlArrayAttribute(Order = 1)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Thread", IsNullable = false)]
        public List<StatusThread> Threads
        {
            get
            {
                return this.threadsField;
            }
            set
            {
                this.threadsField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class StatusThread
    {
        private StatusThreadState stateField;

        private ulong howLongField;

        private ulong problemInstanceIdField;

        private ulong taskIdField;

        private string problemTypeField;

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public StatusThreadState State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public ulong HowLong
        {
            get
            {
                return this.howLongField;
            }
            set
            {
                this.howLongField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public ulong ProblemInstanceId
        {
            get
            {
                return this.problemInstanceIdField;
            }
            set
            {
                this.problemInstanceIdField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
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

        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
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
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public enum StatusThreadState
    {
        Idle,

        Busy,
    }
}
