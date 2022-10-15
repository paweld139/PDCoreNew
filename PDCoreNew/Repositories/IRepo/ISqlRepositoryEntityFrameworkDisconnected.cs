using PDCoreNew.Interfaces;
using System;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.IRepo
{
    public interface ISqlRepositoryEntityFrameworkDisconnected<T> : ISqlRepositoryEntityFrameworkAsync<T> where T : class, IModificationHistory
    {
        Task<bool> SaveUpdatedWithOptimisticConcurrencyAsync(T entity, Action<string, string> writeError, bool update = true, bool? include = null, params Expression<Func<T, object>>[] properties);


        void Delete(params object[] keyValues);


        Task<bool> SaveUpdatedWithOptimisticConcurrencyAsync(T entity, IPrincipal principal, Action<string, string> writeError, IDataAccessStrategy<T> savingStrategy = default);


        Task<bool> SaveUpdatedWithOptimisticConcurrencyAsync(IHasRowVersion input, IPrincipal principal, Action<string, string> writeError, IDataAccessStrategy<T> savingStrategy = default);

        Task<TOutput> SaveUpdatedWithOptimisticConcurrencyAsync<TOutput>(IHasRowVersion input, IPrincipal principal, Action<string, string> writeError, IDataAccessStrategy<T> savingStrategy = default);


        Task<TOutput> SaveUpdatedWithOptimisticConcurrencyAsync<TOutput>(IHasRowVersion source, T destination, IPrincipal principal, Action<string, string> writeError, IDataAccessStrategy<T> savingStrategy = default);
    }
}
