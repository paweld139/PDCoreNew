using System;
using System.Collections.Generic;

namespace PDCore.Repositories.IRepo
{
    public interface IWriteOnlyRepository<in T> : IDisposable
    {
        void Add(T newEntity);

        void AddRange(IEnumerable<T> newEntities);

        void Update(T entity);


        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);


        int Commit();
    }
}
