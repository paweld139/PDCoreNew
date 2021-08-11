using PDCoreNew.Enums;
using System;

namespace PDCoreNew.Factories.IFac
{
    public interface ILogMessageFactory
    {
        string Create(string message, Exception exception, LogType logType);
    }
}
