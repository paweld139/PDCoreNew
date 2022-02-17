using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PDCoreNew.Context.IContext;
using PDCoreNew.Entities.Briefs;
using PDCoreNew.Extensions;
using PDCoreNew.Repositories.IRepo;
using PDCoreNew.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.Repo
{
    public class ConfigurationRepository<TEnum> : SqlRepositoryEntityFrameworkCore<Entities.Configuration<TEnum>>, IConfigurationRepository<TEnum> where TEnum : struct
    {
        public ConfigurationRepository(IEntityFrameworkCoreDbContext ctx, ILogger<Entities.Configuration<TEnum>> logger,
            IMapper mapper) : base(ctx, logger, mapper, null)
        {
        }

        private static string[] GetConfigurationTagsArray(IEnumerable<TEnum> configurationTags)
        {
            return configurationTags.ToArrayString();
        }

        private static string GetKey(TEnum configurationTag) => configurationTag.ToString();


        public IQueryable<Entities.Configuration<TEnum>> Find(IEnumerable<string> keys)
        {
            return Find(c => keys.Contains(c.Key));
        }

        public IQueryable<Entities.Configuration<TEnum>> Find(params string[] keys)
        {
            return Find(keys.AsEnumerable());
        }


        public IQueryable<Entities.Configuration<TEnum>> Find(IEnumerable<TEnum> configurationTags)
        {
            var keys = GetConfigurationTagsArray(configurationTags);

            return Find(keys);
        }

        public IQueryable<Entities.Configuration<TEnum>> Find(params TEnum[] configurationTags)
        {
            return Find(configurationTags.AsEnumerable());
        }


        public Task<Entities.Configuration<TEnum>> Get(TEnum configurationTag, CancellationToken cancellationToken)
        {
            return Find(configurationTag).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<Entities.Configuration<TEnum>> Get(TEnum configurationTag)
        {
            return Get(configurationTag, CancellationToken.None);
        }


        public Task<List<Entities.Configuration<TEnum>>> Get(IEnumerable<TEnum> configurationTags)
        {
            var query = Find(configurationTags);

            return query.ToListAsync();
        }

        public Task<List<Entities.Configuration<TEnum>>> Get(params TEnum[] configurationTags)
        {
            return Get(configurationTags.AsEnumerable());
        }


        public Task<Dictionary<TEnum, T>> GetDictionary<T>(params TEnum[] configurationTags)
        {
            var query = Find(configurationTags);

            return query.ToDictionaryAsync(c => c.Key.ParseEnum<TEnum>(), c => c.Value.ConvertObject<T>());
        }


        public async Task<dynamic> GetExpandoObject<T>(T model)
        {
            var properties = ReflectionUtils.GetObjectPropertyDictionary(model);

            var propertyNames = properties.Keys;

            var query = Find(propertyNames);

            var dictionary = await query.ToDictionaryAsync(c => c.Key, c => c.Value.ConvertObject(properties[c.Key].GetType()));

            ExpandoObject expandoObject = new();

            return dictionary.Aggregate(new ExpandoObject() as IDictionary<string, object>, (a, p) => { a.Add(p.Key, p.Value); return a; });
        }


        public async Task FillModel<T>(T model)
        {
            var properties = model.GetProperties();

            var propertyNames = properties.GetPropertyNames();

            var query = Find(propertyNames);

            var dictionary = await query.ToDictionaryAsync(c => c.Key, c => c.Value);

            foreach (var item in properties)
            {
                var value = dictionary[item.Name].ConvertObject(item.PropertyType);

                item.SetValue(model, value);
            }
        }


        public async Task<T> GetValueByKey<T>(string key, CancellationToken cancellationToken, T defaultValue = default)
        {
            string value = await Find(key).Select(c => c.Value).SingleOrDefaultAsync(cancellationToken);

            return value == null ? defaultValue : value.ConvertObject<T>();
        }

        public Task<T> GetValueByKey<T>(string key, T defaultValue = default)
        {
            return GetValueByKey(key, CancellationToken.None, defaultValue);
        }


        public Task<T> GetValue<T>(TEnum configurationTag, CancellationToken cancellationToken, T defaultValue = default)
        {
            string key = GetKey(configurationTag);

            return GetValueByKey(key, cancellationToken, defaultValue);
        }

        public Task<T> GetValue<T>(TEnum configurationTag, T defaultValue = default)
        {
            return GetValue(configurationTag, CancellationToken.None, defaultValue);
        }


        public Task<Dictionary<TEnum, string>> GetDictionary(params TEnum[] configurationTags)
        {
            return GetDictionary<string>(configurationTags);
        }


        public Task<string> GetValueByKey(string key, CancellationToken cancellationToken, string defaultValue = null)
        {
            return GetValueByKey<string>(key, cancellationToken, defaultValue);
        }

        public Task<string> GetValueByKey(string key, string defaultValue = null)
        {
            return GetValueByKey(key, CancellationToken.None, defaultValue);
        }


        public Task<string> GetValue(TEnum configurationTag, CancellationToken cancellationToken, string defaultValue = null)
        {
            return GetValue<string>(configurationTag, cancellationToken, defaultValue);
        }

        public Task<string> GetValue(TEnum configurationTag, string defaultValue = null)
        {
            return GetValue(configurationTag, CancellationToken.None, defaultValue);
        }


        public Task<bool> GetBool(TEnum configurationTag, CancellationToken cancellationToken, bool defaultValue = false)
        {
            return GetValue(configurationTag, cancellationToken, defaultValue);
        }

        public Task<bool> GetBool(TEnum configurationTag, bool defaultValue = false)
        {
            return GetValue(configurationTag, CancellationToken.None, defaultValue);
        }


        public async Task<T> GetObject<T>(TEnum configurationTag, CancellationToken cancellationToken, T defaultValue = default)
        {
            T result = defaultValue;

            string value = await GetValue(configurationTag, cancellationToken, null);

            if (value != null)
            {
                result = JsonConvert.DeserializeObject<T>(value);
            }

            return result;
        }

        public Task<T> GetObject<T>(TEnum configurationTag, T defaultValue = default)
        {
            return GetObject(configurationTag, CancellationToken.None, defaultValue);
        }


        public Task<bool> ExistsAsync(string key)
        {
            return Find(key).AnyAsync();
        }


        public async Task<bool> SaveAddedOrUpdated(ConfigurationBrief<TEnum> input, IPrincipal principal,
            Action<string, string> writeError, Action<string> cleanRowVersion)
        {
            bool exists = await ExistsAsync(input.Key);

            bool success = true;

            if (!exists)
            {
                await SaveNewAsync(input);
            }
            else
            {
                success = await SaveUpdatedWithOptimisticConcurrencyAsync(input, principal, writeError, cleanRowVersion);
            }

            return success;
        }

        public Task<bool> SaveAddedOrUpdated<T>(T model, IPrincipal principal, Action<string, string> writeError,
            Action<string> cleanRowVersion, TEnum configurationTag)
        {
            ConfigurationBrief<TEnum> input = ConfigurationBrief<TEnum>.Create(configurationTag, model);

            return SaveAddedOrUpdated(input, principal, writeError, cleanRowVersion);
        }
    }
}
