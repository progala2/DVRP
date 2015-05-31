using System;
using System.Runtime.Serialization;

namespace _15pl04.Ucc.Commons.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs during loading task solvers.
    /// </summary>
    public class TaskSolverLoadingException : Exception
    {
        public TaskSolverLoadingException()
        {
        }

        public TaskSolverLoadingException(string message)
            : base(message)
        {
        }

        public TaskSolverLoadingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // This constructor is needed for serialization.
        protected TaskSolverLoadingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}