using Microsoft.Extensions.Caching.Memory;

namespace PDCoreNew.Services.IServ
{
    public interface IMemoryCacheService
    {
        MemoryCache MemoryCache { get; }
    }
}
