using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Status")]
    public class StatusMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "Status.xsd";



        [XmlElement(Order = 0, ElementName = "Id")]
        public ulong ComponentId { get; set; }

        [XmlArray(Order = 1)]
        [XmlArrayItem("Thread", IsNullable = false)]
        public List<ThreadStatus> Threads { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.Status; }
        }



        public StatusMessage()
        {
            Threads = new List<ThreadStatus>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" ComponentId(" + ComponentId + ")");

            builder.Append(" Threads{");
            builder.Append(string.Join(",", Threads));
            builder.Append("}");

            return builder.ToString();
        }
    }
}
