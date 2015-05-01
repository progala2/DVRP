using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UCCTaskSolver;

namespace _15pl04.Ucc.MinMaxTaskSolver
{
    public class MMTaskSolver : TaskSolver
    {
        private readonly IFormatter _formatter;
        private readonly MMProblem _minMaxProblem;

        public MMTaskSolver(byte[] problemData)
            : base(problemData)
        {
            _formatter = new BinaryFormatter();
            try
            {
                using (var memoryStream = new MemoryStream(problemData))
                {
                    _minMaxProblem = (MMProblem) _formatter.Deserialize(memoryStream);
                }
                State = TaskSolverState.OK;
            }
            catch (Exception)
            {
                State = TaskSolverState.Error;
            }
        }

        public override string Name
        {
            get { return "_15pl04.UCC.MinMax"; }
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            if (threadCount < 1)
                throw new ArgumentOutOfRangeException("threadCount");
            var partialProblemsNumbers = new List<int>[threadCount];
            for (var i = 0; i < partialProblemsNumbers.Length; i++)
            {
                partialProblemsNumbers[i] = new List<int>(_minMaxProblem.Numbers.Length/partialProblemsNumbers.Length);
            }
            for (var i = 0; i < _minMaxProblem.Numbers.Length; i++)
            {
                partialProblemsNumbers[i%partialProblemsNumbers.Length].Add(_minMaxProblem.Numbers[i]);
            }

            var partialProblemsData = new byte[partialProblemsNumbers.Length][];
            for (var i = 0; i < partialProblemsNumbers.Length; i++)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var partialProblem = new MMPartialProblem(partialProblemsNumbers[i]);
                    _formatter.Serialize(memoryStream, partialProblem);
                    partialProblemsData[i] = memoryStream.ToArray();
                }
            }
            return partialProblemsData;
        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            var min = int.MaxValue;
            var max = int.MinValue;
            for (var i = 0; i < solutions.Length; i++)
            {
                using (var memoryStream = new MemoryStream(solutions[i]))
                {
                    var solution = (MMSolution) _formatter.Deserialize(memoryStream);
                    if (solution.Min < min) min = solution.Min;
                    if (solution.Max > max) max = solution.Max;
                }
            }
            var finalSolution = new MMSolution(min, max);
            using (var memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, finalSolution);
                return memoryStream.ToArray();
            }
        }

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            MMPartialProblem partialProblem;
            using (var memoryStream = new MemoryStream(partialData))
            {
                partialProblem = (MMPartialProblem) _formatter.Deserialize(memoryStream);
            }
            var min = int.MaxValue;
            var max = int.MinValue;
            if (partialProblem.Numbers.Length > 0)
            {
                min = max = partialProblem.Numbers[0];
                for (var i = 1; i < partialProblem.Numbers.Length; i++)
                {
                    if (partialProblem.Numbers[i] < min) min = partialProblem.Numbers[i];
                    if (partialProblem.Numbers[i] > max) max = partialProblem.Numbers[i];
                }
            }
            var solution = new MMSolution(min, max);
            using (var memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, solution);
                return memoryStream.ToArray();
            }
        }
    }
}