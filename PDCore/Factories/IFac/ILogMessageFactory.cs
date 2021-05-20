using PDCore.Enums;
using System;

namespace PDCore.Factories.IFac
{
    public interface ILogMessageFactory
    {
        string Create(string message, Exception exception, LogType logType);
    }
}
