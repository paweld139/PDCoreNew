using PDCore.Models;
using System.Data.Entity;

namespace PDCore.Common.Context.IContext
{
    public interface IHasLogDbSet
    {
        DbSet<LogModel> ErrorLog { get; set; }
    }
}
