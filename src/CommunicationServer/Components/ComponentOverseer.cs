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
    /// <summary>
    /// Module responsible for monitoring and (de)registering cluster components.
    /// </summary>
    internal class ComponentOverseer : IComponentOverseer
    {
        private static readonly ILogger Logger = new ConsoleLogger();
        private readonly Random _random;
        private readonly ConcurrentDictionary<ulong, ComponentInfo> _registeredComponents;
        private CancellationTokenSource _cancellationTokenSource;
        private volatile bool _isMonitoring;

        /// <summary>
        /// Creates ComponentOverseer instance.
        /// </summary>
        /// <param name="communicationTimeout">Communication timeout in seconds.</param>
        /// <param name="checkInterval">Communication timeout check interval in seconds.</param>
        public ComponentOverseer(uint communicationTimeout, uint checkInterval)
        {
            _registeredComponents = new ConcurrentDictionary<ulong, ComponentInfo>();
            _random = new Random();

            CommunicationTimeout = communicationTimeout;
            CheckInterval = checkInterval;
        }

        /// <summary>
        /// Communication timeout check interval in seconds.
        /// </summary>
        public uint CheckInterval { get; private set; }

        /// <summary>
        /// Communication timeout in seconds.
        /// </summary>
        public uint CommunicationTimeout { get; private set; }

        /// <summary>
        /// Invoked on component's deregistration.
        /// </summary>
        public event DeregisterationEventHandler Deregistration;

        /// <summary>
        /// True if the component overseer is checking for communication timeout. False otherwise.
        /// </summary>
        public bool IsMonitoring
        {
            get { return _isMonitoring; }
        }

        /// <summary>
        /// Tries to register a cluster component in the system.
        /// </summary>
        /// <param name="component">Information about the component to register.</param>
        /// <returns>True if succeeded to register the component. False otherwise.</returns>
        public bool TryRegister(ComponentInfo component)
        {
            if (component == null)
                throw new ArgumentNullException();
            if (component.ComponentId != null)
                throw new Exception("Registering component with id already assigned.");

            ulong id;
            do
            {
                id = _random.NextUInt64() % 100; //TODO this is debug solution
                //id = _random.NextUInt64(); 
            } while (!_registeredComponents.TryAdd(id, component));

            Logger.Info("Registering " + component.ComponentType +
                        " (id: " + id + ").");

            component.Register(id);

            return true;
        }

        /// <summary>
        /// Tries to deregister a cluster component.
        /// </summary>
        /// <param name="componentId">ID of the component to deregister.</param>
        /// <returns>True if succeeded to deregister the component. False otherwise.</returns>
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

        /// <summary>
        /// Checks whether a component is register within the system.
        /// </summary>
        /// <param name="componentId">Component ID.</param>
        /// <returns>True if the component is currently registered. False otherwise.</returns>
        public bool IsRegistered(ulong componentId)
        {
            return _registeredComponents.ContainsKey(componentId);
        }

        /// <summary>
        /// Updates communication timestamp of the specified component.
        /// </summary>
        /// <param name="componentId">ID of the component.</param>
        public void UpdateTimestamp(ulong componentId)
        {
            ComponentInfo component;
            if (!_registeredComponents.TryGetValue(componentId, out component))
                throw new ArgumentException("Timestamp for an unregistered component was requested.");

            component.UpdateTimestamp();
        }

        /// <summary>
        /// Starts checking the communication timeout and deregisters if necessary.
        /// </summary>
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
                    foreach (var i in _registeredComponents)
                        if (i.Value.TimestampAge > 1000 * CommunicationTimeout)
                            TryDeregister(i.Key);

                    if (token.IsCancellationRequested)
                        return;

                    Thread.Sleep((int)(1000 * CheckInterval));
                }
            }, token).Start();

            _isMonitoring = true;
        }

        /// <summary>
        /// Stops checking the communication timeout.
        /// </summary>
        public void StopMonitoring()
        {
            if (!_isMonitoring)
                return;

            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Get imformation about all components of specified type.
        /// </summary>
        /// <param name="type">Type of the cluster component.</param>
        /// <returns>Collection of components data.</returns>
        public ICollection<ComponentInfo> GetComponents(ComponentType type)
        {
            var components = _registeredComponents.Values.Where(c => c.ComponentType == type);

            return new List<ComponentInfo>(components);
        }

        /// <summary>
        /// Get information about the component by specifying its ID.
        /// </summary>
        /// <param name="componentId">ID of the component.</param>
        /// <returns>Component information.</returns>
        public ComponentInfo GetComponent(ulong componentId)
        {
            ComponentInfo component;
            if (!_registeredComponents.TryGetValue(componentId, out component))
                throw new ArgumentException("No component info for specified id exists.");

            return component;
        }
    }
}