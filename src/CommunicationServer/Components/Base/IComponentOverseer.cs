using System;
using System.Collections.Generic;
using Dvrp.Ucc.Commons.Components;

namespace Dvrp.Ucc.CommunicationServer.Components.Base
{
    /// <summary>
    /// Handles cluster component's deregistration event.
    /// </summary>
    /// <param name="sender">Event caller.</param>en
    /// <param name="e">Event args.</param>
    public delegate void DeregistrationEventHandler(object sender, DeregisterationEventArgs e);

    /// <summary>
    /// Module responsible for monitoring and (de)registering cluster components.
    /// </summary>
    public interface IComponentOverseer
    {
        /// <summary>
        /// Maximum time that can pass before component's deregistration.
        /// </summary>
        uint CommunicationTimeout { get; }
        /// <summary>
        /// True if the component overseer is checking for communication timeout. False otherwise.
        /// </summary>
        bool IsMonitoring { get; }
        /// <summary>
        /// Invoked on component's deregistration.
        /// </summary>
        event DeregistrationEventHandler Deregistration;
        /// <summary>
        /// Tries to register a cluster component in the system.
        /// </summary>
        /// <param name="component">Information about the component to register.</param>
        /// <returns>True if succeeded to register the component. False otherwise.</returns>
        bool TryRegister(ComponentInfo component);
        /// <summary>
        /// Tries to deregister a cluster component.
        /// </summary>
        /// <param name="componentId">ID of the component to deregister.</param>
        /// <returns>True if succeeded to deregister the component. False otherwise.</returns>
        bool TryDeregister(ulong componentId);
        /// <summary>
        /// Checks whether a component is register within the system.
        /// </summary>
        /// <param name="componentId">Component ID.</param>
        /// <returns>True if the component is currently registered. False otherwise.</returns>
        bool IsRegistered(ulong componentId);
        /// <summary>
        /// Updates communication timestamp of the specified component.
        /// </summary>
        /// <param name="componentId">ID of the component.</param>
        void UpdateTimestamp(ulong componentId);
        /// <summary>
        /// Get information about the component by specifying its ID.
        /// </summary>
        /// <param name="componentId">ID of the component.</param>
        /// <returns>Component information.</returns>
        ComponentInfo GetComponent(ulong componentId);
        /// <summary>
        /// Get information about all components of specified type.
        /// </summary>
        /// <param name="type">Type of the cluster component.</param>
        /// <returns>Collection of components data.</returns>
        ICollection<ComponentInfo> GetComponents(ComponentType type);
        /// <summary>
        /// Starts checking the communication timeout and deregisters if necessary.
        /// </summary>
        void StartMonitoring();
        /// <summary>
        /// Stops checking the communication timeout.
        /// </summary>
        void StopMonitoring();
    }

    /// <summary>
    /// Event args of the deregistration event.
    /// </summary>
    public class DeregisterationEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
	    public DeregisterationEventArgs(ComponentInfo component)
	    {
		    Component = component;
	    }

	    /// <summary>
        /// Information about the deregistered component.
        /// </summary>
        public ComponentInfo Component { get; }
    }
}