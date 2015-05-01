using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Base
{
    public abstract class Work
    {
        public abstract ulong AssigneeId { get; protected set; }
        public abstract WorkType Type { get; }

        public abstract Message CreateMessage();
    }

    public enum WorkType
    {
        Division = 1,
        Computation,
        Merge,
    }
}
