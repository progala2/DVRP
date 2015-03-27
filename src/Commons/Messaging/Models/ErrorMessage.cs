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

        private ErrorMessageErrorType _errorMessageTypeField;

        private string _errorMessageField;

        [XmlElement(Order = 0)]
        public ErrorMessageErrorType ErrorMessageType
        {
            get
            {
                return _errorMessageTypeField;
            }
            set
            {
                _errorMessageTypeField = value;
            }
        }

        [XmlElement(Order = 1, ElementName = "ErrorMessage")]
        public string ErrorMessageText
        {
            get
            {
                return _errorMessageField;
            }
            set
            {
                _errorMessageField = value;
            }
        }

        [XmlIgnore]
        public override MessageClassType MessageType
        {
            get { return MessageClassType.Error; }
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public enum ErrorMessageErrorType
    {
        UnknownSender,

        InvalidOperation,

        ExceptionOccured,
    }
}
