using System;
using System.Collections.Generic;
using System.IO;
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
            // we generate all the partitions
            // set {{0 0 0 0 0}} represents {{0 1 2 3 4}}
            // set {{0 1 2 3 4}} represents {{0}{1}{2}{3}{4}}
            // ai - a partition, i - 
            // max[i] = max{max[i-1], ai[i-1]}
            // max[0] = -1
            // example:
            // 1 1 1
            // 1 1 2
            // 1 2 1
            // 1 2 2
            // 1 2 3
            
            if (threadCount < 1)
                throw new ArgumentOutOfRangeException("threadCount");
            int n = _dvrpProblem.Requests.Length;
            int[] ai = new int[n];
            int[] max = new int[n];
            // there is need to find only a start and end ai set for each thread
            // and compute the partitions dynamically in the DvrpSolver because of large memory overhead with this solution
            List<int[]> partitions = new List<int[]>();
            for (int i = n - 2; i > -1; --i)
                ai[i] = 0;
            ai[n-1] = max[0] = -1;
            do
            {
                for (int i = 1; i < n; ++i)
                {
                    if (max[i - 1] < ai[i - 1])
                        max[i] = ai[i - 1];
                    else
                        max[i] = max[i - 1];
                }
                int p = n - 1;
                while (ai[p] == max[p] + 1)
                {
                    ai[p] = 0;
                    p = p - 1;
                }
            ai[p] = ai[p]+1;
            partitions.Add((int[])ai.Clone());              
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            } while (ai[n-1] != n-1);
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
            foreach (byte[] t in solutions)
            {
                using (var memoryStream = new MemoryStream(t))
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

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
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
