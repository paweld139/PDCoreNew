using PDCoreNew.Handlers;
using System.Threading.Tasks;

namespace PDCoreNew.Contracts
{
    public interface IHandlerAsync<T> : IHandler<T> where T : class
    {
        ValueTask HandleAsync(T request);

        IHandlerAsync<T> SetNextAsync(IHandlerAsync<T> nextAsync);
    }
}
