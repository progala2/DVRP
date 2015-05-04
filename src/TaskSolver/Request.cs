using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class Request
    {
        public Request(float x, float y, int demand, float availabilityTime, float duration)
        {
            X = x;
            Y = y;
            Demand = demand;
            AvailabilityTime = availabilityTime;
            Duration = duration;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public int Demand { get; private set; }
        public float AvailabilityTime { get; private set; }
        public float Duration { get; private set; }
    }
}
