using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using PDCore.Extensions;
using PDCore.Interfaces;
using PDCore.Repositories.IRepo;
using PDCore.Services.IServ;
using PDCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PDCore.Services.Serv
{
    public abstract class ElasticSearchService<TItem, TEntity, TKey, TCriteria> :
            IElasticSearchService<TItem, TKey, TCriteria> where TEntity : class, IModificationHistory where TItem : class
    {
        private readonly ElasticClient elasticClient;
        private readonly ISqlRepositoryEntityFrameworkAsync<TEntity> sqlRepository;
        private readonly IConfiguration configuration;
        private readonly ILogger<ElasticSearchService<TItem, TEntity, TKey, TCriteria>> logger;

        public ElasticSearchService(ElasticClient elasticClient, ISqlRepositoryEntityFrameworkAsync<TEntity> sqlRepository,
            IConfiguration configuration, ILogger<ElasticSearchService<TItem, TEntity, TKey, TCriteria>> logger)
        {
            this.elasticClient = elasticClient;
            this.sqlRepository = sqlRepository;
            this.configuration = configuration;
            this.logger = logger;
        }


        protected abstract Func<ISqlRepositoryEntityFrameworkAsync<TEntity>, Func<TKey, Task<TItem>>> ItemFunc { get; }

        protected abstract Func<ISqlRepositoryEntityFrameworkAsync<TEntity>, Func<Task<List<TItem>>>> AllItemsFunc { get; }

        protected abstract Func<ISqlRepositoryEntityFrameworkAsync<TEntity>, Func<IEnumerable<TKey>, Task<List<TItem>>>> ItemsFunc { get; }

        protected abstract Func<TItem, TKey> KeyFunc { get; }

        protected abstract Func<TCriteria, string> QueryFunc { get; }

        protected virtual bool CanSearch(string searchText) => !string.IsNullOrWhiteSpace(searchText);


        public virtual Task<TKey[]> Search(TCriteria criteria)
        {
            var task = Task.FromResult<TKey[]>(null);

            var query = CreateQuery(criteria);

            if (query != null)
                task = Search(query);

            return task;
        }

        protected virtual QueryContainer CreateQuery(TCriteria criteria)
        {
            QueryContainer queryContainer = null;

            string searchText = QueryFunc(criteria);

            if (CanSearch(searchText))
            {
                queryContainer = GetQueryContainer(criteria, searchText);
            }

            return queryContainer;
        }


        protected virtual QueryContainer GetQueryContainer(Func<QueryContainerDescriptor<TItem>, QueryContainer>[] queries)
        {
            return Query<TItem>.Bool(b => b.Should(queries));
        }

        protected virtual QueryContainer GetQueryContainer(TCriteria criteria, string query)
        {
            var funcs = GetQueryContainerFuncs(criteria, query).Where(f => f != null).ToArray();

            return GetQueryContainer(funcs);
        }

        protected virtual IEnumerable<Func<QueryContainerDescriptor<TItem>, QueryContainer>> GetQueryContainerFuncsForLanguages<TTranslation, TTranslationEnum>(string query, decimal boost,
            Expression<Func<TItem, TTranslation>> fieldExpression, bool fuziness = false, string field = null)
                where TTranslationEnum : struct, IEquatable<TTranslationEnum>
        {
            var languages = EnumUtils.GetEnumValues<TTranslationEnum>();

            string translationPropertyName = ReflectionUtils.GetNameOf(fieldExpression);

            foreach (var item in languages)
            {
                var expression = ReflectionUtils.CreateExpression<Func<TItem, string>>(typeof(TItem), $"{translationPropertyName}.{item}");

                yield return GetQueryContainerFunc<string, TTranslationEnum>(query, boost, expression, fuziness, field, item);
            }
        }

        protected virtual Func<QueryContainerDescriptor<TItem>, QueryContainer> GetQueryContainerFunc<TValue, TTranslationEnum>(string query, decimal boost,
            Expression<Func<TItem, TValue>> fieldExpression, bool fuziness = false, string field = null, TTranslationEnum? language = null, TTranslationEnum? currentLanguage = null)
                where TTranslationEnum : struct, IEquatable<TTranslationEnum>
        {
            if ((language == null || currentLanguage.Equals(language)) && boost > 0)
            {
                if (!fuziness)
                {
                    if (!string.IsNullOrWhiteSpace(field))
                    {
                        return f => f.Match(m => m.Field(i => fieldExpression.Compile().Invoke(i).Suffix(field)).Boost((double?)boost).Query(query));
                    }

                    return f => f.Match(m => m.Field(fieldExpression).Boost((double?)boost).Query(query));
                }

                return f => f.Match(m => m.Field(fieldExpression).Boost((double?)boost).Fuzziness(Fuzziness.Auto).Query(query));
            }

            return null;
        }

        protected abstract IEnumerable<Func<QueryContainerDescriptor<TItem>, QueryContainer>> GetQueryContainerFuncs(TCriteria criteria, string query);

        public virtual async Task<TKey[]> Search(QueryContainer queryContainer)
        {
            var sorts = new List<ISort>
            {
                new FieldSort { Field = "_score", Order = SortOrder.Descending }
            };

            var response = await elasticClient.SearchAsync<TItem>(new SearchRequest
            {
                Query = queryContainer,
                Sort = sorts,
                TrackScores = true
            });

            logger.LogInformation($"ElasticSearch took: {response.Took}ms");

            return Search(response);
        }

        private TKey[] Search(ISearchResponse<TItem> searchResponse)
        {
            return searchResponse?.Hits.ToArray(p => p.Id.ConvertOrCastTo<string, TKey>());
        }

        public virtual async Task<TKey[]> Search(string query, int page, int pageSize)
        {
            var response = await elasticClient.SearchAsync<TItem>(
                   s => s.Query(q => q.QueryString(d => d.Query(query)))
                           .From(page * pageSize)
                           .Size(pageSize));

            return Search(response);
        }

        public virtual async Task SaveSingleAsync(TItem item)
        {
            var key = KeyFunc(item);

            if (await sqlRepository.ExistsAsync(key))
            {
                await UpdateSingleAsync(item);
            }
            else
            {
                await AddSingleAsync(item);
            }
        }

        public virtual async Task SaveSingleAsync(TKey key)
        {
            if (await sqlRepository.ExistsAsync(key))
            {
                await UpdateSingleAsync(key);
            }
            else
            {
                await AddSingleAsync(key);
            }
        }


        public virtual Task AddSingleAsync(TItem item)
        {
            return elasticClient.IndexDocumentAsync(item);
        }

        public virtual async Task AddSingleAsync(TKey key)
        {
            var item = await ItemFunc(sqlRepository)(key);

            await AddSingleAsync(item);
        }

        public virtual async Task AddBulkAsync(IEnumerable<TItem> items)
        {
            var result = await elasticClient.BulkAsync(b => b.IndexMany(items));

            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    logger.LogError("Failed to index document {0}: {1}", itemWithError.Id, itemWithError.Error);
                }
            }
        }

        public virtual async Task AddBulkAsync(IEnumerable<TKey> itemIds)
        {
            var item = await ItemsFunc(sqlRepository)(itemIds);

            await AddBulkAsync(item);
        }

        public virtual Task AddBulkAsync(params TKey[] itemIds)
        {
            return AddBulkAsync(itemIds.AsEnumerable());
        }

        public virtual async Task AddBulkAsync()
        {
            var items = await AllItemsFunc(sqlRepository)();

            await AddBulkAsync(items);
        }


        public virtual Task UpdateSingleAsync(TItem item)
        {
            return elasticClient.UpdateAsync<TItem>(item, u => u.Doc(item));
        }

        public virtual async Task UpdateSingleAsync(TKey key)
        {
            var item = await ItemFunc(sqlRepository)(key);

            await UpdateSingleAsync(item);
        }

        public virtual async Task UpdateBulkAsync(IEnumerable<TItem> items)
        {
            var result = await elasticClient.BulkAsync(b => b.UpdateMany(items, (d, u) => d.Doc(u)));

            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    logger.LogError("Failed to update document {0}: {1}", itemWithError.Id, itemWithError.Error);
                }
            }
        }

        public virtual async Task UpdateBulkAsync(IEnumerable<TKey> itemIds)
        {
            var items = await ItemsFunc(sqlRepository)(itemIds);

            await UpdateBulkAsync(items);
        }

        public virtual Task UpdateBulkAsync(params TKey[] itemIds)
        {
            return UpdateBulkAsync(itemIds.AsEnumerable());
        }

        public virtual async Task UpdateBulkAsync()
        {
            var items = await AllItemsFunc(sqlRepository)();

            await UpdateBulkAsync(items);
        }


        public virtual Task DeleteSingleAsync(TItem item)
        {
            return elasticClient.DeleteAsync<TItem>(item);
        }

        public virtual async Task DeleteSingleAsync(TKey key)
        {
            var item = await ItemFunc(sqlRepository)(key);

            if (item != null)
                await DeleteSingleAsync(item);
        }

        public virtual async Task DeleteBulkAsync(IEnumerable<TKey> itemIds)
        {
            var result = await elasticClient.BulkAsync(b => b.DeleteMany<TItem>(ids: itemIds.ToArrayString()));

            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    logger.LogError("Failed to delete document {0}: {1}", itemWithError.Id, itemWithError.Error);
                }
            }
        }

        public virtual Task DeleteBulkAsync(params TKey[] itemIds)
        {
            return DeleteBulkAsync(itemIds.AsEnumerable());
        }

        public virtual Task DeleteBulkAsync(IEnumerable<TItem> items, Func<TItem, TKey> keyFunc)
        {
            var itemIds = items.Select(keyFunc);

            return DeleteBulkAsync(itemIds);
        }

        public virtual Task DeleteAll()
        {
            return elasticClient.DeleteByQueryAsync<TItem>(q => q.MatchAll());
        }


        public virtual async Task ReindexSingleAsync(TKey key)
        {
            var item = await ItemFunc(sqlRepository)(key);

            await DeleteSingleAsync(item);

            await AddSingleAsync(item);
        }

        public virtual async Task Reindex()
        {
            await DeleteAll();

            await AddBulkAsync();
        }


        public virtual void CreateIndex()
        {
            elasticClient.CreateIndex(configuration);
        }

        public virtual void DeleteIndex()
        {
            elasticClient.Deleteindex(configuration);
        }

        public virtual void RecreateIndex()
        {
            DeleteIndex();

            CreateIndex();
        }

        public virtual Task RecerateAndFillIndex()
        {
            RecreateIndex();

            return AddBulkAsync();
        }
    }
}
