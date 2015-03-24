using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15pl04.Ucc.Commons
{
    public class TimeoutException : Exception
    {
        public TimeoutException()
            : base()
        {
        }

        public TimeoutException(string message)
            : base(message)
        {
        }

        public TimeoutException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
