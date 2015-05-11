using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace _15pl04.Ucc.TaskSolver.DvrpAlgorithm
{
    public class DvrpSolver
    {
        readonly double[,] _distances;
        /// <summary>
        /// index0 - depot
        /// index1 - request
        /// </summary>
        readonly double[,] _depotDistances; 
        private List<int>[] _carsRoads;
        readonly DvrpProblem _dvrpProblem;
        private readonly bool[] _visited;
        Stopwatch _timer = new Stopwatch();
        TimeSpan _timeout;

        public DvrpSolver(DvrpProblem problem)
        {
            _dvrpProblem = problem;
            var n = _dvrpProblem.Requests.Length;
            _distances = new double[n, n];
            for (var i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    var dx = _dvrpProblem.Requests[i].X - _dvrpProblem.Requests[j].X;
                    var dy = _dvrpProblem.Requests[i].Y - _dvrpProblem.Requests[j].Y;
                    _distances[i, j] = _distances[j, i] = Math.Sqrt(dx * dx + dy * dy);
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
                    _depotDistances[i, j] = Math.Sqrt(dx * dx + dy * dy);
                }
            }

            _visited = new bool[_dvrpProblem.Requests.Length];
            _carsRoads = new List<int>[_dvrpProblem.VehicleCount];
            for (var i = 0; i < _dvrpProblem.VehicleCount; i++)
            {
                _carsRoads[i] = new List<int>();
            }
        }

        public DvrpSolution Solve(DvrpPartialProblem partProblem, TimeSpan timeout)
        {
            var min = partProblem.ApproximateResult;

            int n = _dvrpProblem.Requests.Length;
            int[] max = new int[n];
            max[0] = -1;
            var part = partProblem.SetBegin;

            _timeout = timeout;
            _timer.Reset();
            _timer.Start();
            // let's check all the partitions
            for (ulong i = 0; i < partProblem.NumberOfSets && _timer.Elapsed.TotalSeconds < timeout.TotalSeconds; ++i)
            {
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
                var list = new List<List<int>>();
                for (var j = 0; j < part.Length; ++j)
                {
                    if (list.Count == part[j])
                    {
                        // With this version of algorthm we assume that one car can do only one ride
                        if (list.Count == _dvrpProblem.VehicleCount)
                        {
                            breaking = true;
                            break;
                        }
                        list.Add(new List<int>
                        {
                            j
                        });
                        cap.Add(_dvrpProblem.Requests[j].Demand);
                    }
                    else
                    {
                        list[part[j]].Add(j);
                        cap[part[j]] += (_dvrpProblem.Requests[j].Demand);
                        // The road has to be served in one ride
                        if (cap[part[j]] < -_dvrpProblem.VehicleCapacity)
                        {
                            breaking = true;
                            break;
                        }
                    }
                }
                // if any of partitions doesn't fulfil the requirements
                if (breaking)
                    continue;
                double distance = 0;
                var carsRoads = new List<int>[_dvrpProblem.VehicleCount];
                for (var j = 0; j < list.Count; ++j)
                {
                    var oneCarDist = double.MaxValue;
                    foreach (var city in list[j])
                    {
                        _visited[city] = true;
                        var carRoute = new List<int> {city};
                        var result = SolveTsp(list[j], city, _depotDistances[0, city],
                            _dvrpProblem.Requests[city].AvailabilityTime + _dvrpProblem.Requests[city].Duration,
                            oneCarDist, ref carRoute, 1);
                        if (result < oneCarDist)
                        {
                            oneCarDist = result;
                            carsRoads[j] = new List<int>(carRoute);
                        }
                        _visited[city] = false;
                    }
                    if (Math.Abs(oneCarDist - double.MaxValue) < double.Epsilon)
                    {
                        distance = double.MaxValue;
                        break;
                    }
                    distance += oneCarDist;
                }
                if (min > distance)
                {
                    min = distance;
                    for (var j = 0; j < _dvrpProblem.VehicleCount; j++)
                    {
                        if (carsRoads[j] == null) break;
                        _carsRoads[j] = new List<int>(carsRoads[j]);
                    }
                }
            }
            var roads = new int[_dvrpProblem.VehicleCount][];
            for (var i = 0; i < roads.Length; i++)
            {
                roads[i] = _carsRoads[i].ToArray();
            }
            _timer.Stop();
           return new DvrpSolution(min, roads);  
        }

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
        private double SolveTsp(List<int> citiesList, int lastIndex, double distance, double actual, double min, ref List<int> carRoute, int deepth)
        {
            if (deepth == citiesList.Count)
            {
                if (_dvrpProblem.Requests[carRoute[0]].AvailabilityTime < _dvrpProblem.Depots[0].EndTime &&
                    _depotDistances[0, lastIndex] > _dvrpProblem.Depots[0].EndTime)
                    return double.MaxValue;
                carRoute.Add(-1);
                return distance + _depotDistances[0, lastIndex];
            }
            var carRouteTemp = new List<int>(carRoute);
            foreach (var i in citiesList)
            {
                if (_timer.Elapsed.TotalSeconds >= _timeout.TotalSeconds)
                    return double.MaxValue;
                if (_visited[i])
                    continue;
                var act = _distances[lastIndex, i] + _dvrpProblem.Requests[i].Duration + Math.Max(_dvrpProblem.Requests[i].AvailabilityTime - actual, 0);
                var dist = _distances[lastIndex, i];
                actual += act;
                distance += dist;
                _visited[i] = true;
                carRouteTemp.Add(i);
                if (distance < min)
                {
                    var tmp = SolveTsp(citiesList, i, distance, actual, min, ref carRouteTemp, deepth + 1);
                    if (tmp < min)
                    {
                        min = tmp;
                        carRoute = new List<int>(carRouteTemp);
                    }
                }
                carRouteTemp.RemoveAt(carRoute.Count-1);
                distance -= dist;
                actual -= act;
                _visited[i] = false;
            }
            return min;
        }
    }
}
