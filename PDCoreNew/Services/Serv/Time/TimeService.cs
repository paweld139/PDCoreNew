using PDCoreNew.Services.IServ;
using PDCoreNew.Utils;
using System;
using System.Threading;

namespace PDCoreNew.Services.Serv.Time
{
    public class TimeService : ITimeService
    {
        public virtual DateTime Now { get; protected set; } = DateTime.Now;

        public virtual void Sleep(TimeSpan timeout) => Thread.Sleep(timeout);

        public void Sleep(int millisecondsTimeout)
        {
            TimeSpan timeSpan = DateTimeUtils.GetTimeSpan(millisecondsTimeout);

            Sleep(timeSpan);
        }
    }
}
