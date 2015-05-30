using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace _15pl04.Ucc.TaskSolver.DvrpAlgorithm
{
    /// <summary>
    /// 
    /// </summary>
    internal class DvrpSolver
    {
        /// <summary>
        ///     index0 - depot
        ///     index1 - request
        /// </summary>
        private readonly double[,] _depotDistances;

        private readonly double[,] _distances;
        private readonly DvrpProblem _dvrpProblem;
        private readonly Stopwatch _timer = new Stopwatch();
        private readonly TspSolver _tspSolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="problem"></param>
        public DvrpSolver(DvrpProblem problem)
        {
            State = UCCTaskSolver.TaskSolver.TaskSolverState.OK;
            _dvrpProblem = problem;
            var n = _dvrpProblem.Requests.Length;
            _distances = new double[n, n];
            for (var i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    var dx = _dvrpProblem.Requests[i].X - _dvrpProblem.Requests[j].X;
                    var dy = _dvrpProblem.Requests[i].Y - _dvrpProblem.Requests[j].Y;
                    _distances[i, j] = _distances[j, i] = Math.Sqrt(dx*dx + dy*dy);
                }
            }
            var m = _dvrpProblem.Depots.Length;
            _depotDistances = new double[m, n];
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    var dx = _dvrpProblem.Depots[i].X - _dvrpProblem.Requests[j].X;
                    var dy = _dvrpProblem.Depots[i].Y - _dvrpProblem.Requests[j].Y;
                    _depotDistances[i, j] = Math.Sqrt(dx*dx + dy*dy);
                }
            }
            _tspSolver = new TspSolver(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public UCCTaskSolver.TaskSolver.TaskSolverState State { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partProblem"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public DvrpSolution Solve(DvrpPartialProblem partProblem, TimeSpan timeout)
        {
            var resultCarsRoutes = new List<int>[_dvrpProblem.VehicleCount];
            for (var i = 0; i < _dvrpProblem.VehicleCount; i++)
            {
                resultCarsRoutes[i] = new List<int>();
            }
            var min = partProblem.ApproximateResult;

            var n = _dvrpProblem.Requests.Length;
            var max = new int[n];
            max[0] = -1;
            var part = partProblem.SetBegin;

            _timer.Reset();
            _timer.Start();
            // let's check all the partitions
            for (ulong i = 0; IsLowerSet(part, partProblem.SetEnd); ++i)
            {
                if (_timer.Elapsed.TotalSeconds >= timeout.TotalSeconds)
                {
                    State = UCCTaskSolver.TaskSolver.TaskSolverState.Timeout;
                    break;
                }
                for (var j = 1; j < n; ++j)
                {
                    if (max[j - 1] < part[j - 1])
                        max[j] = part[j - 1];
                    else
                        max[j] = max[j - 1];
                }
                var p = n - 1;
                while (part[p] == max[p] + 1)
                {
                    part[p] = 0;
                    p = p - 1;
                }
                part[p] = part[p] + 1;

                var breaking = false;
                var cap = new List<int>();
                var listOfRoutes = new List<List<int>>();
                for (var j = 0; j < part.Length; ++j)
                {
                    if (listOfRoutes.Count == part[j])
                    {
                        // With this version of algorthm we assume that one car can do only one ride
                        if (listOfRoutes.Count == _dvrpProblem.VehicleCount)
                        {
                            breaking = true;
                            break;
                        }
                        listOfRoutes.Add(new List<int>
                        {
                            j
                        });
                        cap.Add(_dvrpProblem.Requests[j].Demand);
                    }
                    else
                    {
                        listOfRoutes[part[j]].Add(j);
                        cap[part[j]] += (_dvrpProblem.Requests[j].Demand);
                        // The road has to be served in one ride
                        if (cap[part[j]] < -_dvrpProblem.VehicleCapacity)
                        {
                            breaking = true;
                            var tmp = Math.Max(max[j], part[j]);
                            for (int k = j + 1; k < part.Length; k++)
                            {
                                part[k] = ++tmp;
                            }
                            break;
                        }
                    }
                }
                // if any of partitions doesn't fulfil the requirements
                if (breaking)
                    continue;
                double distance = 0;
                var carsRoutes = new List<int>[_dvrpProblem.VehicleCount];
                for (var j = 0; j < listOfRoutes.Count; ++j)
                {
                    distance += _tspSolver.Solve(listOfRoutes[j], min, out carsRoutes[j]);
                    if (distance > min)
                    {
                        int k = 0;
                        for (int l = 0; l <= j; l++)
                        {
                            k = Math.Max(listOfRoutes[l][listOfRoutes[l].Count - 1], k);    
                        }
                        
                        var tmp = Math.Max(max[k], part[k]);
                        for (k += 1; k < part.Length; k++)
                        {
                            part[k] = ++tmp;
                        }
                        break;
                    }
                }
                if (!(min > distance)) continue;

                min = distance;
                for (var j = 0; j < _dvrpProblem.VehicleCount; j++)
                {
                    if (carsRoutes[j] == null)
                        resultCarsRoutes[j] = new List<int>();
                    else
                        resultCarsRoutes[j] = new List<int>(carsRoutes[j]);
                }
            }
            var routes = new int[_dvrpProblem.VehicleCount][];
            for (var i = 0; i < routes.Length; i++)
            {
                routes[i] = resultCarsRoutes[i].ToArray();
            }
            _timer.Stop();
            return new DvrpSolution(min, routes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double SolveApproximately()
        {
            //TODO to improve
            double result = 0;
            for (var i = 0; i < _dvrpProblem.Requests.Length; i++)
            {
                result += _depotDistances[0, i]*2;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lowerSet"></param>
        /// <param name="higherSet"></param>
        /// <returns></returns>
        private bool IsLowerSet(int[] lowerSet, int[] higherSet)
        {
            for (int i = 0; i < lowerSet.Length; i++)
            {
                if (lowerSet[i] == higherSet[i])
                {
                    continue;
                }
                return lowerSet[i] < higherSet[i];
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal class TspSolver
        {
            private readonly double[,] _depotDistances;
            private readonly double[,] _distances;
            private readonly DvrpProblem _dvrpProblem;
            private readonly bool[] _visited;
            private List<int> _carFinalRoute;
            private List<int> _listOfCities;
            private double _oneCarDistance;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="solver"></param>
            public TspSolver(DvrpSolver solver)
            {
                _distances = solver._distances;
                _depotDistances = solver._depotDistances;
                _dvrpProblem = solver._dvrpProblem;

                _visited = new bool[_dvrpProblem.Requests.Length];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="listOfCities"></param>
            /// <param name="min"></param>
            /// <param name="carRoute"></param>
            /// <returns></returns>
            public double Solve(List<int> listOfCities, double min, out List<int> carRoute)
            {
                _oneCarDistance = min;
                _listOfCities = listOfCities;
                foreach (var city in _listOfCities)
                {
                    _visited[city] = true;
                    var route = new List<int> {city};
                    RecurenceSolve(_depotDistances[0, city],
                        _dvrpProblem.Requests[city].AvailabilityTime + _dvrpProblem.Requests[city].Duration, ref route);
                    _visited[city] = false;
                }
                carRoute = _carFinalRoute;
                return _oneCarDistance;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="distance"></param>
            /// <param name="actual"></param>
            /// <param name="carRoute"></param>
            private void RecurenceSolve(double distance, double actual, ref List<int> carRoute)
            {
                if (carRoute.Count == _listOfCities.Count)
                {
                    if (_dvrpProblem.Requests[carRoute[0]].AvailabilityTime < _dvrpProblem.Depots[0].EndTime &&
                        _depotDistances[0, carRoute.Last()] > _dvrpProblem.Depots[0].EndTime)
                        return;
                    distance += _depotDistances[0, carRoute.Last()];
                    if (!(distance < _oneCarDistance)) return;

                    _carFinalRoute = new List<int>(carRoute);
                    _oneCarDistance = distance;
                    return;
                }
                foreach (var i in _listOfCities)
                {
                    if (_visited[i])
                        continue;
                    var act = _distances[carRoute.Last(), i] + _dvrpProblem.Requests[i].Duration +
                              Math.Max(_dvrpProblem.Requests[i].AvailabilityTime - actual, 0);
                    var dist = _distances[carRoute.Last(), i];
                    actual += act;
                    distance += dist;
                    _visited[i] = true;
                    carRoute.Add(i);
                    if (distance < _oneCarDistance)
                    {
                        RecurenceSolve(distance, actual, ref carRoute);
                    }
                    carRoute.RemoveAt(carRoute.Count - 1);
                    distance -= dist;
                    actual -= act;
                    _visited[i] = false;
                }
            }
        }
    }
}
