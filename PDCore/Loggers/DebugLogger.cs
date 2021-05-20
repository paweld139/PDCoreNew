using PDCore.Factories.IFac;
using System;

namespace PDCore.Loggers
{
    public class DebugLogger : StringLogger
    {
        public DebugLogger(ILogMessageFactory logMessageFactory) : base(logMessageFactory)
        {
        }

        protected override Action<string> Print => m => System.Diagnostics.Debug.WriteLine(m);
    }
}
