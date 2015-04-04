using System;
using System.Xml.Linq;

namespace _15pl04.Ucc.Commons.Messaging.Base
{
    public interface IXmlValidator<T>
    {
        bool Validate(T schemaKey, XDocument xml);
    }
}
