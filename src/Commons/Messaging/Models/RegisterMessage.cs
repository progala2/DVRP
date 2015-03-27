using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Register")]
    public class RegisterMessage : Message, IIdentifiableBySender
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "Register.xsd";

        private RegisterType _typeField;

        private List<string> _solvableProblemsField;

        private byte _parallelThreadsField;

        private bool? _deregisterField;

        private ulong _idField;

        private bool _specifiedIdField;

        public RegisterMessage()
        {
            _solvableProblemsField = new List<string>();
        }

        [XmlElement(Order = 0)]
        public RegisterType Type
        {
            get
            {
                return _typeField;
            }
            set
            {
                _typeField = value;
            }
        }

        [XmlArray(Order = 1)]
        [XmlArrayItem("ProblemName", IsNullable = false)]
        public List<string> SolvableProblems
        {
            get
            {
                return _solvableProblemsField;
            }
            set
            {
                _solvableProblemsField = value;
            }
        }

        [XmlElement(Order = 2)]
        public byte ParallelThreads
        {
            get
            {
                return _parallelThreadsField;
            }
            set
            {
                _parallelThreadsField = value;
            }
        }

        [XmlElement(Order = 3)]
        public bool? Deregister
        {
            get
            {
                return _deregisterField;
            }
            set
            {
                _deregisterField = value;
            }
        }

        public bool ShouldSerializeDeregister()
        {
            return _deregisterField.HasValue;
        }

        [XmlElement(Order = 4)]
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

        [XmlIgnore]
        public bool SpecifiedId
        {
            get { return _specifiedIdField; }
            set { _specifiedIdField = value; }
        }

        [XmlIgnore]
        public override MessageClassType MessageType
        {
            get { return MessageClassType.Register;}
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public enum RegisterType
    {
        TaskManager,

        ComputationalNode,

        CommunicationServer,
    }
}
