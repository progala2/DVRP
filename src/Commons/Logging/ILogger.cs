﻿
namespace _15pl04.Ucc.Commons.Logging
{
    public interface ILogger
    {
        void Trace(string s);

        void Debug(string s);

        void Info(string s);

        void Warn(string s);

        void Error(string s);
    }
}