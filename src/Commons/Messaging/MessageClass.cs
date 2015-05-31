using System;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Messaging
{
    /// <summary>
    /// Describes possible message types.
    /// </summary>
    public enum MessageClass
    {
        DivideProblem,
        Error,
        NoOperation,
        SolvePartialProblems,
        Register,
        RegisterResponse,
        SolutionRequest,
        Solutions,
        SolveRequest,
        SolveRequestResponse,
        Status
    }

    /// <summary>
    /// Extension methods for a MessageClass enum.
    /// </summary>
    public static class MessageClassExtensions
    {
        internal static string GetXmlSchema(this MessageClass msgClass)
        {
            switch (msgClass)
            {
                case MessageClass.DivideProblem:
                    return MessageSchemas.DivideProblem;
                case MessageClass.Error:
                    return MessageSchemas.Error;
                case MessageClass.NoOperation:
                    return MessageSchemas.NoOperation;
                case MessageClass.SolvePartialProblems:
                    return MessageSchemas.PartialProblems;
                case MessageClass.Register:
                    return MessageSchemas.Register;
                case MessageClass.RegisterResponse:
                    return MessageSchemas.RegisterResponse;
                case MessageClass.SolutionRequest:
                    return MessageSchemas.SolutionRequest;
                case MessageClass.Solutions:
                    return MessageSchemas.Solutions;
                case MessageClass.SolveRequest:
                    return MessageSchemas.SolveRequest;
                case MessageClass.SolveRequestResponse:
                    return MessageSchemas.SolveRequestResponse;
                case MessageClass.Status:
                    return MessageSchemas.Status;
                default:
                    throw new Exception("Message XML schema not found.");
            }
        }

        /// <summary>
        /// Gets type of message corresponding to this MessageClass value.
        /// </summary>
        /// <param name="msgClass"></param>
        /// <returns></returns>
        public static Type GetMessageType(this MessageClass msgClass)
        {
            switch (msgClass)
            {
                case MessageClass.DivideProblem:
                    return typeof(DivideProblemMessage);
                case MessageClass.Error:
                    return typeof(ErrorMessage);
                case MessageClass.NoOperation:
                    return typeof(NoOperationMessage);
                case MessageClass.SolvePartialProblems:
                    return typeof(PartialProblemsMessage);
                case MessageClass.Register:
                    return typeof(RegisterMessage);
                case MessageClass.RegisterResponse:
                    return typeof(RegisterResponseMessage);
                case MessageClass.SolutionRequest:
                    return typeof(SolutionRequestMessage);
                case MessageClass.Solutions:
                    return typeof(SolutionsMessage);
                case MessageClass.SolveRequest:
                    return typeof(SolveRequestMessage);
                case MessageClass.SolveRequestResponse:
                    return typeof(SolveRequestResponseMessage);
                case MessageClass.Status:
                    return typeof(StatusMessage);
                default:
                    throw new Exception("Unknown message type.");
            }
        }
    }
}