using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Components;

namespace _15pl04.Ucc.CommunicationServer.Components.Base
{
    public delegate void DeregisterationEventHandler(object sender, DeregisterationEventArgs e);

    public interface IComponentOverseer
    {
        uint CommunicationTimeout { get; }
        bool IsMonitoring { get; }
        event DeregisterationEventHandler Deregistration;
        bool TryRegister(ComponentInfo component);
        bool TryDeregister(ulong componentId);
        bool IsRegistered(ulong componentId);
        void UpdateTimestamp(ulong componentId);
        ComponentInfo GetComponent(ulong componentId);
        ICollection<ComponentInfo> GetComponents(ComponentType type);
        void StartMonitoring();
        void StopMonitoring();
    }

    public class DeregisterationEventArgs : EventArgs
    {
        public ComponentInfo Component { get; set; }
    }
}