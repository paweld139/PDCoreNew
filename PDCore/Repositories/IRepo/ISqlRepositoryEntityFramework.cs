﻿using PDCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PDCore.Repositories.IRepo
{
    public interface ISqlRepositoryEntityFramework<T> : ISqlRepository<T> where T : class, IModificationHistory
    {
        IQueryable<T> FindAll(bool asNoTracking);

        //IQueryable<KeyValuePair<TKey, TValue>> FindKeyValuePairs<TKey, TValue>(Expression<Func<T, TKey>> keySelector, Expression<Func<T, TValue>> valueSelector, bool sortByValue = true) where TValue : IComparable<TValue>;

        IQueryable<T> FindByFilter(Expression<Func<T, string>> propertySelector, string substring);

        IQueryable<TOutput> FindByFilter<TOutput>(Expression<Func<T, string>> propertySelector, string substring);

        IQueryable<T> FindPage(int page, int pageSize);

        IQueryable<TOutput> FindPage<TOutput>(int page, int pageSize);

        IQueryable<T> FindByDateCreated(string dateF, string dateT);

        IQueryable<T> FindByDateCreated(DateTime? dateF, DateTime? dateT);

        IQueryable<TOutput> FindByDateCreated<TOutput>(DateTime? dateF, DateTime? dateT);

        IQueryable<T> FindByDateModified(string dateF, string dateT);

        IQueryable<T> FindByDateModified(DateTime? dateF, DateTime? dateT);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        IQueryable<TOutput> FindBy<TOutput>(Expression<Func<T, bool>> predicate, Expression<Func<T, TOutput>> columns);

        IQueryable<TOutput> FindAll<TOutput>();

        IQueryable<TOutput> Find<TOutput>(Expression<Func<T, bool>> predicate);

        Expression<Func<T, bool>> GetByIdPredicate(long id);


        T FindByKeyValues(params object[] keyValues);

        IEnumerable<T> GetAll(bool asNoTracking);

        IEnumerable<TOutput> GetAll<TOutput>();

        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);

        IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairs<TKey, TValue>(Func<T, TKey> keySelector, Func<T, TValue> valueSelector, bool sortByValue = true) where TValue : IComparable<TValue>;

        IEnumerable<T> GetByFilter(Expression<Func<T, string>> propertySelector, string substring);

        IEnumerable<T> GetPage(int page, int pageSize);

        int GetCount(Expression<Func<T, bool>> predicate);

        bool Exists(int id);


        void Attach(T obj);

        void DeleteAndCommit(T entity);

        int CommitAsClientWins();

        int CommitAsDatabaseWins();

        int CommitWithOptimisticConcurrency();

        bool DeleteAndCommitWithOptimisticConcurrency(T entity, Action<string, string> writeError);

        bool UpdateWithIncludeOrExcludeProperties(T item, bool include, params string[] propertyNames);

        bool UpdateWithIncludeOrExcludeProperties(T item, bool include, params Expression<Func<T, object>>[] properties);

        bool UpdateWithIncludeOrExcludeProperties(T item, bool include, IEnumerable<string> propertyNames);

        int CommitWithoutValidation();

        bool UpdateWithIncludeOrExcludeProperties(IHasRowVersion source, T destination, bool include, ICollection<string> propertyNames);

        bool UpdateWithIncludeOrExcludeProperties(IHasRowVersion source, T destination, bool include, params Expression<Func<T, object>>[] properties);

        IQueryable<TOutput> FindAll<TOutput>(bool asNoTracking);
    }
}
