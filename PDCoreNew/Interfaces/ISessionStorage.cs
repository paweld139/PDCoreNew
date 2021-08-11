using System;
using System.Threading.Tasks;

namespace PDCoreNew.Interfaces
{
    public interface ISessionStorage
    {
        void SetString(string key, string value);

        string GetString(string key);
        string GetString(string key, string defaultValue);

        void Remove(string key);

        void SetObject(string key, object value);

        T GetObject<T>(string key);
        T GetObject<T>(string key, Func<T> defaultValue);
        T GetObject<T>(string key, Func<Task<T>> defaultValue);

        ISessionStorage<T> ForType<T>(string sessionKey);
    }

    public interface ISessionStorage<T>
    {
        void Set(T value);
        T Get(Func<T> initialValue);
        void Remove();
    }
}
