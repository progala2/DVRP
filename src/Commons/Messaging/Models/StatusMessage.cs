using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Status")]
    public class StatusMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "Status.xsd";

        private ulong _idField;

        private List<ThreadStatus> _threadsField;

        public StatusMessage()
        {
            _threadsField = new List<ThreadStatus>();
        }

        [XmlElement(Order = 0)]
        public ulong Id
        {
            get
            {
                return _idField;
            }
            set
            {
                _idField = value;
            }
        }

        [XmlArray(Order = 1)]
        [XmlArrayItem("Thread", IsNullable = false)]
        public List<ThreadStatus> Threads
        {
            get
            {
                return _threadsField;
            }
            set
            {
                _threadsField = value;
            }
        }

        [XmlIgnore]
        public override MessageClassType MessageType
        {
            get { return MessageClassType.Status; }
        }
    }

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class ThreadStatus
    {
        private ThreadState _stateField;

        private ulong? _howLongField;

        private ulong? _problemInstanceIdField;

        private ulong? _taskIdField;

        private string _problemTypeField;

        [XmlElement(Order = 0)]
        public ThreadState State
        {
            get
            {
                return _stateField;
            }
            set
            {
                _stateField = value;
            }
        }

        [XmlElement(Order = 1)]
        public ulong? HowLong
        {
            get
            {
                return _howLongField;
            }
            set
            {
                _howLongField = value;
            }
        }

        public bool ShouldSerializeHowLong()
        {
            return _howLongField.HasValue;
        }

        [XmlElement(Order = 2)]
        public ulong? ProblemInstanceId
        {
            get
            {
                return _problemInstanceIdField;
            }
            set
            {
                _problemInstanceIdField = value;
            }
        }

        public bool ShouldSerializeProblemInstanceId()
        {
            return _problemInstanceIdField.HasValue;
        }

        [XmlElement(Order = 3)]
        public ulong? TaskId
        {
            get
            {
                return _taskIdField;
            }
            set
            {
                _taskIdField = value;
            }
        }

        public bool ShouldSerializeTaskId()
        {
            return _taskIdField.HasValue;
        }

        [XmlElement(Order = 4)]
        public string ProblemType
        {
            get
            {
                return _problemTypeField;
            }
            set
            {
                _problemTypeField = value;
            }
        }

        public bool ShouldSerializeProblemType()
        {
            return _problemTypeField != null;
        }
    }
}
