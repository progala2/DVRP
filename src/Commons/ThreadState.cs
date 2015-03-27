using System;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public enum ThreadState
    {
        Idle,
        Busy,
    }
}
