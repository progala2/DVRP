using _15pl04.Ucc.Commons.Messaging.Models.Base;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Messaging.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Error")]
    public class ErrorMessage : Message
    {
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation = "Error.xsd";


        [XmlElement(Order = 0, ElementName = "ErrorMessageType")]
        public ErrorType ErrorType { get; set; }

        [XmlElement(Order = 1, ElementName = "ErrorMessage")]
        public string ErrorText { get; set; }

        [XmlIgnore]
        public override MessageClass MessageType
        {
            get { return MessageClass.Error; }
        }



        public override string ToString()
        {
            return base.ToString() +
                " ErrorType=" + ErrorType +
                "|ErrorMessage=" + ErrorText;
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public enum ErrorType
    {
        UnknownSender,
        InvalidOperation,
        ExceptionOccured,
    }
}
