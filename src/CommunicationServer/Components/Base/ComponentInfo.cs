using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Components.Base
{
    public abstract class ComponentInfo
    {
        public ulong? ComponentId { get; private set; }
        public ComponentType ComponentType { get; private set; }

        public int NumberOfThreads { get; private set; }
        /// <summary>
        /// Information provided by status messages. Do not rely on this data as different cluster implementations may implement these messages slightly differently.
        /// </summary>
        public ICollection<ThreadStatus> ThreadInfo { get; set; }

        public DateTime Timestamp 
        { 
            get; 
            private set; 
        }
        public uint TimestampAge
        {
            get
            {
                TimeSpan diff = DateTime.UtcNow - Timestamp;
                return (uint)diff.TotalMilliseconds;
            }
        }

        public ComponentInfo(ComponentType type, int numberOfThreads)
        {
            ComponentType = type;
            NumberOfThreads = numberOfThreads;
        }

        public void AssignId(ulong id)
        {
            if (ComponentId != null)
                throw new Exception("Id reassign.");

            ComponentId = id;
        }

        public void UpdateTimestamp()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}
