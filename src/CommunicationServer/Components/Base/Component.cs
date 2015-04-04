
using System;
namespace _15pl04.Ucc.CommunicationServer.Components.Base
{
    public abstract class Component
    {
        public ulong? ComponentId { get; set; }

        public DateTime Timestamp { get; private set; }

        public ulong TimestampAge
        {
            get
            {
                TimeSpan diff = DateTime.UtcNow - Timestamp;
                return (ulong)diff.TotalMilliseconds;
            }

        }


        public Component()
        {
            Timestamp = DateTime.UtcNow;
        }

        public void UpdateTimestamp()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}
