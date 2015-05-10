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
                    AI[p] = 0;
                    p = p - 1;
                }
            AI[p] = AI[p]+1;
            partitions.Add((int[])AI.Clone());              
            } while (AI[n-1] != n-1);
            DvrpSolver solver = new DvrpSolver(_dvrpProblem);
            double approximateResult = solver.SolveApproximately();
            
            byte[][] result = new byte[threadCount][];
            var k = partitions.Count/threadCount;
            var d = partitions.Count%threadCount;
            var b = 0;
            for (int i = 0; i < threadCount; i++)
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (d > 0)
                    {
                        DvrpPartialProblem partialProblem = new DvrpPartialProblem(partitions.GetRange(i*k + b, k + 1),
                            approximateResult);
                        --d;
                        ++b;
                        _formatter.Serialize(memoryStream, partialProblem);
                        result[i] = memoryStream.ToArray();
                    }
                    else
                    {
                        DvrpPartialProblem partialProblem = new DvrpPartialProblem(partitions.GetRange(i * k + b, k),
                            approximateResult);
                        _formatter.Serialize(memoryStream, partialProblem);
                        result[i] = memoryStream.ToArray();
                    }
                    
                }
            }
            return result;

        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            DvrpSolution finalSolution = new DvrpSolution(double.MaxValue, null);
            for (int i = 0; i < solutions.Length; i++)
            {
                using (var memoryStream = new MemoryStream(solutions[i]))
                {
                    var solution = (DvrpSolution)_formatter.Deserialize(memoryStream);
                    if (solution.FinalTime < finalSolution.FinalTime)
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
