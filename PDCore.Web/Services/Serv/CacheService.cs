using PDCore.Services.IServ;
using System;
using System.Web;
using System.Web.Caching;

namespace PDCore.Web.Services.Serv
{
    public class CacheService : ICacheService
    {
        public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {
            if (!(HttpContext.Current.Cache.Get(cacheKey) is T item))
            {
                item = getItemCallback();

                HttpContext.Current.Cache.Insert(cacheKey, item, null, DateTime.UtcNow.AddDays(1), Cache.NoSlidingExpiration);
            }

            return item;
        }
    }
}
