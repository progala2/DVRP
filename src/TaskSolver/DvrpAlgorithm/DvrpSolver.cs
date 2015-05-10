using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private int[] _carsLocations; 
        private List<int>[] _carsRoads;
        readonly DvrpProblem _dvrpProblem;
        private bool[] _visited;

        public DvrpSolver(DvrpProblem problem)
        {
            _dvrpProblem = problem;
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

            _visited = new bool[_dvrpProblem.Requests.Length];
            _carsLocations = new int[_dvrpProblem.VehicleCount];
            _carsRoads = new List<int>[_dvrpProblem.VehicleCount];
            for (int i = 0; i < _dvrpProblem.VehicleCount; i++)
            {
                _carsLocations[i] = -1;
                _carsRoads[i] = new List<int>();
            }
        }

        public DvrpSolution Solve(DvrpPartialProblem partProblem, System.TimeSpan timeout)
        {
            double min = partProblem.ApproximateResult;
            
            for (var i = 0; i < partProblem.Sets.Count; ++i)
            {
                bool breaking =false;
                List<int> cap = new List<int>();
                List<List<int>> list = new List<List<int>>();
                for (var j = 0; j < partProblem.Sets[i].Length; ++j)
                {
                    if (list.Count == partProblem.Sets[i][j])
                    {
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
                        list[partProblem.Sets[i][j]].Add(j);
                        cap[partProblem.Sets[i][j]] += (_dvrpProblem.Requests[j].Demand);
                        if (cap[partProblem.Sets[i][j]] < -_dvrpProblem.VehicleCapacity)
                        {
                            breaking = true;
                            break;
                        }
                    }
                }
                if (breaking)
                    continue;
                double distance = 0;
                for (var j = 0; j < list.Count; ++j)
                {
                    double oneCarDist = Double.MaxValue;
                    foreach (var city in list[j])
                    {
                        _visited[city] = true;
                        var carRoute = new List<int> {city};
                        double result = SolveTsp(list[j], city, _depotDistances[0, city],
                            _dvrpProblem.Requests[city].AvailabilityTime + _dvrpProblem.Requests[city].Duration,
                            oneCarDist, ref carRoute, 1);
                        if (result < oneCarDist)
                        {
                            oneCarDist = result;
                            _carsRoads[j] = new List<int>(carRoute);
                        }
                        _visited[city] = false;
                    }
                    if (oneCarDist == Double.MaxValue)
                    {
                        distance = Double.MaxValue;
                        break;
                    }
                    distance += oneCarDist;
                }
                if (min > distance)
                {
                    min = distance;
                }
            }
            int[][] roads = new int[_dvrpProblem.VehicleCount][];
            for (int i = 0; i < roads.Length; i++)
            {
                roads[i] = _carsRoads[i].ToArray();
            }
           return new DvrpSolution(min, roads);         
        }

        public double SolveApproximately()
        {
            //TODO to improve
            double result = 0;
            for (int i = 0; i < _dvrpProblem.Requests.Length; i++)
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
                    return Double.MaxValue;
                carRoute.Add(-1);
                return distance + _depotDistances[0, lastIndex];
            }
            List<int> carRouteTemp = new List<int>(carRoute);
            foreach (var i in citiesList)
            {
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
