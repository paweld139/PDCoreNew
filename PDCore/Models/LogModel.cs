using PDCore.Enums;
using PDCore.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDCore.Models
{
    [Table("Log")]
    public class LogModel : IModificationHistory
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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ELId { get; set; }

        public string ErrorType { get; set; }

        public string Message { get; set; }

        public string ErrorMessage { get; set; }

        public string StackTrace { get; set; }

        public LogType LogLevel { get; set; }

        public string RequestUri { get; set; }

        public string MachineName { get; set; }


        public DateTime DateModified { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDirty { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
