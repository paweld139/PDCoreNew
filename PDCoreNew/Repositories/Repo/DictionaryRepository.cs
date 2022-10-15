using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PDCoreNew.Context.IContext;
using PDCoreNew.Entities;
using PDCoreNew.Entities.Basic;
using PDCoreNew.Entities.Briefs;
using PDCoreNew.Extensions;
using PDCoreNew.Models.Search;
using PDCoreNew.Repositories.IRepo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.Repo
{
    public class DictionaryRepository : SqlRepositoryEntityFrameworkCore<Dictionary>, IDictionaryRepository
    {
        public DictionaryRepository(IEntityFrameworkCoreDbContext ctx, ILogger<Dictionary> logger, IMapper mapper)
            : base(ctx, logger, mapper, null)
        {
        }

        public IQueryable<Dictionary> Find(SearchDictionary searchDictionary)
        {
            var query = Find(d => searchDictionary.Name == null || !searchDictionary.Name.Any() || searchDictionary.Name.Contains(d.Name));

            if (searchDictionary.OrderByKey)
                query = query.OrderBy(d => d.Key);

            if (searchDictionary.OrderByValue)
                query = query.OrderBy(d => d.Value);

            return query;
        }

        public Task<List<T>> GetAsync<T>(SearchDictionary searchDictionary)
        {
            return mapper.ProjectTo<T>(
                Find(searchDictionary)
            ).ToListAsync();
        }


        public Task<List<DictionaryBrief>> GetBriefsAsync(SearchDictionary searchDictionary)
        {
            return GetAsync<DictionaryBrief>(searchDictionary);
        }


        public Task<List<DictionaryBasic>> GetBasicAsync(SearchDictionary searchDictionary)
        {
            return GetAsync<DictionaryBasic>(searchDictionary);
        }


        public async Task<Dictionary<string, IEnumerable<KeyValuePair<string, string>>>> GetKVP(SearchDictionary searchDictionary)
        {
            var dictionaries = await GetBasicAsync(searchDictionary);

            if (dictionaries == null)
                return null;

            return dictionaries.GroupBy(d => d.Name)
                               .ToDictionary(k => k.Key, v => v.GetKVP(dk => dk.Key, dv => dv.Value));
        }

        public Task<string[]> GetValues(string dictionaryName)
        {
            var searchDictionary = new SearchDictionary(name: dictionaryName);

            var query = Find(searchDictionary);

            var valuesQuery = query.Select(e => e.Value);

            return valuesQuery.ToArrayAsync();
        }
    }
}
