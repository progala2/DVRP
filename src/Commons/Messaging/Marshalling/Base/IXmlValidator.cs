using System.Xml.Linq;

namespace _15pl04.Ucc.Commons.Messaging.Marshalling.Base
{
    public interface IXmlValidator<in T>
    {
        bool Validate(T schemaKey, XDocument xml);
    }
}