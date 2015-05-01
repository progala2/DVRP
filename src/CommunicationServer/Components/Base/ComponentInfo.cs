using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Components;

namespace _15pl04.Ucc.CommunicationServer.Components.Base
{
    public abstract class ComponentInfo
    {
        protected ComponentInfo(ComponentType type, int numberOfThreads)
        {
            ComponentType = type;
            NumberOfThreads = numberOfThreads;
        }

        public static Comparison<ComponentInfo> RegistrationTimeComparer
        {
            get { return (a, b) => a.RegistrationTimestamp.CompareTo(b.RegistrationTimestamp); }
        }

        public ulong? ComponentId { get; private set; }
        public ComponentType ComponentType { get; private set; }
        public int NumberOfThreads { get; private set; }

        /// <summary>
        ///     Information provided by status messages. Do not rely on this data as different cluster implementations may
        ///     implement these messages slightly differently.
        /// </summary>
        public ICollection<ThreadStatus> ThreadInfo { get; set; }

        public DateTime RegistrationTimestamp { get; private set; }
        public DateTime Timestamp { get; private set; }

        public uint TimestampAge
        {
            get
            {
                var diff = DateTime.UtcNow - Timestamp;
                return (uint) diff.TotalMilliseconds;
            }
        }

        public void Register(ulong id)
        {
            if (ComponentId != null)
                throw new Exception("Component re-register.");

            RegistrationTimestamp = DateTime.UtcNow;
            Timestamp = DateTime.UtcNow;

            ComponentId = id;
        }

        public void UpdateTimestamp()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}