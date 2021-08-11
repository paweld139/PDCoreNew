using PDCoreNew.Helpers.Calculation;
using System;
using System.Runtime.Versioning;

namespace PDCoreNew.Services.Serv.Time
{
    [SupportedOSPlatform("windows")]
    public class AA
    {
        public DateTime When { get; private set; }

        public AA()
        {
            When = PreciseDatetime.Now;
        }
    }
}
