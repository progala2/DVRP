using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "NoOperation")]
    public class NoOperationMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "NoOperation.xsd";

        private List<BackupCommunicationServer> _backupCommunicationServersField;

        public NoOperationMessage()
        {
            _backupCommunicationServersField = new List<BackupCommunicationServer>();
        }

        [XmlArray(Order = 0)]
        [XmlArrayItem("BackupCommunicationServer", IsNullable = true)]
        public List<BackupCommunicationServer> BackupCommunicationServers
        {
            get
            {
                return _backupCommunicationServersField;
            }
            set
            {
                _backupCommunicationServersField = value;
            }
        }

        [XmlIgnore]
        public override MessageClassType MessageType
        {
            get { return MessageClassType.NoOperation; }
        }
    }

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class BackupCommunicationServer
    {
        private string _addressField;

        private ushort _portField;

        [XmlAttribute("address", DataType = "anyURI")]
        public string Address
        {
            get
            {
                return _addressField;
            }
            set
            {
                _addressField = value;
            }
        }

        [XmlAttribute("port")]
        public ushort Port
        {
            get
            {
                return _portField;
            }
            set
            {
                _portField = value;
            }
        }
    }
}
