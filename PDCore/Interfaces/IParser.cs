using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDCore.Interfaces
{
    public interface IParser<T>
    {
        IList<T> Parse();

        Task<IList<T>> ParseAsyncTask();
    }
}
