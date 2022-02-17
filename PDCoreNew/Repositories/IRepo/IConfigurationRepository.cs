using PDCoreNew.Entities.Briefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.IRepo
{
    public interface IConfigurationRepository<TEnum> : ISqlRepositoryEntityFrameworkAsync<Entities.Configuration<TEnum>> where TEnum : struct
    {
        Task<bool> ExistsAsync(string key);
        Task FillModel<T>(T model);
        IQueryable<Entities.Configuration<TEnum>> Find(IEnumerable<TEnum> configurationTags);
        IQueryable<Entities.Configuration<TEnum>> Find(IEnumerable<string> keys);
        IQueryable<Entities.Configuration<TEnum>> Find(params TEnum[] configurationTags);
        IQueryable<Entities.Configuration<TEnum>> Find(params string[] keys);
        Task<Entities.Configuration<TEnum>> Get(TEnum configurationTag);
        Task<Entities.Configuration<TEnum>> Get(TEnum configurationTag, CancellationToken cancellationToken);
        Task<List<Entities.Configuration<TEnum>>> Get(IEnumerable<TEnum> configurationTags);
        Task<List<Entities.Configuration<TEnum>>> Get(params TEnum[] configurationTags);
        Task<bool> GetBool(TEnum configurationTag, bool defaultValue = false);
        Task<bool> GetBool(TEnum configurationTag, CancellationToken cancellationToken, bool defaultValue = false);
        Task<Dictionary<TEnum, string>> GetDictionary(params TEnum[] configurationTags);
        Task<Dictionary<TEnum, T>> GetDictionary<T>(params TEnum[] configurationTags);
        Task<dynamic> GetExpandoObject<T>(T model);
        Task<T> GetObject<T>(TEnum configurationTag, CancellationToken cancellationToken, T defaultValue = default);
        Task<T> GetObject<T>(TEnum configurationTag, T defaultValue = default);
        Task<string> GetValue(TEnum configurationTag, CancellationToken cancellationToken, string defaultValue = null);
        Task<string> GetValue(TEnum configurationTag, string defaultValue);
        Task<T> GetValue<T>(TEnum configurationTag, T defaultValue = default);
        Task<T> GetValue<T>(TEnum configurationTag, CancellationToken cancellationToken, T defaultValue = default);
        Task<string> GetValueByKey(string key, CancellationToken cancellationToken, string defaultValue = null);
        Task<string> GetValueByKey(string key, string defaultValue);
        Task<T> GetValueByKey<T>(string key, T defaultValue = default);
        Task<T> GetValueByKey<T>(string key, CancellationToken cancellationToken, T defaultValue = default);
        Task<bool> SaveAddedOrUpdated(ConfigurationBrief<TEnum> input, IPrincipal principal, Action<string, string> writeError, Action<string> cleanRowVersion);
        Task<bool> SaveAddedOrUpdated<T>(T model, IPrincipal principal, Action<string, string> writeError, Action<string> cleanRowVersion, TEnum configurationTag);
    }
}
