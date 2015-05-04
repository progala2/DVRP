using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace _15pl04.Ucc.TaskSolver
{
    public class DvrpTaskSolver : UCCTaskSolver.TaskSolver
    {
        readonly IFormatter _formatter;
        readonly DvrpProblem _dvrpProblem;
        
        double[,] _distances;
        double[,] _depotDistances;
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
            var n = _dvrpProblem.Requests.Length;
            _distances = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double dx = _dvrpProblem.Requests[i].X - _dvrpProblem.Requests[j].X;
                    double dy = _dvrpProblem.Requests[i].Y - _dvrpProblem.Requests[j].Y;
                    _distances[i, j] = _distances[j, i] = Math.Sqrt(dx*dx + dy*dy);
                }
            }
            var m = _dvrpProblem.Depots.Length;
            _depotDistances = new double[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double dx = _dvrpProblem.Depots[i].X - _dvrpProblem.Requests[j].X;
                    double dy = _dvrpProblem.Depots[i].Y - _dvrpProblem.Requests[j].Y;
                    _depotDistances[i, j] = Math.Sqrt(dx * dx + dy * dy);
                }
            }
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            if (threadCount < 1)
                throw new ArgumentOutOfRangeException("threadCount");
            var sortList = new List<Request>(_dvrpProblem.Requests);
            sortList.Sort((request, request1) => request.Demand.CompareTo(request1.Demand));
            var linkedList = new LinkedList<Request>(sortList);
            var sets = new List<List<int>>(sortList.Count);
            for (int i = 0; i < sortList.Count; i++)
            {
                
            }

            var partialProblemsData = new byte[threadCount][];
            for (int i = 0; i < threadCount; i++)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var partialProblem = new DvrpPartialProblem(i);
                    _formatter.Serialize(memoryStream, partialProblem);
                    partialProblemsData[i] = memoryStream.ToArray();
                }
            }
            return partialProblemsData;
            
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
            using (var memoryStream = new MemoryStream(partialData))
            {
                var partProblem = (DvrpPartialProblem)_formatter.Deserialize(memoryStream);

            }
            return new byte[0];
        }
    }
}
