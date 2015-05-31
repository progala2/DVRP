using System.Xml.Linq;

namespace _15pl04.Ucc.Commons.Messaging.Marshalling.Base
{
    /// <summary>
    /// Interface to validate with xml schema.
    /// </summary>
    /// <typeparam name="T">Type of schema for validation.</typeparam>
    public interface IXmlValidator<in T>
    {
        /// <summary>
        /// Check if document is valid with schema.
        /// </summary>
        /// <param name="schemaKey">Schema for checking consistency with.</param>
        /// <param name="xml">Document to validate.</param>
        /// <returns>Returns true if xml is valid, false otherwise.</returns>
        bool Validate(T schemaKey, XDocument xml);
    }
}