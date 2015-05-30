using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Components;

namespace _15pl04.Ucc.CommunicationServer.Components.Base
{
    /// <summary>
    /// Information about cluster component.
    /// </summary>
    public abstract class ComponentInfo
    {
        /// <summary>
        /// </summary>
        /// <param name="type">Type of the component.</param>
        /// <param name="numberOfThreads">Number of threads provided by the component.</param>
        protected ComponentInfo(ComponentType type, int numberOfThreads)
        {
            ComponentType = type;
            NumberOfThreads = numberOfThreads;
        }

        /// <summary>
        /// Compares components against their respective register time.
        /// </summary>
        public static Comparison<ComponentInfo> RegistrationTimeComparer
        {
            get { return (a, b) => a.RegistrationTimestamp.CompareTo(b.RegistrationTimestamp); }
        }

        /// <summary>
        /// ID assigned to the component (null before the assignement).
        /// </summary>
        public ulong? ComponentId { get; private set; }
        /// <summary>
        /// Type of the component.
        /// </summary>
        public ComponentType ComponentType { get; private set; }
        /// <summary>
        /// Number of threads provided by the component.
        /// </summary>
        public int NumberOfThreads { get; private set; }

        /// <summary>
        /// Information about threads provided via the Status messages.
        /// </summary>
        public ICollection<ThreadStatus> ThreadInfo { get; set; }

        /// <summary>
        /// Date of the registration in Communication Server.
        /// </summary>
        public DateTime RegistrationTimestamp { get; private set; }
        /// <summary>
        /// Date of the component's most recent connection with Communication Server.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Value in milliseconds that indicates how much time has passed since the last connection with Communication Server.
        /// </summary>
        public uint TimestampAge
        {
            get
            {
                var diff = DateTime.UtcNow - Timestamp;
                return (uint) diff.TotalMilliseconds;
            }
        }

        /// <summary>
        /// Registers the component within the Communication Server. Assigns ID and sets appropriate timestamps.
        /// </summary>
        /// <param name="id">ID to assign.</param>
        public void Register(ulong id)
        {
            if (ComponentId != null)
                throw new Exception("Component re-register.");

            RegistrationTimestamp = DateTime.UtcNow;
            Timestamp = DateTime.UtcNow;

            ComponentId = id;
        }

        /// <summary>
        /// Updates the connection timestamp.
        /// </summary>
        public void UpdateTimestamp()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}