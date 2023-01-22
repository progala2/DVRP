using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Base
{
	/// <summary>
	/// Abstract class representing computation work assignable to the cluster component.
	/// </summary>
	public abstract class Work
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="assignedId"></param>
		protected Work(ulong assignedId)
		{
			AssignedId = assignedId;
		}
		/// <summary>
		/// ID of the component work has been assigned to.
		/// </summary>
		public ulong AssignedId { get; protected set; }
		/// <summary>
		/// Type of work to be done.
		/// </summary>
		public abstract WorkType Type { get; }
		/// <summary>
		/// Create message that can be send to a component in order to request the computaton.
		/// </summary>
		/// <returns>Generated message.</returns>
		public abstract Message CreateMessage();
	}

	/// <summary>
	/// Types of assignable work within the cluster system.
	/// </summary>
	public enum WorkType
	{
		Division = 1,
		Computation,
		Merge
	}
}