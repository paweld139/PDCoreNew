using PDCoreNew.Entities.Briefs;
using PDCoreNew.Enums;
using System;
using System.Collections.Generic;

namespace PDCoreNew.Entities.DTO
{
    public class LogDTO : LogBrief, IEquatable<LogDTO>
    {
        public LogType? LogType { get; set; }

        public DateTime DateCreatedTo { get; set; }

        public LogDTO()
        {
            DateCreated = DateTime.Today.AddDays(-1);
            DateCreatedTo = DateTime.Today;
        }

        public static LogDTO Default() => new();

        public bool IsDefault() => this == Default();

        public override bool Equals(object obj)
        {
            return Equals(obj as LogDTO);
        }

        public bool Equals(LogDTO other)
        {
            return other != null &&
                   DateCreated == other.DateCreated &&
                   EqualityComparer<LogType?>.Default.Equals(LogType, other.LogType) &&
                   DateCreatedTo == other.DateCreatedTo;
        }

        public override int GetHashCode()
        {
            int hashCode = 1390323850;
            hashCode = hashCode * -1521134295 + DateCreated.GetHashCode();
            hashCode = hashCode * -1521134295 + LogType.GetHashCode();
            hashCode = hashCode * -1521134295 + DateCreatedTo.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(LogDTO left, LogDTO right)
        {
            return EqualityComparer<LogDTO>.Default.Equals(left, right);
        }

        public static bool operator !=(LogDTO left, LogDTO right)
        {
            return !(left == right);
        }
    }
}
