using System;

namespace PDCore.Interfaces
{
    public interface IPeriod
    {
        DateTimeOffset StartDate { get; set; }

        TimeSpan Duration { get; set; }
    }
}
