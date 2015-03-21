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
    public class RegisterResponseMessage : Message
    { 
        private ulong idField;

        private uint timeoutField;

        private List<BackupCommunicationServer> backupCommunicationServersField;

        public RegisterResponseMessage()
        {
            this.backupCommunicationServersField = new List<BackupCommunicationServer>();
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

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public uint Timeout
        {
            get
            {
                return this.timeoutField;
            }
            set
            {
                this.timeoutField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public List<BackupCommunicationServer> BackupCommunicationServers
        {
            get
            {
                return this.backupCommunicationServersField;
            }
            set
            {
                this.backupCommunicationServersField = value;
            }
        }
    }
}
