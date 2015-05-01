using System;
using System.Threading;
using _15pl04.Ucc.Commons.Computations.Base;

namespace _15pl04.Ucc.Commons.Computations
{
    public class ThreadPoolThreadManager : ThreadManager
    {
        public ThreadPoolThreadManager()
            : base((byte)Environment.ProcessorCount)
        {
        }


        protected override bool StartInNewThread(Action actionToExecute)
        {
            return ThreadPool.QueueUserWorkItem(arg => actionToExecute());
        }
    }
}
