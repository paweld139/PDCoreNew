using PDCoreNew.Enums;
using System;

namespace PDCoreNew.Interfaces
{
    public interface ILogger
    {
        void Debug(string message, Exception exception);

        void Error(string message, Exception exception);

        void Fatal(string message, Exception exception);

        void Info(string message, Exception exception);

        void Warn(string message, Exception exception);


        void Debug(string message);

        void Error(string message);

        void Fatal(string message);

        void Info(string message);

        void Warn(string message);


        void Log(string message, Exception exception, LogType logType);


        void Log(Exception exception, LogType logType);

        void Log(string message, LogType logType);


        void Log(string message);

        void Log(Exception exception);
    }
}
