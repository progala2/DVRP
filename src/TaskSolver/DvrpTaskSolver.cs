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

        readonly double[,] _distances;
        /// <summary>
        /// index0 - depot
        /// index1 - request
        /// </summary>
        readonly double[,] _depotDistances;
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
                    _distances[i, j] = _distances[j, i] = Math.Sqrt(dx * dx + dy * dy);
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
            return new byte[1][];

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
            double min = Double.MaxValue;
            bool[] visited = new bool[_dvrpProblem.Requests.Length];
            for (int i = 0; i < _dvrpProblem.Requests.Length; i++)
            {
                int capacity = _dvrpProblem.Requests[i].Demand;
                double actual = _depotDistances[0, i] + _dvrpProblem.Requests[i].Duration + _dvrpProblem.Requests[i].AvailabilityTime;
                visited[i] = true;
                double tmp = RecurrenceSolve(i, capacity, actual, 0, ref visited);
                if (tmp < min)
                    min = tmp;
                visited[i] = false;
            }
            var finalSolution = new DvrpSolution(min);
            using (var memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, finalSolution);
                return memoryStream.ToArray();
            }
        }

        private double RecurrenceSolve(int lastIndex, int capacity, double actual, int deepth, ref bool[] visited)
        {
            if (deepth == _dvrpProblem.Requests.Length)
                return actual;
            double min = Double.MaxValue;
            for (int i = 0; i < _dvrpProblem.Requests.Length; i++)
            {
                if (visited[i])
                    continue;
                if (capacity + _dvrpProblem.Requests[i].Demand >= -_dvrpProblem.VehicleCapacity)
                {
                    capacity += _dvrpProblem.Requests[i].Demand;
                    double dist = _distances[lastIndex, i] + _dvrpProblem.Requests[i].Duration + Math.Max(_dvrpProblem.Requests[i].AvailabilityTime - actual, 0);
                    actual += dist;
                    visited[i] = true;
                    if (actual <= min)
                    {
                        var tmp = RecurrenceSolve(i, capacity, actual, ++deepth, ref visited);
                        if (tmp < min)
                            min = tmp;
                    }
                    capacity -= _dvrpProblem.Requests[i].Demand;
                    actual -= dist;
                    visited[i] = false;
                }
                else
                {
                    actual += _depotDistances[0, i];
                    if (actual > min)
                    {
                        actual -= _depotDistances[0, i];
                        continue;
                    }
                    capacity = _dvrpProblem.Requests[i].Demand;
                    double dist = _depotDistances[0, i] + _dvrpProblem.Requests[i].Duration + Math.Max(_dvrpProblem.Requests[i].AvailabilityTime - actual, 0);
                    actual += dist;
                    visited[i] = true;
                    if (actual <= min)
                    {
                        var tmp = RecurrenceSolve(i, capacity, actual, ++deepth, ref visited);
                        if (tmp < min)
                            min = tmp;
                    }
                    capacity -= _dvrpProblem.Requests[i].Demand;
                    actual -= dist;
                    visited[i] = false;
                }
                
            }
            return min;
        }
    }

}
