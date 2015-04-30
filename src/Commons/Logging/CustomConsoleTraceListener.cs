﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace _15pl04.Ucc.Commons.Logging
{
    public class CustomConsoleTraceListener : ConsoleTraceListener
    {
        // contains pairs <attributeName, initAction(attributeValue)>
        private static readonly Dictionary<string, Action<string>> _customAttributes = new Dictionary<string, Action<string>>()
        {
            { "printDate", val => bool.TryParse(val, out _printDate) },
            { "printSource", val => bool.TryParse(val, out _printSource) },
            { "printCallerInfo", val => bool.TryParse(val, out _printCallerInfo) },
        };


        protected bool PrintDate
        {
            get
            {
                EnsureCustomAttributesInitialized();
                return _printDate;
            }
        }

        protected bool PrintSource
        {
            get
            {
                EnsureCustomAttributesInitialized();
                return _printSource;
            }
        }
        protected bool PrintCallerInfo
        {
            get
            {
                EnsureCustomAttributesInitialized();
                return _printCallerInfo;
            }
        }

        private static bool _customAttributesInitialized;
        private static bool _printDate;
        private static bool _printSource;
        private static bool _printCallerInfo;



        public CustomConsoleTraceListener()
            : base()
        {
        }

        public CustomConsoleTraceListener(bool useErrorStream)
            : base(useErrorStream)
        {
        }



        protected override string[] GetSupportedAttributes()
        {
            return _customAttributes.Keys.ToArray();
        }

        // TODO: override methods to customize output...



        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEvent(eventCache, source, eventType, id, "");
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            string message = "";
            try
            {
                message = string.Format(format, args);
            }
            catch (Exception e)
            {
                message = string.Format("Exception during formatting [{0}]; exception message: {1}", format, e.Message);
            }
            TraceEvent(eventCache, source, eventType, id, message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            Console.ForegroundColor = GetConsoleColor(eventType);

            var outputs = new List<string>();
            if (PrintDate && eventCache != null)
                outputs.Add(eventCache.DateTime.ToString());
            if (PrintSource)
                outputs.Add(string.Format("[{0}]", source));
            if (PrintCallerInfo)
                outputs.Add(GetCallerInfoPrefix());
            if (!string.IsNullOrWhiteSpace(message))
                outputs.Add(message);
            Console.WriteLine(string.Join(" ", outputs));

            Console.ResetColor();
        }

        private static ConsoleColor GetConsoleColor(TraceEventType eventType)
        {
            ConsoleColor color;
            switch (eventType)
            {
                case TraceEventType.Verbose:
                    color = ConsoleColor.DarkGray;
                    break;
                case TraceEventType.Information:
                    color = ConsoleColor.White;
                    break;
                case TraceEventType.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case TraceEventType.Error:
                case TraceEventType.Critical:
                    color = ConsoleColor.Red;
                    break;
                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Resume:
                case TraceEventType.Suspend:
                case TraceEventType.Transfer:
                    color = ConsoleColor.DarkGreen;
                    break;
                default:
                    color = ConsoleColor.DarkRed;
                    break;
            }
            return color;
        }

        private void EnsureCustomAttributesInitialized()
        {
            if (_customAttributesInitialized)
                return;
            foreach (var keyValuePair in _customAttributes)
            {
                if (Attributes.ContainsKey(keyValuePair.Key))
                {
                    keyValuePair.Value(Attributes[keyValuePair.Key]);
                }
            }
            _customAttributesInitialized = true;
        }

        private string GetCallerInfoPrefix()
        {
            StackFrame frame = new StackFrame(4);
            var method = frame.GetMethod();

            string className = method.DeclaringType.Name;
            string methodName = method.Name;

            return "[" + className + "/" + methodName + "]::";
        }
    }
}
