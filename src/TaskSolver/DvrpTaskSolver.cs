using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using _15pl04.Ucc.TaskSolver.DvrpAlgorithm;

namespace _15pl04.Ucc.TaskSolver
{
    public class DvrpTaskSolver : UCCTaskSolver.TaskSolver
    {
        readonly IFormatter _formatter;
        readonly DvrpProblem _dvrpProblem;

        public DvrpTaskSolver(byte[] problemData)
            : base(problemData)
        {
            _formatter = new BinaryFormatter();
            try
            {
                using (var memoryStream = new MemoryStream(problemData))
                {
                    _dvrpProblem = (DvrpProblem)_formatter.Deserialize(memoryStream);
                }
                State = TaskSolverState.OK;
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                State = TaskSolverState.Error;
            }
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            try
            {
                var divider = new DvrpDivider();
                var problems = divider.Divide(_dvrpProblem, threadCount);
                var result = new byte[threadCount][];
                for (var i = 0; i < threadCount; ++i)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        _formatter.Serialize(memoryStream, problems[i]);
                        result[i] = memoryStream.ToArray();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                State = TaskSolverState.Error;
                return null;
            }
        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            try
            {
                var finalSolution = new DvrpSolution(double.MaxValue, null);
                foreach (var t in solutions)
                {
                    using (var memoryStream = new MemoryStream(t))
                    {
                        var solution = (DvrpSolution)_formatter.Deserialize(memoryStream);
                        if (solution.FinalDistance < finalSolution.FinalDistance)
                        {
                            finalSolution = solution;
                        }
                    }
                }
                using (var memoryStream = new MemoryStream())
                {
                    _formatter.Serialize(memoryStream, finalSolution);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                State = TaskSolverState.Error;
                return null;
            }
        }

        public override string Name
        {
            get { return "UCC.Dvrp"; }
        }

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            try
            {
                DvrpPartialProblem problem;
                using (var memoryStream = new MemoryStream(partialData))
                {
                    problem = (DvrpPartialProblem)_formatter.Deserialize(memoryStream);
                }
                var solver = new DvrpSolver(_dvrpProblem);
                using (var memoryStream = new MemoryStream())
                {
                    _formatter.Serialize(memoryStream, solver.Solve(problem, timeout));
                    State = solver.State;
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                State = TaskSolverState.Error;
                return null;
            }
        }
    }

}
