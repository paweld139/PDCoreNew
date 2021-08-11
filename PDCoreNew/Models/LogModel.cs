using PDCoreNew.Entities.Basic;
using PDCoreNew.Enums;
using PDCoreNew.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDCoreNew.Models
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
