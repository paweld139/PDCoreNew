using System;
using System.Collections.Generic;
using System.Linq;

namespace PDCoreNew.Repositories.IRepo
{
    public interface IReadOnlyRepository<out T> : IDisposable
    {
        T FindById(int id);

        IQueryable<T> FindAll();

        IEnumerable<T> GetAll();

        int GetCount();
    }
}
