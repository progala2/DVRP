using System;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Base
{
    /// <summary>
    /// Handles work assignment event.
    /// </summary>
    /// <param name="sender">Event caller.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void WorkAssignmentEventHandler(object sender, WorkAssignmentEventArgs e);

    /// <summary>
    /// Module responsible for storing information about problems/computations and assigning them to compatible cluster components.
    /// </summary>
    internal interface IWorkManager
    {
        /// <summary>
        /// Event that indicates assignment of computation/division/merge work to an appropriate cluster component.
        /// </summary>
        event WorkAssignmentEventHandler WorkAssignment;
        /// <summary>
        /// Try get and assign work to the specified component.
        /// </summary>
        /// <param name="node">Node to assign work to.</param>
        /// <param name="work">Assigned work.</param>
        /// <returns>Whether there is any work compatible with the component.</returns>
        bool TryAssignWork(SolverNodeInfo node, out Work? work);

        /// <summary>
        /// Adds new problem instance to the system.
        /// </summary>
        /// <param name="type">Type name of the problem.</param>
        /// <param name="data">Problem data.</param>
        /// <param name="solvingTimeout">Timeout </param>
        /// <returns>ID assigned to the problem instance.</returns>
        ulong AddProblem(string type, byte[] data, ulong solvingTimeout);
        /// <summary>
        /// Adds new partial problem to the system.
        /// </summary>
        /// <param name="problemId">ID of the problem instance this partial problem bleongs to.</param>
        /// <param name="partialProblemId">ID of the partial problem withing the problem instance.</param>
        /// <param name="privateData">Partial problem private data.</param>
        void AddPartialProblem(ulong problemId, ulong partialProblemId, byte[] privateData);
        /// <summary>
        /// Adds final solution to the system.
        /// </summary>
        /// <param name="problemId">ID of the solved problem instance.</param>
        /// <param name="data">Solution data.</param>
        /// <param name="computationsTime">Total time of problem computations.</param>
        /// <param name="timeoutOccured">Whether timeout stopped the computations.</param>
        void AddSolution(ulong problemId, byte[] data, ulong computationsTime, bool timeoutOccured);
        /// <summary>
        /// Adds new partial solution to the system.
        /// </summary>
        /// <param name="problemId">ID of the corresponding problem instance.</param>
        /// <param name="partialProblemId">ID of the corresponding partial problem.</param>
        /// <param name="data">Partial solution data.</param>
        /// <param name="computationsTime">Time of the foregoing computations.</param>
        /// <param name="timeoutOccurred">Whether timeout stopped the computations.</param>
        void AddPartialSolution(ulong problemId, ulong partialProblemId, byte[] data, ulong computationsTime,
            bool timeoutOccurred);

        /// <summary>
        /// Gets problem instance information by ID.
        /// </summary>
        /// <param name="problemId">Problem instance ID.</param>
        /// <returns>Problem instance.</returns>
        Problem? GetProblem(ulong problemId);
        /// <summary>
        /// Gets final solution data by problem instance ID.
        /// </summary>
        /// <param name="problemId">Problem instance (solution) ID.</param>
        /// <returns>Final solution.</returns>
        Solution? GetSolution(ulong problemId);
        /// <summary>
        /// Gets foregoing computations time for the problem instance specified by ID.
        /// </summary>
        /// <param name="problemId">ID of the problem instance.</param>
        /// <returns>Computations time in milliseconds.</returns>
        ulong GetComputationsTime(ulong problemId);
        /// <summary>
        /// Gets ID of the node that is currently processing (partial) problem specified by its ID.
        /// </summary>
        /// <param name="problemId">ID of the problem instance.</param>
        /// <param name="partialProblemId">ID of the partial problem within the problem instance. Can be null.</param>
        /// <returns>Problem or partial problem ID depending on partialProblemId value. Null if (partial)problem not found or isn't processed by any component.</returns>
        ulong? GetProcessingNodeId(ulong problemId, ulong? partialProblemId = null);
        /// <summary>
        /// Removes final solution from the system.
        /// </summary>
        /// <param name="problemId">Corresponding problem instance ID.</param>
        void RemoveSolution(ulong problemId);
    }

    /// <summary>
    /// Event args of the work assignement event.
    /// </summary>
    public class WorkAssignmentEventArgs : EventArgs
    {
	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="work"></param>
	    /// <param name="assigneeId"></param>
	    public WorkAssignmentEventArgs(Work work, ulong assigneeId)
	    {
		    Work = work;
		    AssigneeId = assigneeId;
	    }

	    /// <summary>
        /// Assigned work.
        /// </summary>
        public Work Work { get; set; }
        /// <summary>
        /// ID of the component work has been assigned to.
        /// </summary>
        public ulong AssigneeId { get; set; }
    }
}