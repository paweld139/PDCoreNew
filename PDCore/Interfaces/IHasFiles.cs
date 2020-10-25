using PDCore.Models;
using System.Collections.Generic;

namespace PDCore.Interfaces
{
    public interface IHasFiles
    {
        ICollection<FileModel> Files { get; set; }
    }
}
