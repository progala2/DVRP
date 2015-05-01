using System.Xml.Linq;

namespace _15pl04.Ucc.Commons.Messaging.Marshalling.Base
{
    public interface IXmlValidator<T>
    {
        bool Validate(T schemaKey, XDocument xml);
    }
}
