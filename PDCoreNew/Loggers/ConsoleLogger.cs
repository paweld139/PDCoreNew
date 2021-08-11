using PDCoreNew.Factories.IFac;
using System;

namespace PDCoreNew.Loggers
{
    public class ConsoleLogger : StringLogger
    {
        public ConsoleLogger(ILogMessageFactory logMessageFactory) : base(logMessageFactory)
        {

        }

        protected override Action<string> Print => Console.WriteLine;
    }
}
