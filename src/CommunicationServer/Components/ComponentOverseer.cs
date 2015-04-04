using _15pl04.Ucc.CommunicationServer.Components.Base;
using System;
using _15pl04.Ucc.Commons.Utilities;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer.Components
{
    public class ComponentOverseer : IComponentOverseer
    {
        public event DeregisterationEventHandler Deregistration;


        public bool IsMonitoring
        {
            get { return _isMonitoring; }
        }

        private ConcurrentDictionary<ulong, Component> _registeredComponents;
        private Random _random;

        private ulong _communicationTimeout;
        private int _checkInterval;

        private CancellationTokenSource _cancellationTokenSource;
        private volatile bool _isMonitoring;


        public ComponentOverseer(ulong communicationTimeout, int checkInterval)
        {
            _registeredComponents = new ConcurrentDictionary<ulong, Component>();
            _random = new Random();

            _communicationTimeout = communicationTimeout;
            _checkInterval = checkInterval;
        }


        public bool TryRegister(Component component)
        {
            ulong id;
            do
            {
                id = _random.NextUInt64();
            } while (!_registeredComponents.TryAdd(id, component));

            return true;
        }

        public bool TryDeregister(ulong componentId)
        {
            Component deregisteredComponent;
            if (_registeredComponents.TryRemove(componentId, out deregisteredComponent))
            {
                if (Deregistration != null)
                {
                    var args = new DeregisterationEventArgs()
                    {
                        Component = deregisteredComponent,
                    };
                    Deregistration(this, args);
                }
                return true;
            }
            else
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
            token.Register(() =>
            {
                _isMonitoring = false;
            });

            // Reset timestamps once in case some backup server took over.
            foreach (var component in _registeredComponents)
                component.Value.UpdateTimestamp();

            new Task(() =>
            {
                while (true)
                {
                    var now = DateTime.UtcNow;

                    foreach (var i in _registeredComponents)
                        if (i.Value.TimestampAge >= _communicationTimeout)
                            TryDeregister(i.Key);

                    if (token.IsCancellationRequested)
                        return;

                    Thread.Sleep(_checkInterval);
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
    }
}
