using System;

namespace Dvrp.Ucc.Commons.Components
{
    /// <summary>
    /// Type of the cluster component.
    /// </summary>
    [Serializable]
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