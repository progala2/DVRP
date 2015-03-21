using _15pl04.Ucc.Commons;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer
{
    internal sealed class ComponentMonitor
    {
        public class Component
        {
            public uint Id { get; private set; }
            public ComponentType Type { get; private set; }
            public DateTime Timestamp { get; private set; }

            public uint TimestampAge
            {
                get
                {
                    TimeSpan diff = DateTime.UtcNow - Timestamp;
                    return (uint)diff.TotalMilliseconds;
                }
            }

            public Component(uint id, ComponentType type)
            {
                Id = id;
                Type = type;
                Timestamp = DateTime.UtcNow;
            }

            public void RefreshTimestamp()
            {
                Timestamp = DateTime.UtcNow;
            }

        }

        private const int TimeBetweenStateChecks = 500;

        private static readonly Lazy<ComponentMonitor> _lazy = 
            new Lazy<ComponentMonitor>(() => new ComponentMonitor());

        public static ComponentMonitor Instance 
        { 
            get { return _lazy.Value; } 
        }

        private ConcurrentDictionary<uint, Component> _registeredComponents;
        private Task _stateCheckThread;
        private CancellationTokenSource _cancellationTokenSource;

        private ComponentMonitor()
        {
            _registeredComponents = new ConcurrentDictionary<uint, Component>();
        }

        public void Register(uint id, ComponentType type)
        {
            var component = new Component(id, type);

            if (!_registeredComponents.TryAdd(id, component))
                throw new Exception("Component already registered.");
        }

        public bool IsRegistered(uint id)
        {
            return _registeredComponents.ContainsKey(id);
        }

        public void StartMonitoring(uint timeout)
        {
            if (_stateCheckThread != null)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;
            token.Register(() => 
            {
                _stateCheckThread = null;
            });

            _stateCheckThread = new Task(() =>
            {
                while (true)
                {
                    var now = DateTime.UtcNow;

                    foreach (var i in _registeredComponents)
                    {
                        if (i.Value.TimestampAge >= timeout)
                            ;//TODO: deregister
                    }

                    if (token.IsCancellationRequested)
                        return;

                    Thread.Sleep(TimeBetweenStateChecks);
                }
            }, token);

            _stateCheckThread.Start();
        }

        public void StopMonitoring()
        {
            if (_stateCheckThread == null)
                return;

            _cancellationTokenSource.Cancel();            
        }
    }
}
