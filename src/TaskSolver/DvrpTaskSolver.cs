using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var n = _dvrpProblem.Requests.Length;
            var part = new int[n];
            
            var max = new int[n];
            var maxNumberofSets = TriangularMethodBellNumber(n);
            var numberOfSetsForThread = maxNumberofSets / (ulong)threadCount;
            var result = new byte[threadCount][];
            var solver = new DvrpSolver(_dvrpProblem);
            var approximateResult = solver.SolveApproximately();

            for (var i = n - 2; i > -1; --i)
                part[i] = 0;
            part[n - 1] = max[0] = -1;
            var partLast = (int[])part.Clone();
            ulong j = 0;
            int k = 0;
            do
            {
                for (var i = 1; i < n; ++i)
                {
                    if (max[i - 1] < part[i - 1])
                        max[i] = part[i - 1];
                    else
                        max[i] = max[i - 1];
                }
                var p = n - 1;
                while (part[p] == max[p] + 1)
                {
                    part[p] = 0;
                    p = p - 1;
                }
                part[p] = part[p] + 1;
                ++j;
                if (j == numberOfSetsForThread)
                {
                    
                    using (var memoryStream = new MemoryStream())
                    {
                        _formatter.Serialize(memoryStream, new DvrpPartialProblem(partLast, approximateResult, j));
                        result[k] = memoryStream.ToArray();
                    }
                    partLast = (int[])part.Clone();
                    ++k;
                    j = 0;
                    if (k == threadCount - 1)
                    {
                        break;
                    }
                }

                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            } while (part[n - 1] != n - 1);
            using (var memoryStream = new MemoryStream())
            {
                for (int i = 0; i < n; i++)
                {
                    part[i] = i;
                }
                _formatter.Serialize(memoryStream, new DvrpPartialProblem(partLast, approximateResult, maxNumberofSets - (ulong)k * numberOfSetsForThread));
                result[k] = memoryStream.ToArray();
            }
            return result;

        }

        public override byte[] MergeSolution(byte[][] solutions)
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
            var solver = new DvrpSolver(_dvrpProblem);
            using (var memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, solver.Solve(problem, timeout));
                return memoryStream.ToArray();
            }
        }

        static ulong TriangularMethodBellNumber(int n)
        {
            Dictionary<long, List<ulong>> triangle =
                                    new Dictionary<long, List<ulong>>();
            triangle.Add(1, new List<ulong>(new ulong[] { 1 }));

            for (int i = 2; i <= n; i++)
            {
                triangle.Add(i, new List<ulong>());
                triangle[i].Add(triangle[i - 1].Last());
                for (int k = 1; k < i; k++)
                {
                    var lastVal = triangle[i][k - 1] + triangle[i - 1][k - 1];
                    triangle[i].Add(lastVal);
                }

                triangle.Remove(i - 2);
            }
            return triangle[n].Last();
        }
    }

}
