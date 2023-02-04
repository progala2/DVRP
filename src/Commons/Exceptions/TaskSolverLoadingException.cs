using System;
using System.Runtime.Serialization;

namespace Dvrp.Ucc.Commons.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs during loading task solvers.
    /// </summary>
    public class TaskSolverLoadingException : Exception
    {
	    /// <inheritdoc />
	    public TaskSolverLoadingException()
        {
        }

	    /// <inheritdoc />
	    public TaskSolverLoadingException(string message)
            : base(message)
        {
        }

	    /// <inheritdoc />
	    public TaskSolverLoadingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

	    /// <inheritdoc />
	    protected TaskSolverLoadingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}