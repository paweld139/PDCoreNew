using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using PDCore.Context.IContext;
using PDCore.Exceptions;
using PDCore.Extensions;
using PDCore.Helpers.Wrappers;
using PDCore.Interfaces;
using PDCore.Repositories.IRepo;
using PDCore.Repositories.Repo;
using PDCore.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PDCore.Repositories
{
    public class SqlRepositoryEntityFrameworkCore<T> : SqlRepository<T>, ISqlRepositoryEntityFrameworkAsync<T> where T : class, IModificationHistory
    {
        protected readonly IEntityFrameworkCoreDbContext ctx;
        protected readonly ILogger<T> loggerCore;
        protected readonly IMapper mapper;
        protected readonly DbSet<T> set;

        public SqlRepositoryEntityFrameworkCore(IEntityFrameworkCoreDbContext ctx, ILogger<T> logger, IMapper mapper) : base(ctx, null)
        {
            this.ctx = ctx;
            loggerCore = logger;
            this.mapper = mapper;
            set = this.ctx.Set<T>();
        }

        protected override string ConnectionString => ctx.Database.GetDbConnection().ConnectionString;

        public override void Add(T newEntity)
        {
            set.Add(newEntity);
        }

        public override void AddRange(IEnumerable<T> newEntities)
        {
            set.AddRange(newEntities);
        }

        public void Attach(T obj)
        {
            set.Attach(obj);
        }

        public override int Commit()
        {
            return ctx.SaveChanges();
        }

        protected virtual async Task<int> DoCommitAsClientWins(bool sync, Func<Task<int>> commitAsync)
        {
            bool saved = false;

            int result = 0;

            do
            {
                try
                {
                    if (sync)
                        result = Commit(); //Zwraca ilość wierszy wziętych po uwagę
                    else
                        result = await commitAsync();

                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        var databaseValues = entry.GetDatabaseValues();

                        entry.OriginalValues.SetValues(databaseValues);
                    }
                }
            }
            while (!saved);

            return result;
        }

        public int CommitAsClientWins()
        {
            return DoCommitAsClientWins(true, null).Result;
        }

        public Task<int> CommitAsClientWinsAsync()
        {
            return DoCommitAsClientWins(false, CommitAsync);
        }

        protected virtual async Task<int> DoCommitAsDatabaseWins(bool sync, Func<Task<int>> commitAsync)
        {
            bool saved = false;

            int result = 0;

            do
            {
                try
                {
                    if (sync)
                        result = Commit(); //Zwraca ilość wierszy wziętych po uwagę
                    else
                        result = await commitAsync();

                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (sync)
                            entry.Reload();
                        else
                            await entry.ReloadAsync();
                    }
                }
            }
            while (!saved);

            return result;
        }

        public int CommitAsDatabaseWins()
        {
            return DoCommitAsDatabaseWins(true, null).Result;
        }

        public Task<int> CommitAsDatabaseWinsAsync()
        {
            return DoCommitAsDatabaseWins(false, CommitAsync);
        }

        public Task<int> CommitAsync()
        {
            return ctx.SaveChangesWithModificationHistoryAsync();
        }

        /// <summary>
        /// DbPropertyValues currentValues, DbPropertyValues databaseValues, DbPropertyValues resolvedValues
        /// </summary>
        public static event Action<PropertyValues, PropertyValues, PropertyValues> HaveUserResolveConcurrency;

        protected virtual async Task<int> DoCommitWithOptimisticConcurrency(bool sync, Func<Task<int>> commitAsync)
        {
            bool saved = false;

            int result = 0;

            do
            {
                try
                {
                    if (sync)
                        result = Commit(); //Zwraca ilość wierszy wziętych po uwagę
                    else
                        result = await commitAsync();

                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        var currentValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        // Choose an initial set of resolved values. In this case we
                        // make the default be the values currently in the database.
                        var resolvedValues = currentValues.Clone();

                        // Have the user choose what the resolved values should be
                        HaveUserResolveConcurrency?.Invoke(currentValues, databaseValues, resolvedValues);

                        // Update the original values with the database values and
                        // the current values with whatever the user choose.
                        entry.OriginalValues.SetValues(databaseValues);
                        entry.CurrentValues.SetValues(resolvedValues);
                    }
                }
            }
            while (!saved);

            return result;
        }

        public int CommitWithOptimisticConcurrency()
        {
            return DoCommitWithOptimisticConcurrency(true, null).Result;
        }

        public Task<int> CommitWithOptimisticConcurrencyAsync()
        {
            return DoCommitWithOptimisticConcurrency(false, CommitAsync);
        }

        public int CommitWithoutValidation()
        {
            throw new NotSupportedFunctionalityException();
        }

        public override void Delete(T entity)
        {
            set.Remove(entity);
        }

        public void DeleteAndCommit(T entity)
        {
            Delete(entity);

            Commit();
        }

        public Task DeleteAndCommitAsync(T entity)
        {
            Delete(entity);

            return CommitAsync();
        }

        protected virtual async Task<bool> DoDeleteAndCommitWithOptimisticConcurrency(T entity, Action<string, string> writeError, bool sync, Func<Task<int>> commitAsync)
        {
            Delete(entity);

            bool result = false;

            try
            {
                if (sync)
                    Commit();
                else
                    await commitAsync();

                result = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();

                var databaseEntry = entry.GetDatabaseValues();

                if (databaseEntry == null)
                {
                    result = true;
                }
                else
                {
                    writeError(string.Empty, "The record you attempted to delete "
                        + "was modified by another user after you got the original values. "
                        + "The delete operation was canceled and the current values in the "
                        + "database have been displayed. If you still want to delete this "
                        + "record, delete again.");

                    if (sync)
                        entry.Reload();
                    else
                        await entry.ReloadAsync();
                }
            }
            catch (DataException dex)
            {
                logger.Error("An error occurred while trying to delete the entity", dex);

                writeError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
            }

            return result;
        }

        public bool DeleteAndCommitWithOptimisticConcurrency(T entity, Action<string, string> writeError)
        {
            return DoDeleteAndCommitWithOptimisticConcurrency(entity, writeError, true, null).Result;
        }

        public Task<bool> DeleteAndCommitWithOptimisticConcurrencyAsync(T entity, Action<string, string> writeError)
        {
            return DoDeleteAndCommitWithOptimisticConcurrency(entity, writeError, false, CommitAsync);
        }

        public override void DeleteRange(IEnumerable<T> entities)
        {
            set.RemoveRange(entities);
        }

        public bool Exists<TKey>(TKey id)
        {
            var predicate = RepositoryUtils.GetByIdPredicate<T, TKey>(id);

            return FindAll().Any(predicate);
        }

        public Task<bool> ExistsAsync<TKey>(TKey id)
        {
            var predicate = RepositoryUtils.GetByIdPredicate<T, TKey>(id);

            return FindAll().AnyAsync(predicate);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return FindAll().Where(predicate);
        }

        public IQueryable<TOutput> Find<TOutput>(Expression<Func<T, bool>> predicate)
        {
            return mapper.ProjectTo<TOutput>(Find(predicate));
        }

        public override IQueryable<T> FindAll()
        {
            return FindAll(false);
        }

        public IQueryable<T> FindAll(bool asNoTracking)
        {
            if (asNoTracking)
                return set.AsNoTracking();

            return set;
        }

        public IQueryable<TOutput> FindAll<TOutput>()
        {
            return mapper.ProjectTo<TOutput>(FindAll());
        }

        public IQueryable<TOutput> FindAll<TOutput>(bool asNoTracking)
        {
            return mapper.ProjectTo<TOutput>(FindAll(asNoTracking));
        }

        public IQueryable<TOutput> FindBy<TOutput>(Expression<Func<T, bool>> predicate, Expression<Func<T, TOutput>> columns)
        {
            return Find(predicate).Select(columns);
        }

        public IQueryable<T> FindByDateCreated(string dateF, string dateT)
        {
            return FindAll().FindByDateCreated(dateF, dateT);
        }

        public IQueryable<T> FindByDateCreated(DateTime? dateF, DateTime? dateT)
        {
            return FindAll().FindByDateCreated(dateF, dateT);
        }

        public IQueryable<TOutput> FindByDateCreated<TOutput>(DateTime? dateF, DateTime? dateT)
        {
            return mapper.ProjectTo<TOutput>(FindByDateCreated(dateF, dateT));
        }

        public IQueryable<T> FindByDateModified(string dateF, string dateT)
        {
            return FindAll().FindByDateModified(dateF, dateT);
        }

        public IQueryable<T> FindByDateModified(DateTime? dateF, DateTime? dateT)
        {
            return FindAll().FindByDateModified(dateF, dateT);
        }

        public IQueryable<T> FindByFilter(Expression<Func<T, string>> propertySelector, string substring)
        {
            var query = FindAll();

            if (!string.IsNullOrWhiteSpace(substring))
                query = query.Filter(substring, propertySelector);

            return query;
        }

        public IQueryable<TOutput> FindByFilter<TOutput>(Expression<Func<T, string>> propertySelector, string substring)
        {
            return mapper.ProjectTo<TOutput>(FindByFilter(propertySelector, substring));
        }

        public override T FindById(int id)
        {
            return FindByKeyValues(id);
        }

        public Task<T> FindByIdAsync(int id)
        {
            return FindAll().SingleOrDefaultAsync(GetByIdPredicate(id));
        }

        public Task<T> FindByIdAsync(int id, bool asNoTracking)
        {
            return FindAll(asNoTracking).SingleOrDefaultAsync(GetByIdPredicate(id));
        }

        public Task<TOutput> FindByIdAsync<TOutput>(int id)
        {
            return Find<TOutput>(GetByIdPredicate(id)).SingleOrDefaultAsync();
        }

        public T FindByKeyValues(params object[] keyValues)
        {
            return set.Find(keyValues);
        }

        public Task<T> FindByKeyValuesAsync(params object[] keyValues)
        {
            return set.FindAsync(keyValues);
        }

        public IQueryable<T> FindPage(int page, int pageSize)
        {
            var query = FindAll();

            if (page > 0 && pageSize > 0)
            {
                query = query.OrderByDescending(e => e.DateCreated).GetPage(page, pageSize);
            }

            return query;
        }

        public IQueryable<TOutput> FindPage<TOutput>(int page, int pageSize)
        {
            return mapper.ProjectTo<TOutput>(FindPage(page, pageSize));
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return Find(predicate).ToList();
        }

        public override IEnumerable<T> GetAll()
        {
            return FindAll().ToList();
        }

        public IEnumerable<T> GetAll(bool asNoTracking)
        {
            return FindAll(asNoTracking).ToList();
        }

        public IEnumerable<TOutput> GetAll<TOutput>()
        {
            return FindAll<TOutput>().ToList();
        }

        public Task<List<T>> GetAllAsync(bool asNoTracking)
        {
            return FindAll(asNoTracking).ToListAsync();
        }

        public Task<List<T>> GetAllAsync()
        {
            return FindAll().ToListAsync();
        }

        public Task<List<TOutput>> GetAllAsync<TOutput>()
        {
            return FindAll<TOutput>().ToListAsync();
        }

        public Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return Find(predicate).ToListAsync();
        }

        public Task<List<TOutput>> GetAsync<TOutput>(Expression<Func<T, bool>> predicate)
        {
            return Find<TOutput>(predicate).ToListAsync();
        }

        public IEnumerable<T> GetByFilter(Expression<Func<T, string>> propertySelector, string substring)
        {
            return FindByFilter(propertySelector, substring).ToList();
        }

        public Task<List<T>> GetByFilterAsync(Expression<Func<T, string>> propertySelector, string substring)
        {
            return FindByFilter(propertySelector, substring).ToListAsync();
        }

        public Expression<Func<T, bool>> GetByIdPredicate(int id) => RepositoryUtils.GetByIdPredicate<T, int>(id);

        public override List<T> GetByQuery(string query)
        {
            return set.FromSql(query).ToList();
        }

        public Task<List<T>> GetByQueryAsync(string query)
        {
            return set.FromSql(query).ToListAsync();
        }

        public Task<List<T>> GetByWhereAsync(string where)
        {
            string query = GetQuery(where);

            return GetByQueryAsync(query);
        }

        public int GetCount(Expression<Func<T, bool>> predicate)
        {
            return FindAll().Count(predicate);
        }

        public Task<int> GetCountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
                return FindAll().CountAsync(predicate);
            else
                return FindAll().CountAsync();
        }

        public override DataTable GetDataTableByQuery(string query)
        {
            return DbLogWrapper.Execute(ctx.DataTable, query, ConnectionString, logger, IsLoggingEnabled);
        }

        public override DataTable GetDataTableByWhere(string where)
        {
            var list = GetByWhere(where);

            return ReflectionUtils.CreateDataTable(list);
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairs<TKey, TValue>(Func<T, TKey> keySelector, Func<T, TValue> valueSelector, bool sortByValue = true) where TValue : IComparable<TValue>
        {
            var query = GetAll().GetKVP(keySelector, valueSelector);

            if (sortByValue)
            {
                query = query.OrderBy(e => e.Value);
            }

            return query;
        }

        public async Task<List<KeyValuePair<TKey, TValue>>> GetKeyValuePairsAsync<TKey, TValue>(Func<T, TKey> keySelector, Func<T, TValue> valueSelector, bool sortByValue = true) where TValue : IComparable<TValue>
        {
            var query = (await GetAllAsync()).AsEnumerable().GetKVP(keySelector, valueSelector);

            if (sortByValue)
            {
                query = query.OrderBy(e => e.Value);
            }

            return query.ToList();
        }

        public IEnumerable<T> GetPage(int page, int pageSize)
        {
            return FindPage(page, pageSize).ToList();
        }

        public Task<List<T>> GetPageAsync(int page, int pageSize)
        {
            return FindPage(page, pageSize).ToListAsync();
        }

        public override string GetQuery()
        {
            return FindAll().ToSql();
        }

        public override void Update(T entity)
        {
            set.Update(entity);
        }

        public bool UpdateWithIncludeOrExcludeProperties(T item, bool include, params string[] propertyNames)
        {
            return UpdateWithIncludeOrExcludeProperties(item, include, propertyNames.AsEnumerable());
        }

        public virtual bool UpdateWithIncludeOrExcludeProperties(T item, bool include, IEnumerable<Expression<Func<T, object>>> properties)
        {
            var changedPropertyNames = properties.ToArray(ReflectionUtils.GetNameOf);

            return UpdateWithIncludeOrExcludeProperties(item, include, changedPropertyNames);
        }

        public bool UpdateWithIncludeOrExcludeProperties(T item, bool include, params Expression<Func<T, object>>[] properties)
        {
            return UpdateWithIncludeOrExcludeProperties(item, include, properties.AsEnumerable());
        }

        public bool UpdateWithIncludeOrExcludeProperties(T item, bool include, IEnumerable<string> propertyNames)
        {
            if (include)
            {
                Attach(item);
            }
            else
            {
                Update(item);
            }

            var entry = ctx.Entry(item);

            foreach (var propertyName in propertyNames)
            {
                // If we can't find the property, this line wil throw an exception, 
                //which is good as we want to know about it
                entry.Property(propertyName).IsModified = include;
            }

            return true;
        }

        public bool UpdateWithIncludeOrExcludeProperties(IHasRowVersion source, T destination, bool include, ICollection<string> propertyNames)
        {
            var destinationEntry = ctx.Entry(destination);

            destinationEntry.Property(e => e.RowVersion).OriginalValue = source.RowVersion;

            var sourceProperties = source.GetProperties();

            foreach (var property in sourceProperties)
            {
                string propertyName = property.Name;

                bool contains = propertyNames.Contains(propertyName);

                if ((contains && include) || (!contains && !include))
                {
                    var sourceValue = property.GetPropertyValue(source);

                    destination.SetPropertyValue(propertyName, sourceValue);
                }
            }

            return true;
        }

        public bool UpdateWithIncludeOrExcludeProperties(IHasRowVersion source, T destination, bool include, params Expression<Func<T, object>>[] properties)
        {
            var propertyNames = properties.ConvertArray(ReflectionUtils.GetNameOf);

            return UpdateWithIncludeOrExcludeProperties(source, destination, include, propertyNames);
        }

        public Task<T> FindByKeyValuesAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return set.FindAsync(cancellationToken, keyValues);
        }

        public Expression<Func<T, bool>> GetByIdPredicate<TKey>(TKey id)
        {
            return RepositoryUtils.GetByIdPredicate<T, TKey>(id);
        }
    }
}
