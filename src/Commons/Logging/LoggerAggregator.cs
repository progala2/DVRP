using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.Commons.Logging
{
    public sealed class LoggerAggregator : ILogger
    {
        private static readonly Lazy<LoggerAggregator> lazy =
            new Lazy<LoggerAggregator>(() => new LoggerAggregator());

        private readonly List<ILogger> _loggers = new List<ILogger>();

        private readonly ConcurrentQueue<Tuple<LogLevel, string>> _logQueue =
            new ConcurrentQueue<Tuple<LogLevel, string>>();

        private readonly AutoResetEvent _logsAvailable = new AutoResetEvent(false);

        private LoggerAggregator()
        {
            new Task(() =>
            {
                while (true)
                {
                    Tuple<LogLevel, string> tuple;

                    if (!_logQueue.TryDequeue(out tuple))
                        _logsAvailable.WaitOne();

                    foreach (var l in _loggers)
                    {
                        switch (tuple.Item1)
                        {
                            case LogLevel.Trace:
                                l.Trace(tuple.Item2);
                                continue;
                            case LogLevel.Debug:
                                l.Debug(tuple.Item2);
                                continue;
                            case LogLevel.Info:
                                l.Info(tuple.Item2);
                                continue;
                            case LogLevel.Warn:
                                l.Warn(tuple.Item2);
                                continue;
                            case LogLevel.Error:
                                l.Error(tuple.Item2);
                                continue;
                        }
                    }
                }
            }).Start();
        }

        public static LoggerAggregator Instance
        {
            get { return lazy.Value; }
        }

        public void Trace(string s)
        {
            EnqueueLog(LogLevel.Trace, s);
        }

        public void Debug(string s)
        {
            EnqueueLog(LogLevel.Debug, s);
        }

        public void Info(string s)
        {
            EnqueueLog(LogLevel.Info, s);
        }

        public void Warn(string s)
        {
            EnqueueLog(LogLevel.Warn, s);
        }

        public void Error(string s)
        {
            EnqueueLog(LogLevel.Error, s);
        }

        public void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        private void EnqueueLog(LogLevel level, string log)
        {
            var tuple = new Tuple<LogLevel, string>(level, log);

            _logQueue.Enqueue(tuple);
            _logsAvailable.Set();
        }
    }
}