using System.Collections.Generic;

namespace _15pl04.Ucc.Commons.Messaging.Marshalling.Base
{
    public interface IMarshaller<T>
    {
        List<T> Unmarshall(byte[] rawData);
        byte[] Marshall(IList<T> data);
    }
}