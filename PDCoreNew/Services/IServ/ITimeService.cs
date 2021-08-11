using System;

namespace PDCoreNew.Services.IServ
{
    public interface ITimeService
    {
        DateTime Now { get; }

        void Sleep(TimeSpan timeout);

        void Sleep(int millisecondsTimeout);
    }
}
