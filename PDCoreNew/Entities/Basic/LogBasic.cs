using PDCoreNew.Entities.Briefs;
using PDCoreNew.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDCoreNew.Entities.Basic
{
    public class LogBasic : LogBrief
    {
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
    }
}
