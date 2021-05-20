using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDCore.Services.IServ
{
    public interface IElasticSearchService<TItem, TKey, TCriteria> where TItem : class
    {
        Task AddBulkAsync();
        Task AddBulkAsync(IEnumerable<TItem> items);
        Task AddBulkAsync(IEnumerable<TKey> itemIds);
        Task AddBulkAsync(params TKey[] itemIds);
        Task AddSingleAsync(TItem item);
        Task AddSingleAsync(TKey key);
        void CreateIndex();
        Task DeleteAll();
        Task DeleteBulkAsync(IEnumerable<TItem> items, Func<TItem, TKey> keyFunc);
        Task DeleteBulkAsync(IEnumerable<TKey> itemIds);
        Task DeleteBulkAsync(params TKey[] itemIds);
        void DeleteIndex();
        Task DeleteSingleAsync(TItem item);
        Task DeleteSingleAsync(TKey key);
        Task RecerateAndFillIndex();
        void RecreateIndex();
        Task Reindex();
        Task ReindexSingleAsync(TKey key);
        Task SaveSingleAsync(TItem item);
        Task SaveSingleAsync(TKey key);
        Task<TKey[]> Search(QueryContainer queryContainer);
        Task<TKey[]> Search(string query, int page, int pageSize);
        Task<TKey[]> Search(TCriteria criteria);
        Task UpdateBulkAsync();
        Task UpdateBulkAsync(IEnumerable<TItem> items);
        Task UpdateBulkAsync(IEnumerable<TKey> itemIds);
        Task UpdateBulkAsync(params TKey[] itemIds);
        Task UpdateSingleAsync(TItem item);
        Task UpdateSingleAsync(TKey key);
    }
}