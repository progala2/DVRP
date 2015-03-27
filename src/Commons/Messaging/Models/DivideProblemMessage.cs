using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName="DivideProblem")]
    public class DivideProblemMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "DivideProblem.xsd";

        private string _problemTypeField;

        private ulong _idField;

        private byte[] _dataField;

        private ulong _computationalNodesField;

        private ulong _nodeIdField;

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
        public ulong ComputationalNodes
        {
            get
            {
                return _computationalNodesField;
            }
            set
            {
                _computationalNodesField = value;
            }
        }

        [XmlElement("NodeID", Order = 4)]
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

        [XmlIgnore]
        public override MessageClassType MessageType
        {
            get { return MessageClassType.DivideProblem; }
        }
    }
}
