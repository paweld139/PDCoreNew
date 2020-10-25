using PDCore.Enums;
using PDCore.Web.Helpers.ExceptionHandling;
using System.Web.Mvc;

namespace PDCore.Web.Filters.MVC
{
    public class LogExceptionFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            HttpApplicationErrorHandler.HandleException(context.Exception, LogType.Error);
        }
    }
}
