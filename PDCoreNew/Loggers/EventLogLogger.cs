using PDCoreNew.Enums;
using PDCoreNew.Factories.IFac;
using System;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace PDCoreNew.Loggers
{
    [SupportedOSPlatform("windows")]
    public class EventLogLogger : Logger, IDisposable
    {
        private readonly ILogMessageFactory logMessageFactory;
        private readonly EventLog eventLog;

        public EventLogLogger(ILogMessageFactory logMessageFactory)
        {
            this.logMessageFactory = logMessageFactory;

            eventLog = new EventLog("Application")
            {
                Source = "Application"
            };
        }

        public override void Log(string message, Exception exception, LogType logType)
        {
            var eventLogType = GetEventLogEntryType(logType);

            string info = logMessageFactory.Create(message, exception, logType);

            eventLog.WriteEntry(info, eventLogType);
        }

        private static EventLogEntryType GetEventLogEntryType(LogType logType)
        {
            return logType switch
            {
                LogType.Error or LogType.Fatal => EventLogEntryType.Error,
                LogType.Info or LogType.Debug => EventLogEntryType.Information,
                LogType.Warn => EventLogEntryType.Warning,
                _ => EventLogEntryType.Information,
            };
        }

        public void Dispose()
        {
            eventLog.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
