using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace PDCore.Web.Handlers.Loggers
{
    /// <summary>
    /// Dostęp do wszystkich wyjątków
    /// </summary>
    public class TraceExceptionLogger : ExceptionLogger
    {
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            Trace.TraceError(context.ExceptionContext.Exception.ToString());

            return Task.CompletedTask;
        }
    }
}
