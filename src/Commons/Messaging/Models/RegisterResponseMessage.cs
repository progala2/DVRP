using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "RegisterResponse")]
    public class RegisterResponseMessage : Message
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
        [XmlArrayItem("BackupCommunicationServer")]
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
        public override MessageClass MessageType
        {
            get { return MessageClass.RegisterResponse;}
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append("Id=" + Id.ToString()+";");
            sb.Append("Timeout=" + Timeout.ToString() + ";");
            sb.Append("BackupCommunicationServers={");
            foreach (var backup in BackupCommunicationServers)
                sb.Append(backup.ToString());
            sb.Append("}]");
            return sb.ToString();
        }
    }
}
