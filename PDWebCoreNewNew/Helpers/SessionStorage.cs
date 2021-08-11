using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PDCoreNew.Interfaces;
using PDWebCoreNewNew.Extensions;
using System;
using System.Threading.Tasks;

namespace PDWebCoreNewNew.Helpers
{
    public class SessionStorage : ISessionStorage
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public SessionStorage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext;

        private ISession Session => HttpContext?.Session;

        public void SetString(string key, string value) => Session?.SetString(key, value);

        public string GetString(string key) => HttpContext.GetString(key);

        public string GetString(string key, string defaultValue) => GetString(key) ?? defaultValue;

        public void Remove(string key) => Session?.Remove(key);

        public void SetObject(string key, object value) => SetString(key, JsonConvert.SerializeObject(value));

        public T GetObject<T>(string key) => HttpContext.GetObject<T>(key);

        public T GetObject<T>(string key, Func<T> defaultValue)
        {
            var value = GetObject<T>(key);

            return value == null ? defaultValue() : value;
        }

        public T GetObject<T>(string key, Func<Task<T>> defaultValue)
        {
            var value = GetObject<T>(key);

            return value == null ? defaultValue().Result : value;
        }

        public ISessionStorage<T> ForType<T>(string sessionKey)
        {
            return new SessionStorage<T>(_httpContextAccessor, sessionKey);
        }
    }

    public class SessionStorage<T> : SessionStorage, ISessionStorage<T>
    {
        private readonly string _sessionKey;

        public SessionStorage(IHttpContextAccessor httpContextAccessor, string sessionKey)
            : base(httpContextAccessor)
        {
            _sessionKey = sessionKey;
        }

        public T Get(Func<T> initialValue)
        {
            var value = GetObject<T>(_sessionKey);

            return value == null ? initialValue() : value;
        }

        public void Set(T value) => SetObject(_sessionKey, value);

        public void Remove() => Remove(_sessionKey);
    }
}
