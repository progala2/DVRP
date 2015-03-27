using System;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.ComputationalClient
{
    public class ComputationalClient
    {
        private IPEndPoint _serverAddress;

        private MessageSender _messageSender;

        public ComputationalClient(IPEndPoint serverAddress)
        {
            _serverAddress = serverAddress;
            _messageSender = new MessageSender(_serverAddress);
        }

        /// <summary>
        /// Sends request for solving the problem.
        /// </summary>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="data">The serialized problem data.</param>
        /// <param name="solvingTimeout">The optional time restriction for solving the problem (in ms).</param>
        /// <returns>The ID of the problem instance assigned by the server.</returns>
        public uint SendSolveRequest(string problemType, byte[] data, ulong? solvingTimeout)
        {
            var solveRequestMessage = new SolveRequestMessage()
            {
                ProblemType = problemType,
                Data = data,
                SolvingTimeout = solvingTimeout
            };
            var responseMessages = _messageSender.Send(solveRequestMessage);

            // handle response
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends request for solution of the problem.
        /// </summary>
        /// <param name="id">The ID of the problem instance assigned by the server.</param>
        public SolutionsMessage SendSolutionRequest(uint id)
        {
            // should return received SolutionsMessage or throw some Exception..?
            // it can be changed to return something else (if it'll be better choice)
            throw new NotImplementedException();
        }
    }
}
