using System;

namespace PDCoreNew.Interfaces
{
    public interface IExportable
    {
        DateTime? ExportDate { get; set; }

        long Bytes { get; set; }
    }
}
