using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Utilities;
using _15pl04.Ucc.CommunicationServer.Components.Base;

namespace _15pl04.Ucc.CommunicationServer.Components
{
    internal class ComponentOverseer : IComponentOverseer
    {
        private static readonly ILogger Logger = new ConsoleLogger();
        private readonly Random _random;
        private readonly ConcurrentDictionary<ulong, ComponentInfo> _registeredComponents;
        private CancellationTokenSource _cancellationTokenSource;
        private volatile bool _isMonitoring;

        public ComponentOverseer(uint communicationTimeout, uint checkInterval)
        {
            _registeredComponents = new ConcurrentDictionary<ulong, ComponentInfo>();
            _random = new Random();

            CommunicationTimeout = communicationTimeout;
            CheckInterval = checkInterval;
        }

        public uint CheckInterval { get; private set; }
        public event DeregisterationEventHandler Deregistration;
        public uint CommunicationTimeout { get; private set; }

        public bool IsMonitoring
        {
            get { return _isMonitoring; }
        }

        public bool TryRegister(ComponentInfo component)
        {
            if (component.ComponentId != null)
                throw new Exception("Registering component with id already assigned.");

            ulong id;
            do
            {
                id = _random.NextUInt64();
            } while (!_registeredComponents.TryAdd(id, component));

            component.Register(id);

            return true;
        }

        public bool TryDeregister(ulong componentId)
        {
            ComponentInfo deregisteredComponent;
            if (_registeredComponents.TryRemove(componentId, out deregisteredComponent))
            {
                if (Deregistration != null)
                {
                    Logger.Info("Deregistering " + deregisteredComponent.ComponentType +
                                " (id: " + deregisteredComponent.ComponentId + ").");

                    var args = new DeregisterationEventArgs
                    {
                        Component = deregisteredComponent
                    };
                    Deregistration(this, args);
                }
                return true;
            }
            return false;
        }

        public bool IsRegistered(ulong componentId)
        {
            return _registeredComponents.ContainsKey(componentId);
        }

        public void UpdateTimestamp(ulong componentId)
        {
            if (!_registeredComponents.ContainsKey(componentId))
                throw new ArgumentException("Timestamp for an unregistered component was requested.");

            _registeredComponents[componentId].UpdateTimestamp();
        }

        public void StartMonitoring()
        {
            if (_isMonitoring)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;
            token.Register(() => { _isMonitoring = false; });

            // Reset timestamps once in case some backup server took over.
            foreach (var component in _registeredComponents)
                component.Value.UpdateTimestamp();

            new Task(() =>
            {
                while (true)
                {
                    var now = DateTime.UtcNow;

                    foreach (var i in _registeredComponents)
                        if (i.Value.TimestampAge >= CommunicationTimeout)
                            TryDeregister(i.Key);

                    if (token.IsCancellationRequested)
                        return;

                    Thread.Sleep((int) CheckInterval);
                }
            }, token).Start();

            _isMonitoring = true;
        }

        public void StopMonitoring()
        {
            if (!_isMonitoring)
                return;

            _cancellationTokenSource.Cancel();
        }

        public ICollection<ComponentInfo> GetComponents(ComponentType type)
        {
            var components = _registeredComponents.Values.Cast<ComponentInfo>().Where(c => c.ComponentType == type);

            return new List<ComponentInfo>(components);
        }

        public ComponentInfo GetComponent(ulong componentId)
        {
            if (!_registeredComponents.ContainsKey(componentId))
                throw new ArgumentException("No component info for specified id exists.");

            return _registeredComponents[componentId];
        }
    }
}