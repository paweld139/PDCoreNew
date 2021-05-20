using PDCore.Factories.IFac;
using System;

namespace PDCore.Loggers
{
    public class ConsoleLogger : StringLogger
    {
        public ConsoleLogger(ILogMessageFactory logMessageFactory) : base(logMessageFactory)
        {

        }

        protected override Action<string> Print => Console.WriteLine;
    }
}
