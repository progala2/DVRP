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
    public class NoOperationMessage : Message
    {
        private List<BackupCommunicationServer> backupCommunicationServersField;

        public NoOperationMessage()
        {
            backupCommunicationServersField = new List<BackupCommunicationServer>();
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("PartialProblem", IsNullable = true)]
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

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class BackupCommunicationServer
    {
        private string addressField;

        private ushort portField;

        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }
    }
}
