using System;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class Depot
    {
        public Depot(double x, double y, double startTime, double endTime)
        {
            X = x;
            Y = y;
            StartTime = startTime;
            EndTime = endTime;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double StartTime { get; private set; }
        public double EndTime { get; private set; }
    }
}
