using _15pl04.Ucc.Commons;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Components.Base
{
    public delegate void DeregisterationEventHandler(object sender, DeregisterationEventArgs e);

    public interface IComponentOverseer
    {
        event DeregisterationEventHandler Deregistration;

        bool IsMonitoring { get; }

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
