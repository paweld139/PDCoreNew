using PDCoreNew.Entities;
using PDCoreNew.Entities.Basic;
using PDCoreNew.Entities.Briefs;
using PDCoreNew.Models.Search;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.IRepo
{
    public interface IDictionaryRepository
    {
        IQueryable<Dictionary> Find(SearchDictionary searchDictionary);

        Task<List<T>> GetAsync<T>(SearchDictionary searchDictionary);

        Task<List<DictionaryBasic>> GetBasicAsync(SearchDictionary searchDictionary);

        Task<List<DictionaryBrief>> GetBriefsAsync(SearchDictionary searchDictionary);

        Task<Dictionary<string, IEnumerable<KeyValuePair<string, string>>>> GetKVP(SearchDictionary searchDictionary);
    }
}
