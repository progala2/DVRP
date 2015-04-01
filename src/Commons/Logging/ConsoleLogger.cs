using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15pl04.Ucc.Commons.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Trace(string s)
        {
            Console.WriteLine(s);
        }

        public void Debug(string s)
        {
            Console.WriteLine(s);
        }

        public void Info(string s)
        {
            Console.WriteLine(s);
        }

        public void Warn(string s)
        {
            Console.WriteLine(s);
        }

        public void Error(string s)
        {
            Console.WriteLine(s);
        }
    }
}
