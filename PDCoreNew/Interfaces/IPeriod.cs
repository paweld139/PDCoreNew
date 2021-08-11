using System;

namespace PDCoreNew.Interfaces
{
    public interface IPeriod
    {
        DateTimeOffset StartDate { get; set; }

        TimeSpan Duration { get; set; }
    }
}
