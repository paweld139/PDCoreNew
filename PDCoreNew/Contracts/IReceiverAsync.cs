using PDCoreNew.Handlers;
using System.Threading.Tasks;

namespace PDCoreNew.Contracts
{
    public interface IReceiverAsync<T> : IReceiver<T> where T : class
    {
        ValueTask HandleAsync(T request);
    }
}
