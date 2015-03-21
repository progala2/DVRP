using _15pl04.Ucc.Commons;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer
{
    internal sealed class ComponentMonitor
    {
        public const int TimeBetweenStateChecks = 500;

        public delegate void DeregisterationEventHandler(object sender, DeregisterationEventArgs e);
        public event DeregisterationEventHandler Deregistration;

        public static ComponentMonitor Instance
        {
            get { return _lazy.Value; }
        }

        private static readonly Lazy<ComponentMonitor> _lazy =  new Lazy<ComponentMonitor>(() => new ComponentMonitor());

        private ConcurrentDictionary<ulong, ComponentInfo> _registeredComponents;
        private Task _stateCheckThread;
        private CancellationTokenSource _cancellationTokenSource;
        private Random _random;

        private ComponentMonitor()
        {
            _registeredComponents = new ConcurrentDictionary<ulong, ComponentInfo>();
            _random = new Random();
        }

        public ulong Register(ComponentType type, byte numberOfThreads, string[] solvableProblems)
        {
            var component = new ComponentInfo(type, numberOfThreads, solvableProblems);
            ulong id;

            do 
            {
                id = (ulong)_random.Next();
            } while (_registeredComponents.ContainsKey(id));

            _registeredComponents.TryAdd(id, component);

            return id;
        }

        public bool IsRegistered(ulong id)
        {
            return _registeredComponents.ContainsKey(id);
        }

        public void StartMonitoring(ulong timeout)
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
                        if (i.Value.TimestampAge >= timeout)
                            Deregister(i.Key);

                    if (token.IsCancellationRequested)
                        return;

                    Thread.Sleep(TimeBetweenStateChecks);
                }
            }, token);

            _stateCheckThread.Start();
        }

        private void Deregister(ulong id)
        {
            ComponentInfo info;

            if (_registeredComponents.TryRemove(id, out info))
            {
                var eventArgs = new DeregisterationEventArgs()
                {
                    ComponentInfo = info,
                    Id = id
                };

                // Invoke subscribers' methods synchronously.
                Deregistration.Invoke(this, eventArgs);
            }
            else
                throw new Exception("Unregistered component is being deregistered.");
        }

        public void StopMonitoring()
        {
            if (_stateCheckThread == null)
                return;

            _cancellationTokenSource.Cancel();            
        }
    }
}
