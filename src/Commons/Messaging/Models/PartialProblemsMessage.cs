using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "SolvePartialProblems")]
    public class PartialProblemsMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "PartialProblems.xsd";

        private string _problemTypeField;

        private ulong _idField;

        private byte[] _commonDataField;

        private ulong? _solvingTimeoutField;

        private List<RegisterResponsePartialProblem> _partialProblemsField;

        public PartialProblemsMessage()
        {
            _partialProblemsField = new List<RegisterResponsePartialProblem>();
        }

        [XmlElement(Order = 0)]
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

        [XmlElement(Order = 1)]
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

        [XmlElement(DataType = "base64Binary", Order = 2)]
        public byte[] CommonData
        {
            get
            {
                return _commonDataField;
            }
            set
            {
                _commonDataField = value;
            }
        }

        [XmlElement(Order = 3)]
        public ulong? SolvingTimeout
        {
            get
            {
                return _solvingTimeoutField;
            }
            set
            {
                _solvingTimeoutField = value;
            }
        }

        public bool ShouldSerializeSolvingTimeout()
        {
            return SolvingTimeout.HasValue;
        }

        [XmlArray(Order = 4)]
        [XmlArrayItem("PartialProblem", IsNullable = false)]
        public List<RegisterResponsePartialProblem> PartialProblems
        {
            get
            {
                return _partialProblemsField;
            }
            set
            {
                _partialProblemsField = value;
            }
        }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.SolvePartialProblems;}
        }
    }

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class RegisterResponsePartialProblem
    {
        private ulong _taskIdField;

        private byte[] _dataField;

        private ulong _nodeIdField;

        [XmlElement(Order = 0)]
        public ulong TaskId
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

        [XmlElement(DataType = "base64Binary", Order = 1)]
        public byte[] Data
        {
            get
            {
                return _dataField;
            }
            set
            {
                _dataField = value;
            }
        }

        [XmlElement("NodeID", Order = 2)]
        public ulong NodeId
        {
            get
            {
                return _nodeIdField;
            }
            set
            {
                _nodeIdField = value;
            }
        }
    }
}
