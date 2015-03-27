using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "SolveRequest")]
    public class SolveRequestMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "SolveRequest.xsd";
        private string _problemTypeField;

        private ulong? _solvingTimeoutField;

        private byte[] _dataField;

        private ulong? _idField;

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
            return _solvingTimeoutField.HasValue;
        }

        [XmlElement(DataType = "base64Binary", Order = 2)]
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

        [XmlElement(Order = 3)]
        public ulong? Id
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

        public bool ShouldSerializeId()
        {
            return _idField.HasValue;
        }

        [XmlIgnore]
        public override MessageClassType MessageType
        {
            get { return MessageClassType.SolveRequest; }
        }
    }
}
