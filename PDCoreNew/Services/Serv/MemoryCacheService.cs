using Microsoft.Extensions.Caching.Memory;
using PDCoreNew.Services.IServ;
using System;

namespace PDCoreNew.Services.Serv
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly Lazy<MemoryCache> memoryCache = new(() => new MemoryCache(new MemoryCacheOptions()));

        public MemoryCache MemoryCache => memoryCache.Value;
    }
}
