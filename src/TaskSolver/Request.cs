using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// Class representing a client's request.
    /// </summary>
    [Serializable]
    public class Request
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="demand">How many load capacity the client "orders".</param>
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
        /// X coordinate.
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// Y coordinate.
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// How much capacity the client requests. The value should be negative.
        /// </summary>
        public int Demand { get; private set; }
        /// <summary>
        /// When the request is available; in minutes.
        /// </summary>
        public double AvailabilityTime { get; internal set; }
        /// <summary>
        /// Duration of service; in minutes.
        /// </summary>
        public double Duration { get; private set; }
    }
}