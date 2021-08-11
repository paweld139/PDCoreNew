using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDCoreNew.Interfaces
{
    public interface IParser<T>
    {
        IList<T> Parse();

        Task<IList<T>> ParseAsyncTask();
    }
}
