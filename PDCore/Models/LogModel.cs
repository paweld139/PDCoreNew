using PDCore.Entities.Basic;
using PDCore.Enums;
using PDCore.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDCore.Models
{
    [Table("Log")]
    public class LogModel : LogBasic, IModificationHistory
    {
        public LogModel(string message, LogType logType, string requestUri, Exception exception = null)
        {
            Message = message;

            if (exception != null)
            {
                ErrorType = exception.GetType().Name;
                StackTrace = exception.StackTrace;
                ErrorMessage = exception.Message;
            }

            MachineName = Environment.MachineName;
            LogLevel = logType;
            RequestUri = requestUri;
        }

        public LogModel() { }

        public bool IsDirty { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
