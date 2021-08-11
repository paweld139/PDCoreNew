using System;

namespace PDCoreNew.Services.Serv.Time
{
    public class TimeServiceStub : TimeService
    {
        public TimeServiceStub()
        {
            Now = base.Now;
        }

        public override DateTime Now { get; protected set; }

        public override void Sleep(TimeSpan timeout)
        {
            Now += timeout;
        }
    }
}
