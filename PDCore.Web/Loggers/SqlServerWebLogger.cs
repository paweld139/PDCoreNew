using PDCore.Common.Loggers.Async;
using PDCore.Enums;
using PDCore.Models;
using PDCore.Repositories.IRepo;
using System;
using System.Web;

namespace PDCore.Web.Loggers
{
    public class SqlServerWebLogger : SqlServerLogger
    {
        public SqlServerWebLogger(ISqlRepositoryEntityFrameworkAsync<LogModel> logRepository) : base(logRepository)
        {

        }

        protected override LogModel GetLogModel(string message, LogType logType, Exception exception)
        {
            return new LogModel(message, logType, HttpContext.Current?.Request.Url.AbsoluteUri, exception);
        }
    }
}
