using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using PDCoreNew.Context.IContext;
using PDCoreNew.Exceptions;
using PDCoreNew.Extensions;
using PDCoreNew.Helpers.Wrappers;
using PDCoreNew.Interfaces;
using PDCoreNew.Repositories.IRepo;
using PDCoreNew.Repositories.Repo;
using PDCoreNew.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories
{
    public class SqlRepositoryEntityFrameworkCore<T> : SqlRepository<T>, ISqlRepositoryEntityFrameworkAsync<T> where T : class, IModificationHistory
    {
        protected readonly IEntityFrameworkCoreDbContext ctx;
        protected readonly ILogger<T> loggerCore;
        protected readonly IMapper mapper;
        protected readonly DbSet<T> set;
        private readonly IDataAccessStrategy<T> dataAccessStrategy;

        public SqlRepositoryEntityFrameworkCore(IEntityFrameworkCoreDbContext ctx,
            ILogger<T> logger,
            IMapper mapper,
            IDataAccessStrategy<T> dataAccessStrategy) : base(ctx, null)
        {
            this.ctx = ctx;
            loggerCore = logger;
            this.mapper = mapper;
            set = this.ctx.Set<T>();
            this.dataAccessStrategy = dataAccessStrategy;
        }

        public static bool IsConnected { get; set; } = false;

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
            if (IsConnected)
                RemoveEmptyEntries();

            return ctx.SaveChangesWithModificationHistory();
        }

        protected virtual async ValueTask<int> DoCommitAsClientWins(bool sync, Func<Task<int>> commitAsync)
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
            return ReturnIfCompleted(() => DoCommitAsClientWins(true, null));
        }

        public ValueTask<int> CommitAsClientWinsAsync()
        {
            return DoCommitAsClientWins(false, CommitAsync);
        }

        protected virtual async ValueTask<int> DoCommitAsDatabaseWins(bool sync, Func<Task<int>> commitAsync)
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

        private static U ReturnIfCompleted<U>(Func<ValueTask<U>> valueTask)
        {
            U result = default;

            var value = valueTask();

            if (value.IsCompleted)
                result = value.Result;

            return result;
        }

        public int CommitAsDatabaseWins()
        {
            return ReturnIfCompleted(() => DoCommitAsDatabaseWins(true, null));
        }

        public ValueTask<int> CommitAsDatabaseWinsAsync()
        {
            return DoCommitAsDatabaseWins(false, CommitAsync);
        }

        public Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            if (IsConnected)
                RemoveEmptyEntries();

            return ctx.SaveChangesWithModificationHistoryAsync();
        }

        public Task<int> CommitAsync() => CommitAsync(CancellationToken.None);

        /// <summary>
        /// DbPropertyValues currentValues, DbPropertyValues databaseValues, DbPropertyValues resolvedValues
        /// </summary>
        public static event Action<PropertyValues, PropertyValues, PropertyValues> HaveUserResolveConcurrency;

        protected virtual async ValueTask<int> DoCommitWithOptimisticConcurrency(bool sync, Func<Task<int>> commitAsync)
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
            return ReturnIfCompleted(() => DoCommitWithOptimisticConcurrency(true, null));
        }

        public ValueTask<int> CommitWithOptimisticConcurrencyAsync()
        {
            return DoCommitWithOptimisticConcurrency(false, CommitAsync);
        }

        public int CommitWithoutValidation()
        {
            throw new NotSupportedFunctionalityException();
        }

        public override void Delete(T entity)
        {
            if (IsConnected)
                set.Remove(entity);
            else
                ctx.Entry(entity).State = EntityState.Deleted;
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

        protected virtual async ValueTask<bool> DoDeleteAndCommitWithOptimisticConcurrency(T entity, Action<string, string> writeError, bool sync, Func<Task<int>> commitAsync)
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
            return ReturnIfCompleted(() => DoDeleteAndCommitWithOptimisticConcurrency(entity, writeError, true, null));
        }

        public ValueTask<bool> DeleteAndCommitWithOptimisticConcurrencyAsync(T entity, Action<string, string> writeError)
        {
            return DoDeleteAndCommitWithOptimisticConcurrency(entity, writeError, false, CommitAsync);
        }

        public override void DeleteRange(IEnumerable<T> entities)
        {
            set.RemoveRange(entities);
        }

        public bool Exists<TKey>(TKey id)
        {
            var predicate = GetByIdPredicate(id);

            return FindAll().Any(predicate);
        }

        public Task<bool> ExistsAsync<TKey>(TKey id)
        {
            var predicate = GetByIdPredicate(id);

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
            return FindAll(!IsConnected);
        }

        public IQueryable<T> FindAll(bool asNoTracking)
        {
            IQueryable<T> query;

            if (asNoTracking)
                query = set.AsNoTracking();
            else
                query = set;

            return dataAccessStrategy?.PrepareQuery(query) ?? query;
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

        public ValueTask<T> FindByKeyValuesAsync(params object[] keyValues)
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
            return set.FromSqlRaw(query).ToList();
        }

        public Task<List<T>> GetByQueryAsync(string query)
        {
            return set.FromSqlRaw(query).ToListAsync();
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

        public ValueTask<T> FindByKeyValuesAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return set.FindAsync(cancellationToken, keyValues);
        }

        public Expression<Func<T, bool>> GetByIdPredicate<TKey>(TKey id)
        {
            return RepositoryUtils.GetByIdPredicate<T, TKey>(id);
        }

        //Funkcjonalność ConnectedRepository. Nie ma potrzeby pobierania danych wiele razy. Repository i kontekst żyją w danym oknie.
        public virtual LocalView<T> GetAllFromMemory()
        {
            if (set.Local.IsEmpty())
            {
                Load();
            }

            return set.Local;
        }

        public virtual Task LoadAsync()
        {
            return set.LoadAsync();
        }

        public virtual void Load()
        {
            set.Load();
        }


        //Funkcjonalność ConnectedRepository. Nie ma potrzeby pobierania danych wiele razy. Repository i kontekst żyją w danym oknie.
        public virtual async ValueTask<LocalView<T>> GetAllFromMemoryAsync()
        {
            if (set.Local.IsEmpty())
            {
                await LoadAsync();
            }

            return set.Local;
        }


        //Funkcjonalność ConnectedRepository, np. do Bindingu obiektu w WPF.
        public virtual T Add()
        {
            var entry = Activator.CreateInstance<T>();

            Add(entry);

            return entry;
        }


        //Funkcjonalność ConnectedRepository. Pozbycie się z pamięci przed zapisem obiektów utworzonych, ale niezedytowanych.
        private void RemoveEmptyEntries()
        {
            T entry;

            //you can't remove from or add to a collection in a foreach loop
            for (int i = set.Local.Count; i > 0; i--)
            {
                entry = set.Local.ElementAt(i - 1);

                if (ctx.Entry(entry).State == EntityState.Added && !entry.IsDirty)
                {
                    Delete(entry);
                }
            }
        }

        public T AddAndReturn(T entity)
        {
            Add(entity);

            return entity;
        }

        public EntityEntry<T> AddAndReturnEntry(T entity)
        {
            return set.Add(entity);
        }

        public virtual void SaveNew(T entity)
        {
            Add(entity);

            Commit();
        }

        public virtual ValueTask<int> SaveNewAsync(T entity)
        {
            Add(entity);

            return CommitAsClientWinsAsync();
        }

        public async virtual Task SaveNewAsync<TInput>(TInput input)
        {
            var entity = mapper.Map<T>(input);

            await SaveNewAsync(entity);

            mapper.Map(entity, input);
        }

        public virtual void SaveUpdated(T entity)
        {
            Update(entity);

            Commit();
        }

        public virtual Task SaveUpdatedAsync(T entity)
        {
            Update(entity);

            return CommitAsync();
        }

        private async ValueTask<bool> DoSaveUpdatedWithOptimisticConcurrency(T entity, Action<string, string> writeError,
            Action<string> cleanRowVersion, bool sync, bool update, bool? include,
            params Expression<Func<T, object>>[] properties)
        {
            if (update)
            {
                if (include == null)
                {
                    Update(entity);
                }
                else
                {
                    UpdateWithIncludeOrExcludeProperties(entity, include.Value, properties);
                }
            }

            bool result = false;

            try
            {
                if (sync)
                    Commit();
                else
                    await CommitAsync();

                result = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ex.HandleExceptionOnEdit(entity, writeError, cleanRowVersion);
            }
            catch (RetryLimitExceededException dex)
            {
                logger.Error("An error occurred while trying to update the entity", dex);

                writeError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return result;
        }

        public virtual bool SaveUpdatedWithOptimisticConcurrency(T entity, Action<string, string> writeError,
            Action<string> cleanRowVersion, bool update = true, bool? include = null,
            params Expression<Func<T, object>>[] properties)
        {
            return ReturnIfCompleted(() => DoSaveUpdatedWithOptimisticConcurrency(entity, writeError, cleanRowVersion, true, update, include, properties));
        }

        public virtual ValueTask<bool> SaveUpdatedWithOptimisticConcurrencyAsync(T entity,
            Action<string, string> writeError, Action<string> cleanRowVersion, bool update = true, bool? include = null,
            params Expression<Func<T, object>>[] properties)
        {
            return DoSaveUpdatedWithOptimisticConcurrency(entity, writeError, cleanRowVersion, false, update, include, properties);
        }

        public virtual void DeleteByKeyValues(params object[] keyValues)
        {
            var entry = FindByKeyValues(keyValues);

            if (entry == null)
                return; // not found; assume already deleted.

            Delete(entry);
        }

        public virtual void DeleteAndCommit(params object[] keyValues)
        {
            DeleteByKeyValues(keyValues);

            Commit();
        }
        public virtual Task DeleteAndCommitAsync(params object[] keyValues)
        {
            DeleteByKeyValues(keyValues);

            return CommitAsync();
        }

        public virtual void Update(T entity, IHasRowVersion dto)
        {
            var entry = ctx.Entry(entity);

            entry.Property(e => e.RowVersion).OriginalValue = dto.RowVersion;

            entry.CurrentValues.SetValues(dto);
        }

        public virtual ValueTask<bool> SaveUpdatedWithOptimisticConcurrencyAsync(T entity, IPrincipal principal,
            Action<string, string> writeError, Action<string> cleanRowVersion,
            IDataAccessStrategy<T> savingStrategy = default)
        {
            savingStrategy ??= dataAccessStrategy;

            ValueTask<bool> result;

            if (savingStrategy?.CanUpdate(entity) ?? true)
            {
                if (savingStrategy?.CanUpdateAllProperties(entity) ?? true)
                {
                    Update(entity);
                }
                else
                {
                    var properties = savingStrategy.GetPropertiesForUpdate(entity);

                    UpdateWithIncludeOrExcludeProperties(entity, true, properties);
                }

                result = SaveUpdatedWithOptimisticConcurrencyAsync(entity, writeError, cleanRowVersion, false);
            }
            else
            {
                writeError(string.Empty, "Access denied");

                result = ValueTask.FromResult(false);
            }

            return result;
        }

        public virtual async ValueTask<TOutput> SaveUpdatedWithOptimisticConcurrencyAsync<TOutput>(IHasRowVersion input,
            IPrincipal principal, Action<string, string> writeError, Action<string> cleanRowVersion,
            IDataAccessStrategy<T> savingStrategy = default)
        {
            TOutput result = default;

            var entity = mapper.Map<T>(input);

            bool success = await SaveUpdatedWithOptimisticConcurrencyAsync(entity, principal, writeError, cleanRowVersion, savingStrategy);

            if (success)
                result = mapper.Map<TOutput>(entity);

            return result;
        }

        public virtual async ValueTask<bool> SaveUpdatedWithOptimisticConcurrencyAsync(IHasRowVersion input,
            IPrincipal principal, Action<string, string> writeError, Action<string> cleanRowVersion,
            IDataAccessStrategy<T> savingStrategy = default)
        {
            bool result = false;

            var entity = mapper.Map<T>(input);

            result = await SaveUpdatedWithOptimisticConcurrencyAsync(entity, principal, writeError, cleanRowVersion, savingStrategy);

            if (result)
                mapper.Map(entity, input);

            return result;
        }

        public virtual async ValueTask<TOutput> SaveUpdatedWithOptimisticConcurrencyAsync<TOutput>(IHasRowVersion source,
            T destination, IPrincipal principal, Action<string, string> writeError, Action<string> cleanRowVersion,
            IDataAccessStrategy<T> savingStrategy = default)
        {
            savingStrategy ??= dataAccessStrategy;

            TOutput result = default;

            if (savingStrategy?.CanUpdate(destination) ?? true)
            {
                if (savingStrategy?.CanUpdateAllProperties(destination) ?? true)
                {
                    mapper.Map(source, destination);
                }
                else
                {
                    var properties = savingStrategy.GetPropertiesForUpdate(destination);

                    UpdateWithIncludeOrExcludeProperties(source, destination, true, properties);
                }

                bool success = await SaveUpdatedWithOptimisticConcurrencyAsync(destination, writeError, cleanRowVersion, false);

                if (success)
                    result = mapper.Map<TOutput>(destination);
            }
            else
            {
                writeError(string.Empty, "Access denied");
            }

            return result;
        }

        public async Task<bool> SaveNewAsync<TInput>(TInput input, IPrincipal principal, IDataAccessStrategy<T> savingStrategy = default, params object[] args)
        {
            savingStrategy ??= dataAccessStrategy;

            savingStrategy.ThrowIfNull(nameof(savingStrategy));

            bool result = false;

            args = args.Concat(input);

            bool canAdd = await savingStrategy.CanAdd(args);

            if (canAdd)
            {
                savingStrategy.PrepareForAdd(args);

                await SaveNewAsync(input); //I tak EF nie obsługuje operacji równoległych

                result = true;
            }

            return result;
        }

        public virtual ValueTask<bool> DeleteAndCommitWithOptimisticConcurrencyAsync(T entity, IPrincipal principal,
            Action<string, string> writeError, IDataAccessStrategy<T> savingStrategy = default)
        {
            savingStrategy ??= dataAccessStrategy;

            ValueTask<bool> result;

            if (!(savingStrategy?.CanDelete(entity) ?? false))
            {
                writeError(string.Empty, "Access denied");

                result = ValueTask.FromResult(false);
            }
            else
            {
                result = DeleteAndCommitWithOptimisticConcurrencyAsync(entity, writeError);
            }

            return result;
        }
    }
}
