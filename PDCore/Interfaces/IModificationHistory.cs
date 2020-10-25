using System;
using System.ComponentModel.DataAnnotations;

namespace PDCore.Interfaces
{
    public interface IModificationHistory : IHasRowVersion
    {
        DateTime DateModified { get; set; }

        DateTime DateCreated { get; set; }

        bool IsDirty { get; set; }
    }

    public interface IHasRowVersion
    {
        [Timestamp]
        byte[] RowVersion { get; set; }
    }
}
