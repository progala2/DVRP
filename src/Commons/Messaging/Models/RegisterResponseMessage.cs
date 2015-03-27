using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "RegisterResponse")]
    public class RegisterResponseMessage : Message, IIdentifiableBySender
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "RegisterResponse.xsd";

        private ulong _idField;

        private uint _timeoutField;

        private List<BackupCommunicationServer> _backupCommunicationServersField;

        public RegisterResponseMessage()
        {
            _backupCommunicationServersField = new List<BackupCommunicationServer>();
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

        [XmlElement(Order = 1)]
        public uint Timeout
        {
            get
            {
                return _timeoutField;
            }
            set
            {
                _timeoutField = value;
            }
        }

        [XmlArray(Order = 2)]
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
            get { return MessageClassType.RegisterResponse;}
        }
    }
}
