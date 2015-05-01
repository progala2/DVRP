
using _15pl04.Ucc.Commons.Computations.Base;

namespace _15pl04.Ucc.Commons.Computations
{
    public class ThreadPoolThreadManagerFactory : IThreadManagerFactory
    {
        #region IThreadManagerFactory Members

        public ThreadManager CreateThreadManager()
        {
            return new ThreadPoolThreadManager();
        }

        #endregion
    }
}
