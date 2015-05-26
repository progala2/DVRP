using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using _15pl04.Ucc.TaskSolver.DvrpAlgorithm;

namespace _15pl04.Ucc.TaskSolver
{
    public class DvrpTaskSolver : UCCTaskSolver.TaskSolver
    {
        readonly IFormatter _formatter = new BinaryFormatter();
        readonly DvrpProblem _dvrpProblem;

        public DvrpTaskSolver(byte[] problemData)
            : base(problemData)
        {
            try
            {
                string problem;
                using (var mem = new MemoryStream(problemData))
                {
                    problem = (string)_formatter.Deserialize(mem);
                }
                var data = Encoding.UTF8.GetBytes(problem);
                using (var mem = new MemoryStream(data))
                using (var r = new StreamReader(mem))
                {
                    var reg = new Regex(@"-?\d+");
                    MatchCollection m;
                    for (var i = 0; i < 6; i++)
                        r.ReadLine();

                    var line = r.ReadLine();
                    m = reg.Matches(line);
                    var numVisits = int.Parse(m[0].Value);
                    // num_location
                    r.ReadLine();

                    line = r.ReadLine();
                    m = reg.Matches(line);
                    var numVehicles = int.Parse(m[0].Value);

                    line = r.ReadLine();
                    m = reg.Matches(line);
                    var capacity = int.Parse(m[0].Value);

                    for (var i = 0; i < 4; i++)
                        r.ReadLine();
                    var demands = new int[numVisits];
                    for (var i = 0; i < demands.Length; i++)
                    {
                        line = r.ReadLine();
                        m = reg.Matches(line);
                        demands[i] = int.Parse(m[1].Value);
                    }
                    // header
                    r.ReadLine();
                    var locationCoord = new int[numVisits + 1, 2];
                    for (var i = 0; i < locationCoord.GetLength(0); i++)
                    {
                        line = r.ReadLine();
                        m = reg.Matches(line);
                        locationCoord[i, 0] = int.Parse(m[1].Value);
                        locationCoord[i, 1] = int.Parse(m[2].Value);
                    }

                    for (var i = -numVisits; i < 4; i++)
                        r.ReadLine();
                    var durations = new int[numVisits];
                    for (var i = 0; i < demands.Length; i++)
                    {
                        line = r.ReadLine();
                        m = reg.Matches(line);
                        durations[i] = int.Parse(m[1].Value);
                    }
                    // header
                    r.ReadLine();
                    line = r.ReadLine();
                    m = reg.Matches(line);
                    var depotStart = int.Parse(m[1].Value);
                    var depotEnd = int.Parse(m[2].Value);
                    for (var i = 0; i < 2; i++)
                        r.ReadLine();
                    var timeAvail = new int[numVisits];
                    for (int i = 0; i < timeAvail.Length; i++)
                    {
                        line = r.ReadLine();
                        m = reg.Matches(line);
                        timeAvail[i] = int.Parse(m[1].Value);
                    }
                    var requests = new Request[numVisits];
                    for (var i = 0; i < requests.Length; i++)
                    {
                        requests[i] = new Request(locationCoord[i + 1, 0], locationCoord[i + 1, 1], demands[i], timeAvail[i], durations[i]);
                    }
                    _dvrpProblem = new DvrpProblem(numVehicles, capacity, new[]
                    {
                        new Depot(locationCoord[0, 0], locationCoord[0, 1], depotStart, depotEnd),
                    }, requests);
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
                State = TaskSolverState.Error;
            }
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            try
            {
                var divider = new DvrpDivider();
                var problems = divider.Divide(_dvrpProblem, threadCount);
                var result = new byte[threadCount][];
                for (var i = 0; i < threadCount; ++i)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        _formatter.Serialize(memoryStream, problems[i]);
                        result[i] = memoryStream.ToArray();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                State = TaskSolverState.Error;
                return null;
            }
        }

        public override byte[] MergeSolution(byte[][] solutions)
        {
            try
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
                    string result = "Final Distance: " + finalSolution.FinalDistance;
                    foreach (var route in finalSolution.CarsRoutes)
                    {
                        result += "\n";
                        if (route.Length == 0)
                            break;
                        result = route.Aggregate(result, (current, i) => current + (" " + i));
                    }
                    _formatter.Serialize(memoryStream, result);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                State = TaskSolverState.Error;
                return null;
            }
        }

        public override string Name
        {
            get { return "UCC.Dvrp"; }
        }

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            try
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
                    State = solver.State;
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                State = TaskSolverState.Error;
                return null;
            }
        }
    }

}
