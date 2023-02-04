using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Models
{
    /// <summary>
    /// Object representation of the Error message.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    [XmlRoot(Namespace = "http://www.mini.pw.edu.pl/ucc/", IsNullable = false, ElementName = "Error")]
    public class ErrorMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string NoNamespaceSchemaLocation = "Error.xsd";

        /// <summary>
        /// The type of error.
        /// </summary>
        [XmlElement(Order = 0, ElementName = "ErrorMessageType")]
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// The information about error.
        /// </summary>
        [XmlElement(Order = 1, ElementName = "ErrorMessage")]
        public string? ErrorText { get; set; }

        /// <summary>
        /// Gets corresponding MessageClass enum value.
        /// </summary>
        [XmlIgnore]
        public override MessageClass MessageType => MessageClass.Error;

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>String value that represents this object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.Append(" ErrorType(" + ErrorType + ")");
            builder.Append(" ErrorMessage(" + ErrorText + ")");

            return builder.ToString();
        }
    }

    /// <summary>
    /// Possible errors types.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public enum ErrorType
    {
        /// <summary>
        /// 
        /// </summary>
        UnknownSender,
        /// <summary>
        /// 
        /// </summary>
        InvalidOperation,
        /// <summary>
        /// 
        /// </summary>
        ExceptionOccured
    }
}