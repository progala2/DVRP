using System;
using System.Xml.Serialization;

namespace Dvrp.Ucc.Commons.Components
{
    /// <summary>
    /// Type of the cluster component.
    /// </summary>
    [Serializable]
    [XmlType("RegisterType", AnonymousType = true, Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public enum ComponentType
    {
        /// <summary>
        /// 
        /// </summary>
        CommunicationServer = 1,
        /// <summary>
        /// 
        /// </summary>
        ComputationalClient,
        /// <summary>
        /// 
        /// </summary>
        TaskManager,
        /// <summary>
        /// 
        /// </summary>
        ComputationalNode
    }
}