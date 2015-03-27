using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Solutions")]
    public class SolutionsMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "Solutions.xsd";

        private string _problemTypeField;

        private ulong _idField;

        private byte[] _commonDataField;

        private List<SolutionsSolution> _solutionsField;

        public SolutionsMessage()
        {
            _solutionsField = new List<SolutionsSolution>();
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

        public bool ShouldSerializeCommonData()
        {
            return _commonDataField != null;
        }

        [XmlArray(Order = 3)]
        [XmlArrayItem("Solution", IsNullable = false)]
        public List<SolutionsSolution> Solutions
        {
            get
            {
                return _solutionsField;
            }
            set
            {
                _solutionsField = value;
            }
        }

        [XmlIgnore]
        public override MessageClassType MessageType
        {
            get { return MessageClassType.Solutions; }
        }
    }

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class SolutionsSolution
    {
        private ulong? _taskIdField;

        private bool _timeoutOccuredField;

        private SolutionType _typeField;

        private ulong _computationsTimeField;

        private byte[] _dataField;

        [XmlElement(Order = 0)]
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

        [XmlElement(Order = 1)]
        public bool TimeoutOccured
        {
            get
            {
                return _timeoutOccuredField;
            }
            set
            {
                _timeoutOccuredField = value;
            }
        }

        [XmlElement(Order = 2)]
        public SolutionType Type
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

        [XmlElement(Order = 3)]
        public ulong ComputationsTime
        {
            get
            {
                return _computationsTimeField;
            }
            set
            {
                _computationsTimeField = value;
            }
        }

        [XmlElement(DataType = "base64Binary", Order = 4)]
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

        public bool ShouldSerializeData()
        {
            return _dataField != null;
        }
    }
}
