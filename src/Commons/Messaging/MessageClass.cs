using System;

namespace _15pl04.Ucc.Commons.Messaging.Models.Base
{
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

    public static class MessageClassExtensions
    {
        internal static string GetXmlSchema(this MessageClass msgClass)
        {
            string schema;

            switch (msgClass)
            {
                case MessageClass.DivideProblem:
                    schema = MessageSchemas.DivideProblem;
                    break;
                case MessageClass.Error:
                    schema = MessageSchemas.Error;
                    break;
                case MessageClass.NoOperation:
                    schema = MessageSchemas.NoOperation;
                    break;
                case MessageClass.SolvePartialProblems:
                    schema = MessageSchemas.PartialProblems;
                    break;
                case MessageClass.Register:
                    schema = MessageSchemas.Register;
                    break;
                case MessageClass.RegisterResponse:
                    schema = MessageSchemas.RegisterResponse;
                    break;
                case MessageClass.SolutionRequest:
                    schema = MessageSchemas.SolutionRequest;
                    break;
                case MessageClass.Solutions:
                    schema = MessageSchemas.Solutions;
                    break;
                case MessageClass.SolveRequest:
                    schema = MessageSchemas.SolveRequest;
                    break;
                case MessageClass.SolveRequestResponse:
                    schema = MessageSchemas.SolveRequestResponse;
                    break;
                case MessageClass.Status:
                    schema = MessageSchemas.Status;
                    break;
                default:
                    throw new Exception("Message XML schema not found.");
            }

            return schema;
        }
    }
}
