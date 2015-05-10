using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
            catch (Exception)
            {
                State = TaskSolverState.Error;
            }
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            if (threadCount < 1)
                throw new ArgumentOutOfRangeException("threadCount");
            int n = _dvrpProblem.Requests.Length;
            int[] AI = new int[n];
            int[] Max = new int[n];
            List<int[]> partitions = new List<int[]>();
            for (int i = n - 2; i > -1; --i)
                AI[i] = 0;
            AI[n-1] = Max[0] = -1;
            do
            {
                for (int i = 1; i < n; ++i)
                {
                    if (Max[i - 1] < AI[i - 1])
                        Max[i] = AI[i - 1];
                    else
                        Max[i] = Max[i - 1];
                }
                int p = n - 1;
                while (AI[p] == Max[p] + 1)
                {
                    AI[p] = 1;
                    p = p - 1;
                }
            AI[p] = AI[p]+1;
            Debug.WriteLine("My array: {0}",
                string.Join(", ", AI.Select(v => v.ToString()))
                );
            partitions.Add((int[])AI.Clone());              
            } while (AI[n-1] != n-1);
            DvrpPartialProblem partialProblem = new DvrpPartialProblem(partitions);
            byte[][] result = new byte[1][];
            using (var memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, partialProblem);
                result[0] = memoryStream.ToArray();
                return result;
            }
        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            double min = double.MaxValue;
            for (int i = 0; i < solutions.Length; i++)
            {
                using (var memoryStream = new MemoryStream(solutions[i]))
                {
                    var solution = (DvrpSolution)_formatter.Deserialize(memoryStream);
                    if (solution.FinalTime < min) min = solution.FinalTime;
                }
            }
            var finalSolution = new DvrpSolution(min);
            using (var memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, finalSolution);
                return memoryStream.ToArray();
            }
        }

        public override string Name
        {
            get { return "UCC.Dvrp"; }
        }

        public override byte[] Solve(byte[] partialData, System.TimeSpan timeout)
        {
            DvrpPartialProblem problem;
            using (var memoryStream = new MemoryStream(partialData))
            {
                problem = (DvrpPartialProblem)_formatter.Deserialize(memoryStream);
            }
            DvrpSolver solver = new DvrpSolver(_dvrpProblem);
            using (var memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, solver.Solve(problem, timeout));
                return memoryStream.ToArray();
            }
        }
    }

}
