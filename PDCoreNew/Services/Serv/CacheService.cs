
using Microsoft.Extensions.Caching.Memory;
using PDCoreNew.Services.IServ;
using System;

namespace PDCoreNew.Services.Serv
{
    public class CacheService : ICacheService
    {
        private readonly Lazy<MemoryCache> memoryCache = new(() => new MemoryCache(new MemoryCacheOptions()));

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
