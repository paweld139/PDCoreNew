using PDCore.Models;
using System.Data.Entity;

namespace PDCore.Common.Context.IContext
{
    public interface IHasFileDbSet
    {
        DbSet<FileModel> File { get; set; }
    }
}
