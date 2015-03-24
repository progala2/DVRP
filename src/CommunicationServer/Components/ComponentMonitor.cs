using _15pl04.Ucc.Commons;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.CommunicationServer
{
    /// <summary>
    /// Singleton class that takes care of components' (de)registration and (if running on the primary Pommunication Server) checks if any of those 
    /// exceeds the timeout between subsequent messages.
    /// </summary>
    internal sealed class ComponentMonitor
    {   
        /// <summary>
        /// Sleeping time between every timeout check for registered components.
        /// </summary>
        public const int TimeBetweenStateChecks = 500;

        public delegate void DeregisterationEventHandler(object sender, DeregisterationEventArgs e);
        public event DeregisterationEventHandler Deregistration;

        /// <summary>
        /// Singleton instance.
        /// </summary>
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

        /// <summary>
        /// Check if there is a registred component with the given id.
        /// </summary>
        /// <param name="id">Component's id.</param>
        /// <returns>Boolean value that indicates whether the component is currently registered.</returns>
        public bool IsRegistered(ulong id)
        {
            return _registeredComponents.ContainsKey(id);
        }

        /// <summary>
        /// Register component.
        /// </summary>
        /// <param name="type">Component's type.</param>
        /// <param name="numberOfThreads">Number of threads provided by the component.</param>
        /// <param name="solvableProblems">Names of computational problems supported by this component.</param>
        /// <returns>Newly generated id.</returns>
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

        /// <summary>
        /// Deregister component.
        /// </summary>
        /// <param name="id">Component's id assigned by the CS during registration.</param>
        public void Deregister(ulong id)
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
                if (Deregistration != null)
                    Deregistration.Invoke(this, eventArgs);
            }
            else
                throw new Exception("Unregistered component is being deregistered.");
        }

        /// <summary>
        /// Start components' timeouts checking thread.
        /// </summary>
        /// <param name="timeout">Timeout for component's messages.</param>
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

            // Reset timestamps in case some backup server took over.
            foreach (var component in _registeredComponents)
                component.Value.UpdateTimestamp();

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

        /// <summary>
        /// Stop components' timeouts checking thread.
        /// </summary>
        public void StopMonitoring()
        {
            if (_stateCheckThread == null)
                return;

            _cancellationTokenSource.Cancel();            
        }

        public int GetNumberOfAvailableComputationalThreads(string problemName)
        {
            int threadNum = 0;

            foreach (var component in _registeredComponents.Values)
                if (component.Type == ComponentType.ComputationalNode && component.SolvableProblems.Contains(problemName))
                    threadNum += component.NumberOfIdleThreads;

            return threadNum;
        }
    }
}
