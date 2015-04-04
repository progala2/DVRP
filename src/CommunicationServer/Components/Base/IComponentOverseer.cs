
using System;
namespace _15pl04.Ucc.CommunicationServer.Components.Base
{
    public delegate void DeregisterationEventHandler(object sender, DeregisterationEventArgs e);

    public interface IComponentOverseer
    {
        event DeregisterationEventHandler Deregistration;

        bool IsMonitoring { get; }

        bool TryRegister(Component component);

        bool TryDeregister(ulong componentId);

        bool IsRegistered(ulong componentId);

        void UpdateTimestamp(ulong componentId);

        void StartMonitoring();

        void StopMonitoring();
    }

    public class DeregisterationEventArgs : EventArgs
    {
        public Component Component { get; set; }

    }
}
