using System.Diagnostics;

namespace _15pl04.Ucc.Commons.Logging
{
    public class TraceSourceLogger : ILogger
    {
        public TraceSourceLogger(string name)
        {
            TraceSource = new TraceSource(name);
        }

        public TraceSourceLogger(string name, SourceLevels defaultLevel)
        {
            TraceSource = new TraceSource(name, defaultLevel);
        }

        public TraceSource TraceSource { get; private set; }

        public void Trace(string s)
        {
            TraceSource.TraceEvent(TraceEventType.Verbose, 0, s);
        }

        public void Debug(string s)
        {
            TraceSource.TraceEvent(TraceEventType.Verbose, 0, s);
        }

        public void Info(string s)
        {
            TraceSource.TraceEvent(TraceEventType.Information, 0, s);
        }

        public void Warn(string s)
        {
            TraceSource.TraceEvent(TraceEventType.Warning, 0, s);
        }

        public void Error(string s)
        {
            TraceSource.TraceEvent(TraceEventType.Error, 0, s);
        }
    }
}