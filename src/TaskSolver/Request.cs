using System;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class Request
    {
        public Request(double x, double y, int demand, double availabilityTime, double duration)
        {
            X = x;
            Y = y;
            Demand = demand;
            AvailabilityTime = availabilityTime;
            Duration = duration;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
        public int Demand { get; private set; }
        public double AvailabilityTime { get; internal set; }
        public double Duration { get; private set; }
    }
}
