
using Microsoft.Extensions.Caching.Memory;
using PDCore.Services.IServ;
using System;

namespace PDCore.Services.Serv
{
    public class CacheService : ICacheService
    {
        private readonly Lazy<MemoryCache> memoryCache = new Lazy<MemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));

        public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {
            if (!memoryCache.Value.TryGetValue(cacheKey, out T item))
            {
                item = getItemCallback();

                memoryCache.Value.Set(cacheKey, item, DateTime.Now.AddMinutes(10));
            }

            return item;
        }
    }
}
