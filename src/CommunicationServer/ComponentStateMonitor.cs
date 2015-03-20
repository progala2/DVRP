using _15pl04.Ucc.Commons;
using System.Collections.Concurrent;

namespace _15pl04.Ucc.CommunicationServer
{
    internal class ComponentStateMonitor
    {
        private ConcurrentDictionary<uint, Component> _registeredComponents;

        public ComponentStateMonitor()
        {
            _registeredComponents = new ConcurrentDictionary<uint, Component>();
        }

        public class Component
        {
            public uint Id { get; private set; }
            public ComponentType Type { get; private set; }

            public Component(uint id, ComponentType type)
            {
                Id = id;
                Type = type;
            }
        }

        public void Register(uint id, ComponentType type)
        {

        }

        public bool IsRegistered()
        {
            return false;
        }

        public void Start()
        {
            /*
             * Asynchronously check the dictionary and remove inactive components.
             */
        }

        public void Stop()
        {
            /*
             * Cleanup.
             */
        }
    }
}
