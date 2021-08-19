using Microsoft.EntityFrameworkCore.ChangeTracking;
using PDCoreNew.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.IRepo
{
    public interface ISqlRepositoryEntityFrameworkAsync<T> : ISqlRepositoryEntityFramework<T> where T : class, IModificationHistory
    {
        Task<T> FindByIdAsync(int id);

        Task<T> FindByIdAsync(int id, bool asNoTracking);

        Task<List<T>> GetByQueryAsync(string query);

        Task<List<T>> GetByWhereAsync(string where);

        Task<List<T>> GetAllAsync(bool asNoTracking);

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate);

        Task<List<TOutput>> GetAllAsync<TOutput>();

        Task<List<KeyValuePair<TKey, TValue>>> GetKeyValuePairsAsync<TKey, TValue>(Func<T, TKey> keySelector, Func<T, TValue> valueSelector, bool sortByValue = true) where TValue : IComparable<TValue>;

        Task<List<T>> GetByFilterAsync(Expression<Func<T, string>> propertySelector, string substring);

        Task<List<T>> GetPageAsync(int page, int pageSize);

        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate = null);

        Task<List<TOutput>> GetAsync<TOutput>(Expression<Func<T, bool>> predicate);

        Task<TOutput> FindByIdAsync<TOutput>(int id);


        Task<int> CommitAsync();

        Task<int> CommitAsync(CancellationToken cancellationToken);

        Task DeleteAndCommitAsync(T entity);

        ValueTask<int> CommitAsClientWinsAsync();

        ValueTask<int> CommitAsDatabaseWinsAsync();

        ValueTask<int> CommitWithOptimisticConcurrencyAsync();

        ValueTask<bool> DeleteAndCommitWithOptimisticConcurrencyAsync(T entity, Action<string, string> writeError);
        ValueTask<T> FindByKeyValuesAsync(params object[] keyValues);
        ValueTask<T> FindByKeyValuesAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task<bool> ExistsAsync<TKey>(TKey id);

        LocalView<T> GetAllFromMemory();

        ValueTask<LocalView<T>> GetAllFromMemoryAsync();


        T Add();
        T AddAndReturn(T entity);
        Task LoadAsync();
        EntityEntry<T> AddAndReturnEntry(T entity);
        void SaveNew(T entity);
        ValueTask<int> SaveNewAsync(T entity);
        Task SaveNewAsync<TInput>(TInput input);
        void SaveUpdated(T entity);
        Task SaveUpdatedAsync(T entity);
        bool SaveUpdatedWithOptimisticConcurrency(T entity, Action<string, string> writeError, Action<string> cleanRowVersion, bool update = true, bool? include = null, params Expression<Func<T, object>>[] properties);
        ValueTask<bool> SaveUpdatedWithOptimisticConcurrencyAsync(T entity, Action<string, string> writeError, Action<string> cleanRowVersion, bool update = true, bool? include = null, params Expression<Func<T, object>>[] properties);
        void DeleteByKeyValues(params object[] keyValues);
        void DeleteAndCommit(params object[] keyValues);
        Task DeleteAndCommitAsync(params object[] keyValues);
        void Update(T entity, IHasRowVersion dto);
        ValueTask<bool> SaveUpdatedWithOptimisticConcurrencyAsync(T entity, IPrincipal principal, Action<string, string> writeError, Action<string> cleanRowVersion, IDataAccessStrategy<T> savingStrategy = null);
        ValueTask<TOutput> SaveUpdatedWithOptimisticConcurrencyAsync<TOutput>(IHasRowVersion input, IPrincipal principal, Action<string, string> writeError, Action<string> cleanRowVersion, IDataAccessStrategy<T> savingStrategy = null);
        ValueTask<bool> SaveUpdatedWithOptimisticConcurrencyAsync(IHasRowVersion input, IPrincipal principal, Action<string, string> writeError, Action<string> cleanRowVersion, IDataAccessStrategy<T> savingStrategy = null);
        ValueTask<TOutput> SaveUpdatedWithOptimisticConcurrencyAsync<TOutput>(IHasRowVersion source, T destination, IPrincipal principal, Action<string, string> writeError, Action<string> cleanRowVersion, IDataAccessStrategy<T> savingStrategy = null);
        Task<bool> SaveNewAsync<TInput>(TInput input, IPrincipal principal, IDataAccessStrategy<T> savingStrategy = null, params object[] args);
        ValueTask<bool> DeleteAndCommitWithOptimisticConcurrencyAsync(T entity, IPrincipal principal, Action<string, string> writeError, IDataAccessStrategy<T> savingStrategy = null);
    }
}
