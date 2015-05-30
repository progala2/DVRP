using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// Class containing necessery information about a client's request.
    /// </summary>
    [Serializable]
    public class Request
    {
        /// <summary>
        /// Constructor of the Request class.
        /// </summary>
        /// <param name="x">X location coordination.</param>
        /// <param name="y">Y location coordination.</param>
        /// <param name="demand"> How many capacity the client want to reserve.</param>
        /// <param name="availabilityTime">When the request is available.</param>
        /// <param name="duration">Duration of service.</param>
        public Request(double x, double y, int demand, double availabilityTime, double duration)
        {
            X = x;
            Y = y;
            Demand = demand;
            AvailabilityTime = availabilityTime;
            Duration = duration;
        }

        /// <summary>
        /// 
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Demand { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double AvailabilityTime { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public double Duration { get; private set; }
    }
}