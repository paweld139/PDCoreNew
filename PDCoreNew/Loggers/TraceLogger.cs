using PDCoreNew.Factories.IFac;
using System;
using System.Diagnostics;

namespace PDCoreNew.Loggers
{
    public class TraceLogger : StringLogger
    {
        public TraceLogger(ILogMessageFactory logMessageFactory) : base(logMessageFactory)
        {

        }

        protected override Action<string> Print => m => Trace.WriteLine(m);
    }
}
