using PDCoreNew.Factories.IFac;
using PDCoreNew.Helpers.DataStructures.Buffer;
using System;

namespace PDCoreNew.Loggers
{
    public class InMemoryLogger : StringLogger
    {
        public InMemoryLogger(ILogMessageFactory logMessageFactory) : base(logMessageFactory)
        {

        }

        private static readonly Lazy<Buffer<string>> logs = new();

        public static IBuffer<string> Logs => logs.Value;

        protected override Action<string> Print => m => Logs.Write(m);
    }
}
